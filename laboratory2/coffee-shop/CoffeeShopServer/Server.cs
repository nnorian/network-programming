// listening socket and accept loop

using System;
using System.Net;
using System.Net.Sockets;
using System.Threading;

namespace CoffeeShopServer{

    public class Server{

        private readonly int port;
        private readonly IShop shop;

        // task 3
        // which connedction is the provider
        
        private readonly object providerLock = new object();
        private ClientConnection provider = null;

        public Server(int port, IShop shop){

            this.port = port;
            this.shop = shop;

        }

        public void Run(){
            Socket listener = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);

            // ANY bonus
            // works with all network cards so another computer can cannect too

            listener.Bind(new IPEndPoint(IPAddress.Any, port));
            listener.Listen(100);
            Log.Write("server", "listening on port " + port);

            while (true){
                try{

                    Socket socket = listener.Accept();
                    // waits for next client

                    //task2
                    // one thread per client then back to accept at once
                    ClientConnection connection = new ClientConnection(socket, this, shop);
                    thread.IsBackground = true;
                    thread.Start();
                }

                catch (SocketException ex){

                    // for cases like when the client disconected before accept returned
                    Log.Write("server", "ERROR accept failed: " + ex.SocketErrorCode);
                }
            }
        }

        public bool TrySetProvider(ClientConnection connection){

            lock (providerLock){

                if (provider != null){
                    return false;
                }

                provider = connection;
                return true;
            }
        }

        public void RemoveProvider(ClientConnection connection){

            lock (providerLock){

                if (provider == connection){
                    provider = null;
                }
            }
        }
    }
}