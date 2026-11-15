using System;
using System.Collections.Generic;
using System.IO;
using System.Threading;

namespace CoffeeShopServer{

    // shop with stock, order queue, workers, alerts and persistence
    public class Shop : IShop{

        // order in the queue or being prepared
        private class Order{
            public int Number;
            public string Client;
            public Dictionary<string, int> Items;
            public int PreparationSeconds;
            public Action<string> Notify;
            public ManualResetEvent CancelEvent = new ManualResetEvent(false);
            public bool IsCancelled = false;
        }

        // task 1 and 2
        // lock for stock, queue and order counter
        private readonly object shopLock = new object();
        private readonly Dictionary<string, int> stock = new Dictionary<string, int>();
        private readonly List<Order> waitingOrders = new List<Order>();
        private readonly List<Order> activeOrders = new List<Order>();
        private readonly List<string> pendingAlerts = new List<string>();

        private Action<string> providerNotify = null;
        private int nextOrderNumber = 1;
        private const string StockFileName = "stock.txt";

        // task 1 and 2
        // load stock and start workers
        public Shop(){

            // bonus 2
            // load stock from file or random if no file
            LoadStock();

            // task 2
            // one worker thread per CPU core
            int workerCount = Environment.ProcessorCount;
            for (int i = 0; i < workerCount; i++){
                int workerId = i + 1;
                Thread worker = new Thread(() => WorkerLoop(workerId));
                worker.IsBackground = true;
                worker.Start();
            }
            Log.Write("shop", "started " + workerCount + " workers (1 per CPU core)");
        }

        // task 1
        // return a copy of the stock under lock
        public Dictionary<string, int> GetMenu(){
            lock (shopLock){
                return new Dictionary<string, int>(stock);
            }
        }

        // task 2
        // check stock and subtract under lock, queue order
        public OrderResult PlaceOrder(string client, Dictionary<string, int> items, Action<string> notify){
            List<string> alertsToSend = null;
            int orderNumber;
            int totalSeconds = 0;

            lock (shopLock){

                // check every product first, refuse if not enough
                foreach (KeyValuePair<string, int> item in items){
                    string product = item.Key;
                    int quantity = item.Value;

                    if (!stock.ContainsKey(product) || stock[product] < quantity){
                        int left = stock.ContainsKey(product) ? stock[product] : 0;
                        return new OrderResult{
                            Accepted = false,
                            MissingProduct = product,
                            Left = left
                        };
                    }
                }

                // subtract all at once
                foreach (KeyValuePair<string, int> item in items){
                    stock[item.Key] -= item.Value;
                    totalSeconds += Products.PreparationSeconds(item.Key) * item.Value;

                    // task 3
                    // alert if stock drops below 5
                    if (stock[item.Key] < 5){
                        string alert = "ALERT " + item.Key + " " + stock[item.Key];
                        if (providerNotify != null){
                            if (alertsToSend == null){
                                alertsToSend = new List<string>();
                            }
                            alertsToSend.Add(alert);
                        }
                        else{
                            pendingAlerts.RemoveAll(a => a.StartsWith("ALERT " + item.Key + " "));
                            pendingAlerts.Add(alert);
                        }
                    }
                }

                orderNumber = nextOrderNumber++;
                SaveStockToFile();

                // add order to queue and wake worker
                Order order = new Order{
                    Number = orderNumber,
                    Client = client,
                    Items = new Dictionary<string, int>(items),
                    PreparationSeconds = totalSeconds,
                    Notify = notify
                };

                waitingOrders.Add(order);
                Monitor.Pulse(shopLock);
            }

            // task 3
            // send alerts outside the lock
            if (alertsToSend != null){
                Action<string> sendAlert = providerNotify;
                if (sendAlert != null){
                    foreach (string alert in alertsToSend){
                        try{
                            sendAlert(alert);
                            Log.Write("shop", "sent to provider: " + alert);
                        }
                        catch (Exception ex){
                            Log.Write("shop", "ERROR sending alert: " + ex.Message);
                        }
                    }
                }
            }

            return new OrderResult{
                Accepted = true,
                OrderNumber = orderNumber,
                PreparationSeconds = totalSeconds
            };
        }

        // task 2
        // worker thread takes oldest order from queue
        private void WorkerLoop(int workerId){
            while (true){
                Order order = null;

                lock (shopLock){
                    while (waitingOrders.Count == 0){
                        Monitor.Wait(shopLock);
                    }

                    order = waitingOrders[0];
                    waitingOrders.RemoveAt(0);
                    activeOrders.Add(order);
                }

                Log.Write("worker-" + workerId, "preparing order " + order.Number + " (" + Protocol.FormatItems(order.Items) + "), " + order.PreparationSeconds + " s");

                // task 4
                // interruptible wait so cancel can stop it
                bool signaled = order.CancelEvent.WaitOne(order.PreparationSeconds * 1000);

                bool wasCancelled = false;
                lock (shopLock){
                    activeOrders.Remove(order);

                    if (signaled || order.IsCancelled){
                        wasCancelled = true;
                        foreach (KeyValuePair<string, int> item in order.Items){
                            stock[item.Key] += item.Value;
                            if (stock[item.Key] >= 5){
                                pendingAlerts.RemoveAll(a => a.StartsWith("ALERT " + item.Key + " "));
                            }
                        }
                        SaveStockToFile();
                        Log.Write(order.Client, "order " + order.Number + " CANCELLED while preparing, products returned to stock");
                    }
                    else{
                        Log.Write(order.Client, "order " + order.Number + " ready");
                    }
                }

                // send ready notify outside lock
                if (!wasCancelled){
                    try{
                        order.Notify("READY " + order.Number);
                    }
                    catch (Exception ex){
                        Log.Write(order.Client, "ERROR sending READY " + order.Number + ": " + ex.Message);
                    }
                }
            }
        }

        // task 3
        // provider delivers products to stock
        public void Deliver(Dictionary<string, int> items){
            lock (shopLock){
                foreach (KeyValuePair<string, int> item in items){
                    if (stock.ContainsKey(item.Key)){
                        stock[item.Key] += item.Value;
                    }
                    else{
                        stock[item.Key] = item.Value;
                    }

                    if (stock[item.Key] >= 5){
                        pendingAlerts.RemoveAll(a => a.StartsWith("ALERT " + item.Key + " "));
                    }
                }

                SaveStockToFile();
            }
        }

        // task 3
        // provider connects or disconnects, send queued alerts
        public void SetProvider(Action<string> notify){
            List<string> alertsToSend = null;

            lock (shopLock){
                providerNotify = notify;

                if (providerNotify != null && pendingAlerts.Count > 0){
                    alertsToSend = new List<string>(pendingAlerts);
                    pendingAlerts.Clear();
                }
            }

            if (alertsToSend != null && notify != null){
                foreach (string alert in alertsToSend){
                    try{
                        notify(alert);
                        Log.Write("shop", "sent pending alert to provider: " + alert);
                    }
                    catch (Exception ex){
                        Log.Write("shop", "ERROR sending pending alert: " + ex.Message);
                    }
                }
            }
        }

        // task 4
        // client disconnected: cancel waiting and active orders
        public void CancelClient(string client){
            lock (shopLock){
                // cancel waiting orders and return products to stock
                for (int i = waitingOrders.Count - 1; i >= 0; i--){
                    Order order = waitingOrders[i];
                    if (order.Client == client){
                        waitingOrders.RemoveAt(i);
                        foreach (KeyValuePair<string, int> item in order.Items){
                            stock[item.Key] += item.Value;
                            if (stock[item.Key] >= 5){
                                pendingAlerts.RemoveAll(a => a.StartsWith("ALERT " + item.Key + " "));
                            }
                        }
                        Log.Write(client, "order " + order.Number + " CANCELLED in queue, " + Protocol.FormatItems(order.Items) + " returned to stock");
                    }
                }

                // signal active worker for this client
                foreach (Order order in activeOrders){
                    if (order.Client == client && !order.IsCancelled){
                        order.IsCancelled = true;
                        order.CancelEvent.Set();
                    }
                }

                SaveStockToFile();
            }
        }

        // bonus 2
        // load stock from file or initialize random
        private void LoadStock(){
            foreach (string name in Products.Names){
                stock[name] = 0;
            }

            if (File.Exists(StockFileName)){
                try{
                    string[] lines = File.ReadAllLines(StockFileName);
                    foreach (string line in lines){
                        string[] parts = line.Split('=');
                        if (parts.Length == 2){
                            string product = parts[0].Trim().ToLower();
                            int qty;
                            if (stock.ContainsKey(product) && int.TryParse(parts[1].Trim(), out qty)){
                                stock[product] = qty;
                            }
                        }
                    }

                    Log.Write("shop", "loaded stock from " + StockFileName + ": " + Protocol.FormatItems(stock));
                    return;
                }
                catch (Exception ex){
                    Log.Write("shop", "warning: failed to read " + StockFileName + ": " + ex.Message);
                }
            }

            // task 1
            // random stock 10 to 50
            Random random = new Random();
            foreach (string name in Products.Names){
                stock[name] = random.Next(10, 51);
            }

            SaveStockToFile();
            Log.Write("shop", "initialized random stock: " + Protocol.FormatItems(stock));
        }

        // bonus 2
        // save stock to file after every change
        private void SaveStockToFile(){
            try{
                List<string> lines = new List<string>();
                foreach (KeyValuePair<string, int> entry in stock){
                    lines.Add(entry.Key + "=" + entry.Value);
                }
                File.WriteAllLines(StockFileName, lines);
            }
            catch (Exception ex){
                Log.Write("shop", "warning: failed to save stock: " + ex.Message);
            }
        }
    }
}
