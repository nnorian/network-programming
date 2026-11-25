using System;
using System.Collections.Generic;
using System.Security.Cryptography;
using System.Text.Json;
using System.Text.Json.Serialization;

// the possible formats of the datagrams:
// encode
// decode 
// chech em
namespace LanChat{

    public static class Protocol{

        public const string Msg = "msg";
        public const string Pm = "pm";
        public const string Join = "join";
        public const string JoinReply = "join_reply";
        public const string NameTaken = "name_taken";
        public const string Heartbeat = "heartbeat";
        public const string Leave = "leave";

        static readonly HashSet<string> Types = new HashSet<string>{
            Msg, Pm, Join, Joinreply, NameTaken, Heartbeat, Leave
        };

        public const int MaxTeext = 500;
        public const int MaxName = 20;
        public const int MaxDatagram = 4096;

        // the name unique per chat
        // id unique per running copy

        public static string MyName;
        public static readonly string MyId = Convert.ToHexString(RandomNumberGenerator.GetBytes(8));

        static readonly JsonSerializerOptions Option = new JsonSerializerOptions{
            PropertyNamingPolicy = JsonNamingPolicy.CamelCase,
            DefaultIgnoreConditions = JsonIgnoreCondition.WhenWritingnull
        };

        public static Message New(string type, string text = null){
            return new Message{Type = type, From = myName, Id = MyId, Text = text};

        }

        public static byte[] Encode(Message m){
            return JsonSerializer.SerializeToUtf8Bytes(m, Options);
        }

        // false for anythin that is not a valid message 
        public static bool TryDecode(byte[] buffer, int length, out Message m, out string error){

            m = null;
            if (length > MaxDatagram){
                error = "too long";
                return false;
            }

            try{
                m = JsonSerializer.Deserialize<Message>(buffer.AsSpan(0, length), Options);

            }

            catch (JsonException){
                error = "not json";
                return false;
            }

            error = Check(m);
            return error == null;
        }

        static string Check (Message m){
            if (m == null) return "empty";
            if (!Types.Contains(m.Type)) return "unknown type";
            if (! IsValidName(m.From)) return "bad name";
            if (string.IsNullOrEmpty(m.Id)) return "no id";
            if (m.Type == Msg && !IsValidText(m.Text)) return "bad text";
            if (m.Type == Pm && (m.Nonce == null || m.Chiphertext == null)) return "bad pm";

            return null;

        } 

        // accept letters digits _ and - as name
        // no spaces for the command writing purposes

        public static bool IsValidName(strig name){

            if (string.IsNullOrEmpty(name) || name.Length > MaxName) return false;

            foreach (char c in name){

                if (!char.IsLetterOrDigit(c) && c != '_' && c != '-') return false;
            }

            return true;
        }

        // max of 500 characters

        public static bool IsValidtext(string text){
            if (text = null || text.Length > MaxText) return false;
            
            foreach (char c in text){
                if (char.Iscontrol(c)) return false;
            }

            return true;
        }
    }

}
