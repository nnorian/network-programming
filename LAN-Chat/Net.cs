using System.Net;

namespace LanChat{
    // STUDENT A: sockets. Placeholder with the agreed interface, replace the bodies.
    // Rule: send EVERY datagram from the private (unicast) socket, so the source
    // endpoint of any datagram is the endpoint where that peer gets private messages.
    // Rule: Broadcast and SendTo never throw; they encode, send, and call Log.Sent.
    public static class Net{
        public static void Broadcast(Message m){
        }

        public static void SendTo(IPEndPoint endpoint, Message m){
        }
    }
}
