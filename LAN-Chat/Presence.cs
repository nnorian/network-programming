using System;
using System.Collections.Generic;
using System.Net;
using System.Threading;

// task 2
// who is online check
// without server 
namespace LanChat{

    public static class Presence{
        public const int HeartbeatSeconds = 5;
        public const int TimeoutSeconds = 15;
        const int JoinWaitMs =  1000;

        class Peer{

            public string Id;
            public IPEndPoint EndPoint;
            public DateTime LastSeen;
        }

        // name given to peeer
        // neede to receive thread
        // the timer and console
        // always locked

        static Dictionary<string, Peer> peers = new Dictionary<string, Peer>();
        static Timer timer;
        static volatile bool nameTaken;

        // tells everybody that it had joined
        // wats for answers
        // false when the declared name is already in use

        public static Join(){
            Net.Broadcast(Protocol.New(Protocol.Join));
            Thread.Sleep(JoinWaitMs);
            if(nameTaken) return false;


            timer = new Timer(Tick, null, HeartbeatSeconds * 1000, HeartbeatSeconds * 1000);
            PrintUsers();
            return true;
        }

        public static void Leave(){
            timer?.Dispose();
            Net.Broadcast(Protocol.New(Protocol.Leave));
        }

        // called by the receive loop for 
        // join
        // join_reply
        // name_taken
        // heatbeat
        // leave

        public static void Handle(Message m, IPEndPoint from){

            if (m.Id == Protocol.MyId) return;

            lock(peers){
                Peer know;
                peer.TryGetValue(m.From, out know);

                // same name
                //another runninh copy

                bool otherOwner = m.From == Protocol.MyName || (know != null && knowId != m.Id);

                switch (m.Type){

                    case Protocol.Join:
                    if (otherOwner){
                        Net.SendTo(from, Protocol.New(Protocol.JoinReply));
                        return;

                    }

                    Seen(m, from);
                    Net.SendTo(from, Protocol.New(Protocol.JoinReply));
                    break;
                
                case Protocol.JoinReply:
                case Protocol.Heartbeat:
                    if (!otherOwner) Seen(m, from);
                    break;
                case Protocol.Leave:
                    if (know != null && know.Id == m.Id) Remove(m.From);
                    break;

                case Protocol.NameTaken:
                    nameTaken = true;
                    break;
                }

            }
        }

        public static bool TryGetPoint(string name, out IPEndPoint endpoint){
            lock (peers){
                Console.WriteLine(" online: " + Protocol.MyName + "(you)");
                foeach (KeyValuePair<string, Peer> pair in peers){
                    Console.WriteLine(" ", + pair.Key + " " + pair.Value.EndPoint);
                }
            }
        }

        // add a new peer 
        // or refreshes one we know

        static void Seen(Message m, IPEndPoint from){
            Peer p;
            if(!peers.TryGetValue(m.From, out p)){
                p = newPeer{Id = m.Td};
                peers[m.From] = p;

                // a join_reply is an old memeber answering
                // not new

                if (m.Type != Protocol.JoinReply) Console.WriteLine(" " + m.From + " joined the chat");

            }

            p.EndPoint = from;
            p.LastSeen = DateTime.UtcNow;
        }

        static void remove(string name){
            peers.Remove(name);
            Console.WriteLine(" " + name + " left the chat");


        }

        // every 5 seconds
        // say i am still here
        // forget if someone was silent for mere than 15 sec
        static void Tick(object state){

            Net.Broadcast(Protocol.New(Protocol.Heartbeat));
            lock (peers){
                foreach (string name in new List<string>(peers.Keys)){
                    if((DateTime.UtcNow - peers[name].LastSeen).TotalSeconds > TimeoutSeconds){
                        Remove(name);
                    }
                }
            }
        }

    }
    
}