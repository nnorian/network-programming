using System;
using System.Collections.Generic;
using System.Net.Sockets;
using System.Text;

namespace CoffeeShopServer{
    // for clients and providers
    // every connections runs Run() on own thread

    public class ClientConnection{

        private const int MaxMessageLength = 1024;
        // bytes without /n

        private const int ReceiveTimeoutMs = 10000;
        // clients send ping each 3 sec

        private const int SendTimeoutMs = 5000;
        //client that does not read

        private readonly Socket socket;
        private readonly Server server;
        private readonly IShop shop;

        // task 2
        // the client thread nd the workers send on this socket
        private readonly object sendLock = new object();
        private bool closed = false;

        // the endpoint
        public string Name;

        // task 3
        public bool IsProvider = false;

        public ClientConnection(Socket socket, Server server, IShop shop){

            this.socket = socket;
            this.server = server;
            this.shop = shop;
            Name = socket.RemoteEndPoint.ToString();

        }

        public void Run(){
            Log.Write(Name, "connected");
            socket.ReceiveTimeout = ReceiveTimeoutMs;
            socket.SendTimeout = SendTimeoutMs;

            try{

                ReceiveLoop();
                Log.Write(Name, "closed the connection");
                // receive the returned 0
            }

            catch (SocketException ex) when (ex.SocketErrorCode == SocketError.TimedOut){
                Log.Write(Name, "silent for " + ReceiveTimeoutMs / 1000 + " s, considered gone");
            }

            catch (SocketException ex){
                Log.Write(Name, "connection broken: " + ex.SocketErrorCode);
            }

            catch (ObjectDisposedException){
                Log.Write(Name, "socket closed after a failed send");
            }

            catch (Exception ex){
                Log.Write(Name, "ERROR unexpected: " + ex.Message);
            }

            finally{
                CleanUp();
            }
        }

        // Task 1
        // cut the stream of bytes into messages that end in /n

        private void ReceiveLoop(){

            byte[] buffer = new byte[1024];
            byte[] message = new byte[MaxMessageLength];

            // bytes of the current message so far
            int length = 0;

            //after a long message you ignore bytes until its /n
            bool skipping = false;

            while (true){

                int count = socket.Receive(buffer);

                if (count == 0){
                    return;
                }

                for (int i = 0; i< count; i++){

                    byte b = buffer[i];

                    if (b == (byte)'\n'){
                        if(!skipping){

                            HandleMessage(Encoding.UTF8.GetString(message, 0, length));
                        }
                        length = 0;
                        skipping = false;
                    }

                    else if (skipping){
                        // land here and get ignored

                    }

                    else if (length == MaxMessageLength){
                        SendError("message too long, the maximum is " + MaxMessageLength + " bytes");
                        skipping = true;
                    }

                    else {
                        message[length] = b;
                        length++;
                    }

                }
            }
        }

        private void HandleMessage(string text){

            // telnet and Windows send "\r\n"
            text = text.TrimEnd('\r');

            if (!Protocol.IsPrintable(text)){

                SendError("the message contains invalid characters");
                return;
            }

            string[] parts = text.Split(' ', StringSplitOptions.RemoveEmptyEntries);

            if (parts.Length == 0){

                SendError("empty message");
                return;
            }

            switch (parts[0].ToUpper()){

                //task 4, check if alive with the answer
                case "PING":
                    break;
                case "MENU":
                    HandleMenu(parts);
                    break;
                case "ORDER":
                    HandleOrder(parts);
                    break;
                case "HELLO":
                    HandleHello(parts);
                    break;
                case "DELIVER":
                    HandleDeliver(parts);
                    break;
                default:
                    SendError("unknown command '" + parts[0] + "', use MENU, ORDER, HELLO PROVIDER or DELIVER");
                    break;

            }
        }

        // task 1
        // returns menu to client
        private void HandleMenu(string[] parts){

            if (parts.Length != 1){

                SendError("MENU has no parameters");
                return;
            }

            SendLine("MENU " + Protocol.FormatItems(shop.GetMenu()));
        }

        // task 2
        // client places an order
        private void HandleOrder(string[] parts){

            Dictionary<string, int> items;
            string error;

            if(!Protocol.TryParseItems(parts, out items, out error)){

                SendError(error);
                return;
            }

            OrderResult result = shop.PlaceOrder(Name, items, SendLine);

            if(result.Accepted){

                //answer at once
                // the worder sends the readiness notify later
                SendLine("OK ORDER " + result.OrderNumber + " TIME " + result.PreparationSeconds);
                Log.Write(Name, "order " + result.OrderNumber + " accepted: " + Protocol.FormatItems(items) + ", " + result.PreparationSeconds + " s");

            }

            else {

            SendLine("REFUSED " + result.MissingProduct + " LEFT " + result.Left);
            Log.Write(Name, "order refused: " + Protocol.FormatItems(items) + ", only " + result.Left + " " + result.MissingProduct + " left");

            }

        }

        // task 3
        // "HELLO PROVIDER" turns this connection into the provider

        private void HandleHello(string[] parts){
            if (parts.Length != 2 || parts[1].ToUpper() != "PROVIDER"){

                SendError("use: HELLO PROVIDER");
                return;
            }

            if (IsProvider){
                SendError("you are already the provider");
                return;
            }

            if (!server.TrySetProvider(this)){

                SendError("another provider is already connected");
                return;
            }

            IsProvider = true;
            SendLine("OK PROVIDER");
            Log.Write(Name, "is the provider");
            // the shop sends the waiting alerts
            shop.SetProvider(SendLine);
        }

        // task 3
        // only the provider can   deliver

        private void HandleDeliver(string[] parts){

            if (!IsProvider){

                SendError("only the provider can deliver");
                return;

            }

            Dictionary<string, int> items;
            string error;
            if (!Protocol.TryParseItems(parts, out items, out error)){

                SendError(error);
                return;
            }

            shop.Deliver(items);
            SendLine("OK DELIVERED " + Protocol.FormatItems(items));
            Log.Write(Name, "delivery: " + Protocol.FormatItems(items));
        }

        private void SendError(string reason){
            SendLine("ERROR " + reason);
            Log.Write(Name, "ERROR " + reason);
        }

        // Task 2
        // the lock that doesnt mix the connection of client and workers

        public void SendLine(string line){
            byte[] data = Encoding.UTF8.GetBytes(line + "\n");

            lock (sendLock){
                if (closed){
                    return;
                }
                try{
                    //  send may send only part of the data: send the rest until all is gone
                    int sent = 0;
                    while (sent < data.Length){

                        sent += socket.Send(data, sent, data.Length - sent, SocketFlags.None);
                    }
                }

                catch (Exception ex) when (ex is SocketException || ex is ObjectDisposedException){
                    // the client is gone or does not read - we close the socket
                    // its thread in receiver and that thread cleans up

                    closed = true;
                    try{
                        socket.Shutdown(SocketShutdown.Both);
                    }

                    catch (Exception){

                    }

                    socket.Close();
                }
            }
        }

        // Task 4
        // run once on this client thread when the client is gone
        private void CleanUp(){
            lock (sendLock){
                closed = true;
            }

            if (IsProvider){
                server.RemoveProvider(this);
                shop.SetProvider(null);
                Log.Write(Name, "the proviider left, the shop goes on with its stock");

            }

            shop.CancelClient(Name);
            socket.Close();
            Log.Write(Name, "disconnected");
        }

    }
}
