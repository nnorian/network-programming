// task 1: general channel over udp broadcast with shared port
// task 3: private messages over unicast ephemeral port

using System;
using System.Net;
using System.Net.Sockets;
using System.Threading;

namespace LanChat{

    public static class Net{

        static Socket broadcastSocket;
        static Socket privateSocket;
        static int chatPort;
        static volatile bool running;
        static Thread broadcastThread;
        static Thread privateThread;

        static readonly ManualResetEvent nameTakenSignal = new ManualResetEvent(false);
        public static volatile bool NameTakenReceived = false;

        public static void Start(int port){
            chatPort = port;
            running = true;
            NameTakenReceived = false;
            nameTakenSignal.Reset();

            // task 1: bind general broadcast socket on shared port
            broadcastSocket = new Socket(AddressFamily.InterNetwork, SocketType.Dgram, ProtocolType.Udp);
            broadcastSocket.SetSocketOption(SocketOptionLevel.Socket, SocketOptionName.ReuseAddress, true);
            broadcastSocket.EnableBroadcast = true;
            broadcastSocket.Bind(new IPEndPoint(IPAddress.Any, port));

            // task 3: bind unicast private socket on ephemeral port (0)
            // all datagrams are sent from this socket so receiver learns our unicast port
            privateSocket = new Socket(AddressFamily.InterNetwork, SocketType.Dgram, ProtocolType.Udp);
            privateSocket.EnableBroadcast = true;
            privateSocket.Bind(new IPEndPoint(IPAddress.Any, 0));

            broadcastThread = new Thread(() => ReceiveLoop(broadcastSocket, isBroadcast: true)){
                IsBackground = true,
                Name = "NetBroadcastRecv"
            };
            broadcastThread.Start();

            privateThread = new Thread(() => ReceiveLoop(privateSocket, isBroadcast: false)){
                IsBackground = true,
                Name = "NetPrivateRecv"
            };
            privateThread.Start();
        }

        public static void Stop(){
            running = false;
            try{ broadcastSocket?.Close(); } catch { }
            try{ privateSocket?.Close(); } catch { }
        }

        public static bool WaitForNameConflict(int timeoutMs){
            return nameTakenSignal.WaitOne(timeoutMs) && NameTakenReceived;
        }

        // task 1: broadcast to general channel
        public static void Broadcast(Message m){
            if (privateSocket == null) return;

            byte[] data = Protocol.Encode(m);
            IPEndPoint target = new IPEndPoint(IPAddress.Broadcast, chatPort);

            try{
                privateSocket.SendTo(data, target);
                Log.Send(target, m.Type, data.Length);
            }
            catch (SocketException ex) when (ex.SocketErrorCode == SocketError.NetworkUnreachable){
                // fallback for environments without a default route or running on loopback
                IPEndPoint fallback = new IPEndPoint(IPAddress.Parse("127.255.255.255"), chatPort);
                try{
                    privateSocket.SendTo(data, fallback);
                    Log.Send(fallback, m.Type, data.Length);
                }
                catch (Exception e){
                    Log.Dropped(fallback, "send error: " + e.Message, data.Length);
                }
            }
            catch (Exception ex){
                Log.Dropped(target, "send error: " + ex.Message, data.Length);
            }
        }

        // task 3: unicast targeted send directly to peer endpoint
        public static void SendTo(IPEndPoint endpoint, Message m){
            if (privateSocket == null || endpoint == null) return;

            byte[] data = Protocol.Encode(m);
            try{
                privateSocket.SendTo(data, endpoint);
                Log.Send(endpoint, m.Type, data.Length);
            }
            catch (Exception ex){
                Log.Dropped(endpoint, "send error: " + ex.Message, data.Length);
            }
        }

        static void ReceiveLoop(Socket sock, bool isBroadcast){
            byte[] buffer = new byte[Protocol.MaxDatagram + 512];

            while (running){
                EndPoint remote = new IPEndPoint(IPAddress.Any, 0);
                int bytesRead;

                try{
                    bytesRead = sock.ReceiveFrom(buffer, ref remote);
                }
                catch (SocketException){
                    if (!running) break;
                    continue;
                }
                catch (ObjectDisposedException){
                    break;
                }

                IPEndPoint fromEp = (IPEndPoint)remote;

                if (!Protocol.TryDecode(buffer, bytesRead, out Message m, out string error)){
                    Log.Dropped(fromEp, error ?? "malformed", bytesRead);
                    continue;
                }

                Log.Received(fromEp, m.Type, bytesRead);

                // task 1: echo suppression for own messages
                if (m.Id == Protocol.MyId){
                    continue;
                }

                HandleMessage(m, fromEp);
            }
        }

        static void HandleMessage(Message m, IPEndPoint fromEp){
            switch (m.Type){
                // task 2: presence join
                case Protocol.Join:
                    if (string.Equals(m.From, Protocol.MyName, StringComparison.OrdinalIgnoreCase)){
                        // someone trying to join with our name
                        SendTo(fromEp, Protocol.New(Protocol.NameTaken));
                    }
                    else{
                        // reply unicast with our presence
                        SendTo(fromEp, Protocol.New(Protocol.JoinReply));
                        bool isNew = Peers.Update(m.From, m.Id, fromEp);
                        if (isNew){
                            Console.WriteLine($"* {m.From} joined the chat");
                        }
                    }
                    break;

                // task 2: presence join_reply
                case Protocol.JoinReply:
                    Peers.Update(m.From, m.Id, fromEp);
                    break;

                // task 2: name conflict detected
                case Protocol.NameTaken:
                    NameTakenReceived = true;
                    nameTakenSignal.Set();
                    break;

                // task 2: liveness heartbeat
                case Protocol.Heartbeat:
                    if (!string.Equals(m.From, Protocol.MyName, StringComparison.OrdinalIgnoreCase)){
                        Peers.Update(m.From, m.Id, fromEp);
                    }
                    break;

                // task 2: graceful leave
                case Protocol.Leave:
                    if (Peers.Remove(m.From)){
                        Console.WriteLine($"* {m.From} left the chat");
                    }
                    break;

                // task 1: general channel message
                case Protocol.Msg:
                    if (!string.Equals(m.From, Protocol.MyName, StringComparison.OrdinalIgnoreCase)){
                        Peers.Update(m.From, m.Id, fromEp);
                        Console.WriteLine($"[general] {m.From}: {m.Text}");
                    }
                    break;

                // task 3 & 4: secure private message
                case Protocol.Pm:
                    Peers.Update(m.From, m.Id, fromEp);
                    if (Crypto.Decrypt(m.From, m.Nonce, m.Ciphertext, out string plainText)){
                        Console.WriteLine($"[private] {m.From}: {plainText}");
                    }
                    else{
                        Log.Dropped(fromEp, "decrypt failure", 0);
                    }
                    break;
            }
        }
    }
}
