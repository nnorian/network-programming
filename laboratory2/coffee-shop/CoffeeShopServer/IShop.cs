using System;
using System.Collections.Generic;

namespace CoffeeShopServer{

    // common interface that connects two halves of server 
    public interface IShop{
        // copy of the stock
        
        OrderResult PlaceOrder(string client, Dictionary<string, int> items, Action<string> notify);
        // adds product to stock
        void Deliver (Dictionary<string, int> items);
        // if client gone, cancel put back to stock
        void CancelClient(string client);
        // provider connects or disconnects 
        void SetProvider(Action<string> notify);
    }

}