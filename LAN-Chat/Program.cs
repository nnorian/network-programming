// console ui, command parser and lifecycle management

using System;
using System.Net;

namespace LanChat{

    public static class Program{

        static volatile bool running = true;

        public static void Main(string[] args){
            if (args.Length < 3){
                Console.WriteLine("Usage: dotnet run -- <name> <port> <passphrase>");
                return;
            }

            string name = args[0].Trim();
            if (!Protocol.IsValidName(name)){
                Console.WriteLine("Error: name must be 1-20 characters of A-Z, a-z, 0-9, _ or -");
                return;
            }

            if (!int.TryParse(args[1], out int port) || port < 1024 || port > 65535){
                Console.WriteLine("Error: port must be a valid number between 1024 and 65535");
                return;
            }

            string passphrase = args[2];
            if (string.IsNullOrEmpty(passphrase)){
                Console.WriteLine("Error: passphrase cannot be empty");
                return;
            }

            // task 2: setup logging and identities
            Log.Open(name);
            Protocol.MyName = name;

            // task 4: derive shared key
            Crypto.Init(passphrase);

            // task 1 & 3: start sockets and receiver threads
            Net.Start(port);

            // task 2: announce presence and check if name is taken
            // wait up to 1 second for any name_taken response
            if (!Presence.Join()){
                Console.WriteLine($"Name '{name}' is already taken.");
                Net.Stop();
                return;
            }

            // interactive input loop
            while (running){
                string line = Console.ReadLine();
                if (line == null) break;

                line = line.Trim();
                if (line.Length == 0) continue;

                if (line.Equals("/quit", StringComparison.OrdinalIgnoreCase)){
                    running = false;
                    Presence.Leave();
                    break;
                }
                
                if (line.StartsWith("/me", StringComparison.OrdinalIgnoreCase)){
                    string rest = line.Substring(4).TrimStart();
                    int spaceIdx = rest.IndexOf(' ');
                    string text = rest.Substring(spaceIdx + 1).Trim();
                    Net.SendTo(new IPEndPoint(IPAddress.Any, port), Protocol.New(Protocol.Msg, line));
                    Console.WriteLine($"[private to {Protocol.MyName}] {text}");
                    continue;
                }

                if (line.Equals("/users", StringComparison.OrdinalIgnoreCase)){
                    Presence.PrintUsers();
                    continue;
                }

                // task 3: private messages
                if (line.StartsWith("/msg ", StringComparison.OrdinalIgnoreCase)){
                    string rest = line.Substring(5).TrimStart();
                    int spaceIdx = rest.IndexOf(' ');
                    if (spaceIdx <= 0){
                        Console.WriteLine("Usage: /msg <name> <text>");
                        continue;
                    }

                    string target = rest.Substring(0, spaceIdx).Trim();
                    string text = rest.Substring(spaceIdx + 1).Trim();

                    if (text.Length == 0){
                        Console.WriteLine("Usage: /msg <name> <text>");
                        continue;
                    }

                    if (text.Length > Protocol.MaxText){
                        Console.WriteLine($"Error: message exceeds maximum length of {Protocol.MaxText} characters");
                        continue;
                    }

                    if (!Presence.TryGetEndPoint(target, out IPEndPoint targetEndPoint)){
                        Console.WriteLine($"* {target} is not online");
                        continue;
                    }

                    // task 4: encrypt private message
                    Net.SendTo(targetEndPoint, Crypto.EncryptPm(target, text));
                    Console.WriteLine($"[private to {target}] {text}");
                    continue;
                }

                // task 1: general channel broadcast
                if (line.Length > Protocol.MaxText){
                    Console.WriteLine($"Error: message exceeds maximum length of {Protocol.MaxText} characters");
                    continue;
                }

                if (!Protocol.IsValidText(line)){
                    Console.WriteLine("Error: message contains invalid control characters");
                    continue;
                }

                Net.Broadcast(Protocol.New(Protocol.Msg, line));
                Console.WriteLine($"[general] {name}: {line}");
            }

            Net.Stop();
        }
    }
}
