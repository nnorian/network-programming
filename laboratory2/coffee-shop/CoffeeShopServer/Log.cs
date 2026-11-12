using System;

namespace CoffeeShopServer{
    // time, who
    public static class Log{
        public static void Write(string who, string message){
            string time = DateTime.Now.ToString("HH:mm:ss");
            Console.WriteLine("[" + time + "] " + who.PadRight(21) + " " + message);
        }
    }
}