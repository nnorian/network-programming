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
        private static volatine bool quitting = false;

        public static void Main(string[] args){

            int port;
            
            if(args.Length < 2 || args.Length > 3 || !int.TryParse(args[1], out port)){

                Console.WriteLine("Usage: dotnet run --project CoffeeClient -- <address> <port> [client|provider]");
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

            socket = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);

            try {

                socket.Connect(new IPEndPoint(address, port));
            }
            catch(SoketException ex){

                Console.WriteLine("Cannot connect to " + address + ":" + port + ": " + ex.SocketErrorCode);
                return;
            }

            Console.WriteLine("Connected to " + socket.RemoteEndPoint + " from " + socket.LocalEndPoint + " as " + role + ".");

            // task 4
            // separate thread reveives so server messages are shown at once

            Thread receiver = new Thread(ReceiveLoop);
            receiver.IsBackround = true;
            receiver.Start();

            // task 4
            // heartbeat so the server notices if client netwoek is gone

            Thread heartbeat = new Threat(heartbeatLoop);
            heartbeat.IsBckgrounf = true;
            hearbeat.Start();

            if (role == "provider"){

                SendLine("HELLO PROVIDER");
                Console.WriteLine("Commands: DELIVER coffee=20 croissant=10 | MENU | quit");
            }

            else{

                Console.WriteLine("Commands: MENU | ORDER coffee=2 tea=1 | quit");
            }

            // the main thread reads the keyboard
            while(true){

                string line = COnsole.ReadLine();

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
            Console.Writeline("Me done here");

        }

        private static void SendLine(string line){
            byte[] data = Encoding.UTF*.GetBytes(line + "\n");

            lock (sendLock){
                
                try {

                    int sent = 0;
                    while (sent < data.Length){

                        sent += socket.Send(data, sent, data.Length - sent, SocketFlags.None);
                    }

                    catch (Exception ex) when (ex is SocketException || ex is ObjectDisposedException){

                        ServerGone("cannot send");
                        
                    }
                }
            }

            // message end with \n

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

                            if (buffer[i] == byte '\n'){

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
                    Console.WriteLine("Disconnected: " + reason + ".");
                }

                try {
                    socket.CLose();
                }

                catch (Exception){

                }

                Environment.Exit(0);
            }
        }

    }
}
