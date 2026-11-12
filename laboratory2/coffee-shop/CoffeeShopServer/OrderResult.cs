namespace CoffeeShopServer{
    // shop answers for orders
    public class OrderResult{
        public bool Accepted;

        // accepted = true
        public int OrderNumber;
        public int PreparationSeconds;

        // accepted = false 
        public string MissingProduct;
        public int Left;
    }
}