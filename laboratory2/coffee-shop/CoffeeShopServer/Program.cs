using System;
using System.Net.Sockets;

namespace CoffeeShopServer{

    public class Program{

        public static voif Main(string[] args){

            int port;

            if (args.Length != 1 || !int.TryParse(args[0], out port) || port < 1 || port > 65535){

                Console.WriteLine("Usage: dotnet run --project CoffeeServer -- <port>");
                return;
            }

            // Until Student B's Shop is ready, test with the fake one.
            // Then change this line to: IShop shop = new Shop();
            IShop shop = new FakeShop();

            Server server = new Server (port, shop);

            try{

                server.Run();
            }
            catch (SocketException ex){

                if (ex.SocketErrorCode == SocketError.AddressAlreadyInUse){
                    Console.WriteLine("Port " + port + " is already in use. Stop the other program or choose another port.");
                else
                    Console.WriteLine("Cannot start the server: " + ex.SocketErrorCode);
                }
            }
        }
    }
}