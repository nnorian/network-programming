using System;
using System.Collections.Generic;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading;

// the program for client and provider
// Usage: CoffeeClient <address> <port> [client|provider]

namespace CoffeeShopClient{

    public class Program{

        private static Socket socket;
        private static readonly object sendLock  = new object();
        private static volatile bool quitting = false;

        public static void Main(string[] args){

            int port;

            if(args.Length < 2 || args.Length > 3 || !int.TryParse(args[1], out port) || port < 1 || port > 65535){

                Console.WriteLine("Usage: dotnet run --project CoffeeShopClient -- <address> <port> [client|provider]");
                return;
            }

            string role = "client";

            if (args.Length == 3){

                role = args[2].ToLower();

            }

            if (role != "client" && role != "provider"){

                Console.WriteLine("The role must be client or provider.");
                return;

            }

            IPAddress address = FindAddress(args[0]);

            if(address == null){
                Console.WriteLine("Unknown address: " + args[0]);
                return;
            }

            // task 1
            // connect to the server
            socket = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);

            try {

                socket.Connect(new IPEndPoint(address, port));
            }
            catch(SocketException ex){

                Console.WriteLine("Cannot connect to " + address + ":" + port + ": " + ex.SocketErrorCode);
                return;
            }

            Console.WriteLine("Connected to " + socket.RemoteEndPoint + " from " + socket.LocalEndPoint + " as " + role + ".");

            // task 4
            // separate thread reveives so server messages are shown at once

            Thread receiver = new Thread(ReceiveLoop);
            receiver.IsBackground = true;
            receiver.Start();

            // task 4
            // heartbeat so the server notices if client netwoek is gone

            Thread heartbeat = new Thread(HeartbeatLoop);
            heartbeat.IsBackground = true;
            heartbeat.Start();

            // task 3
            // provider registers and delivers
            if (role == "provider"){

                SendLine("HELLO PROVIDER");
                Console.WriteLine("Commands: DELIVER coffee=20 croissant=10 | MENU | quit");
            }

            // task 1 and 2
            // client commands
            else{

                Console.WriteLine("Commands: MENU | ORDER coffee=2 tea=1 | quit");
            }

            // the main thread reads the keyboard
            while(true){

                string line = Console.ReadLine();

                if (line == null || line.Trim().ToLower() == "quit") {
                    break;
                }

                if (line.Trim() == ""){
                    continue;

                }

                SendLine(line);
            }

            quitting = true;

            try{
                socket.Shutdown(SocketShutdown.Both);
            }

            catch (Exception){

            }

            socket.Close();
            Console.WriteLine("Me done here");

        }

        private static IPAddress FindAddress(string text){
            IPAddress address;
            if (IPAddress.TryParse(text, out address)){
                if (address.AddressFamily == AddressFamily.InterNetwork){
                    return address;
                }
                return null;
            }

            try{
                foreach (IPAddress candidate in Dns.GetHostAddresses(text)){
                    if (candidate.AddressFamily == AddressFamily.InterNetwork){
                        return candidate;
                    }
                }
            }
            catch (SocketException){
            }
            catch (ArgumentException){
            }
            return null;
        }

        // task 2
        // lock so sends dont get mixed between threads
        private static void SendLine(string line){
            byte[] data = Encoding.UTF8.GetBytes(line + "\n");

            lock (sendLock){

                try {

                    int sent = 0;
                    while (sent < data.Length){

                        sent += socket.Send(data, sent, data.Length - sent, SocketFlags.None);
                    }
                }

                catch (Exception ex) when (ex is SocketException || ex is ObjectDisposedException){

                    ServerGone("cannot send");

                }
            }
        }

        // task 1
        // receive loop, message ends with \n
        private static void ReceiveLoop(){
            byte[] buffer = new byte[1024];
            List<byte> message = new List<byte>();
            try{
                while (true){
                    int count = socket.Receive(buffer);

                    if (count == 0){

                        ServerGone("the server closed the connection");
                        return;

                    }

                    for (int i = 0; i < count; i++ ){

                        if (buffer[i] == (byte)'\n'){

                            Console.WriteLine("<< " + Encoding.UTF8.GetString(message.ToArray()));
                            message.Clear();
                        }
                        else{

                            message.Add(buffer[i]);
                        }
                    }
                }
            }

            catch (Exception ex) when (ex is SocketException || ex is ObjectDisposedException){

                ServerGone("the connection is broken");
            }

        }

        private static void HeartbeatLoop(){
            while (true){
                Thread.Sleep(3000);
                SendLine("PING");
            }
        }

        // task 4
        // end cleanly when the seveer dissapears

        private static void ServerGone(string reason){

            if (quitting){

                return;
            }

            Console.WriteLine("Disconnected: " + reason + ".");

            try {
                socket.Close();
            }

            catch (Exception){

            }

            Environment.Exit(0);
        }
    }
}
