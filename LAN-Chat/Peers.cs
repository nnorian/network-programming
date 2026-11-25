// task 2
// keep track of who is online
// timeouts and listings

using System;
using System.Collections.Generic;
using System.Net;

namespace LanChat{

    public class PeerInfo{
        public string Name {get; set;}
        public string Id {get; set;}
        public IPEndPoint EndPoint {get; set;}
        public DateTime LastSeen {get; set;}
    }

    public static class Peers{

        static readonly object Lock = new object();
        static readonly Dictionary<string, PeerInfo> table = new Dictionary<string, PeerInfo>(StringComparer.OrdinalIgnoreCase);

        // returns true if newly added
        public static bool Update(string name, string id, IPEndPoint endpoint){
            lock (Lock){
                if (table.TryGetValue(name, out var existing)){
                    existing.Id = id;
                    existing.EndPoint = endpoint;
                    existing.LastSeen = DateTime.UtcNow;
                    return false;
                }
                else{
                    table[name] = new PeerInfo{
                        Name = name,
                        Id = id,
                        EndPoint = endpoint,
                        LastSeen = DateTime.UtcNow
                    };
                    return true;
                }
            }
        }

        public static bool Remove(string name){
            lock (Lock){
                return table.Remove(name);
            }
        }

        public static bool TryGet(string name, out PeerInfo info){
            lock (Lock){
                return table.TryGetValue(name, out info);
            }
        }

        public static List<PeerInfo> GetAll(){
            lock (Lock){
                return new List<PeerInfo>(table.Values);
            }
        }

        // drop peers not heard from for 15s
        public static List<string> CheckTimeouts(TimeSpan timeout){
            List<string> expired = new List<string>();
            DateTime now = DateTime.UtcNow;

            lock (Lock){
                foreach (var kvp in table){
                    if (now - kvp.Value.LastSeen > timeout){
                        expired.Add(kvp.Key);
                    }
                }
                foreach (var name in expired){
                    table.Remove(name);
                }
            }
            return expired;
        }
    }
}
