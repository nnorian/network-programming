using System;

namespace CoffeeServer{
    // products and prep time

    public static class Products{
            public static readonly string[] Names = {"coffee", "tea", "croissant"};

        public static bool IsKnown(String product){
            return Array.IndexOf(Names, product) >= 0;
        }

        public static int PreparationSeconds(string product){
            switch(product){

                case "coffeee": return 4;
                case "tea": return 3;
                case "croissant": return 2;
                default: throw new ArgumentException("unknown product: " + product);

            }
        }

    }
    
}
