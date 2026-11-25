// console ui, command parser and lifecycle management

using System;
using System.Collections.Generic;
using System.Threading;

namespace LanChat{

    public static class Program{

        static volatile bool running = true;
        static Timer heartbeatTimer;
        static Timer timeoutTimer;

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
            Net.Broadcast(Protocol.New(Protocol.Join));

            // wait up to 1 second for any name_taken response
            if (Net.WaitForNameConflict(1000)){
                Console.WriteLine($"Name '{name}' is already taken.");
                Net.Stop();
                return;
            }

            // task 2: heartbeat every 5 seconds
            heartbeatTimer = new Timer(_ => {
                if (running){
                    Net.Broadcast(Protocol.New(Protocol.Heartbeat));
                }
            }, null, 5000, 5000);

            // task 2: check timeouts every 1 second (15s inactivity = gone)
            timeoutTimer = new Timer(_ => {
                if (running){
                    List<string> expired = Peers.CheckTimeouts(TimeSpan.FromSeconds(15));
                    foreach (string exp in expired){
                        Console.WriteLine($"* {exp} left the chat");
                    }
                }
            }, null, 1000, 1000);

            // interactive input loop
            while (running){
                string line = Console.ReadLine();
                if (line == null) break;

                line = line.Trim();
                if (line.Length == 0) continue;

                if (line.Equals("/quit", StringComparison.OrdinalIgnoreCase)){
                    running = false;
                    Net.Broadcast(Protocol.New(Protocol.Leave));
                    break;
                }

                if (line.Equals("/users", StringComparison.OrdinalIgnoreCase)){
                    var list = Peers.GetAll();
                    if (list.Count == 0){
                        Console.WriteLine("* No other users online");
                    }
                    else{
                        Console.WriteLine($"* Online users ({list.Count}):");
                        foreach (var peer in list){
                            Console.WriteLine($"  - {peer.Name} ({peer.EndPoint})");
                        }
                    }
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

                    if (!Peers.TryGet(target, out var targetPeer)){
                        Console.WriteLine($"* {target} is not online");
                        continue;
                    }

                    // task 4: encrypt private message
                    if (Crypto.Encrypt(target, text, out string nonce, out string cipher)){
                        var pm = Protocol.New(Protocol.Pm);
                        pm.Nonce = nonce;
                        pm.Ciphertext = cipher;
                        Net.SendTo(targetPeer.EndPoint, pm);
                        Console.WriteLine($"[private to {target}] {text}");
                    }
                    else{
                        Console.WriteLine("Error: encryption failed");
                    }
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

            heartbeatTimer?.Dispose();
            timeoutTimer?.Dispose();
            Net.Stop();
        }
    }
}
