// test file


using System;
using System.Collections.Generic;
using System.Threading;

namespace CoffeeServer
{
    // ONLY FOR TESTING the network part before Student B's Shop is ready.
    // No real stock, no workers. Delete this file when Shop works.
    public class FakeShop : IShop
    {
        private readonly object fakeLock = new object();
        private int nextOrderNumber = 1;

        public Dictionary<string, int> GetMenu()
        {
            Dictionary<string, int> menu = new Dictionary<string, int>();
            menu["coffee"] = 99;
            menu["tea"] = 99;
            menu["croissant"] = 99;
            return menu;
        }

        public OrderResult PlaceOrder(string client, Dictionary<string, int> items, Action<string> notify)
        {
            // More than 99 of something: refuse, so the REFUSED answer can be tested.
            foreach (KeyValuePair<string, int> item in items)
            {
                if (item.Value > 99)
                    return new OrderResult { Accepted = false, MissingProduct = item.Key, Left = 99 };
            }

            int number;
            lock (fakeLock)
            {
                number = nextOrderNumber;
                nextOrderNumber++;
            }

            int seconds = 0;
            foreach (KeyValuePair<string, int> item in items)
                seconds += Products.PreparationSeconds(item.Key) * item.Value;

            // Pretend to prepare it on another thread, then send READY.
            Thread thread = new Thread(() =>
            {
                Thread.Sleep(seconds * 1000);
                notify("READY " + number);
            });
            thread.IsBackground = true;
            thread.Start();

            return new OrderResult { Accepted = true, OrderNumber = number, PreparationSeconds = seconds };
        }

        public void Deliver(Dictionary<string, int> items)
        {
        }

        public void CancelClient(string client)
        {
            Log.Write(client, "(fake shop) the orders would be cancelled here");
        }

        public void SetProvider(Action<string> notify)
        {
            if (notify != null)
                notify("ALERT tea 3");   // a fake alert, to test that the provider receives it
        }
    }
}
