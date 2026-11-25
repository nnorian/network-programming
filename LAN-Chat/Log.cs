// logging for the chat
// one liners with 
// time, SENT/RECV/DROP, endpoint, kind, size

using System;
using System.IO;
using System.Net;

namespace LanChat{
    public static class Log{

        static readonly object Lock = new object();
        static string path;

        public static void Open(string name){
            // process id keeps things separated when several peers run
            path = "chat-" + name + "-" + Environment.ProcessId + ".log";
        }

        public static void Send(EndPoint to, string kind, int size){
            Write("SENT", to, kind, size);
        }

        public static void Received(EndPoint from, string kind, int size){
            Write("RECV", from, kind, size);
        }

        public static void Dropped(EndPoint from, string kind, int size){
            Write("DROP", from, kind, size);
        }

        static void Write(string action, EndPoint endpoint, string kind, int size){
            string line = $"{DateTime.Now:HH:mm:ss.fff} {action} {endpoint} {kind} {size} bytes";

            // the receive thread the heartbeat timer and the console write here
            lock (Lock){
                File.AppendAllText(path, line + Environment.NewLine);
            }
        }
    }
}
