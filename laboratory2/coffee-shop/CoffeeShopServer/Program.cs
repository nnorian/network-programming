using System;
using System.Net.Sockets;

namespace CoffeeShopServer{

    public class Program{

        public static void Main(string[] args){

            int port;

            if (args.Length != 1 || !int.TryParse(args[0], out port) || port < 1 || port > 65535){

                Console.WriteLine("Usage: dotnet run --project CoffeeShopServer -- <port>");
                return;
            }

            // task 1 and 2
            // the shop with stock and workers
            IShop shop = new Shop();

            // task 1
            // starts listening socket
            Server server = new Server(port, shop);

            try{

                server.Run();
            }
            catch (SocketException ex){

                if (ex.SocketErrorCode == SocketError.AddressAlreadyInUse){
                    Console.WriteLine("Port " + port + " is already in use. Stop the other program or choose another port.");
                }
                else{
                    Console.WriteLine("Cannot start the server: " + ex.SocketErrorCode);
                }
            }
        }
    }
}
