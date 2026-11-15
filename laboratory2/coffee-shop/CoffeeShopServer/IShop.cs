using System;
using System.Collections.Generic;

namespace CoffeeShopServer{

    // common interface that connects two halves of server
    public interface IShop{

        // task 1
        // copy of the stock
        Dictionary<string, int> GetMenu();

        // task 2
        // checks stock and starts worker
        OrderResult PlaceOrder(string client, Dictionary<string, int> items, Action<string> notify);

        // task 3
        // adds product to stock
        void Deliver(Dictionary<string, int> items);

        // task 4
        // if client gone, cancel and put back to stock
        void CancelClient(string client);

        // task 3
        // provider connects or disconnects
        void SetProvider(Action<string> notify);
    }
}
