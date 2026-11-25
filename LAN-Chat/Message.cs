// one datagram of the chat sent as JSON 
// null fields left out 

namespace LanChat{

    public class Message{

        public string Type {get; set;}
        public string From {get; set;}
        public string Id {get; set;}
        public string Text {get; set;}
        public string Nonce {get; set;}
        public string Ciphertext {get; set;}
    }


}