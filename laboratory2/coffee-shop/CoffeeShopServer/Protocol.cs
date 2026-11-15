// task 1
// making products in dictionaries
// check for errors and format messages

using System.Collections.Generic;

namespace CoffeeShopServer{
    public static class Protocol{
        public const int MaxQuantity = 1000;

        //parts are he words of the message
        //reads the products from parts

        public static bool TryParseItems(string[] parts, out Dictionary<string, int> items, out string error){
            items = new Dictionary<string, int>();
            error = null;

            string command = parts[0].ToUpper();
            if (parts.Length < 2){
                error = command + " needs at least one product, e.g. " + command + " coffee=2 tea=1";
                return false;
            }

            for (int i = 1; i < parts.Length; i++){

                string[] pair = parts[i].Split('=');

                if (pair.Length != 2){

                    error = "'" + parts[i] + "' must look like product=quantity";
                    return false;
                }

                string product = pair[0].ToLower();
                if (!Products.IsKnown(product)){
                    error = "unknown product '" + pair[0] + "'";
                    return false;
                }

                int quantity;

                if (!int.TryParse(pair[1], out quantity) || quantity < 1 || quantity > MaxQuantity){

                    error = "the quantity of " + product + " must be a number from 1 to " + MaxQuantity;
                    return false;
                }

                if (items.ContainsKey(product)){

                    error = product + " is written twice";
                    return false;
                }

                items[product] = quantity;

            }

            return true;
        }


        // {coffee: 2, tea: 1} into "coffee=2 tea=1"
        public static string FormatItems(Dictionary<string, int> items){
            string text = "";
            foreach (KeyValuePair<string, int> item in items){

                if (text != ""){

                    text += " ";

                }
                text += item.Key + "=" + item.Value;
            }

            return text;
        }

        public static bool IsPrintable(string text){
            foreach (char c in text){
                if (char.IsControl(c) || c == '�'){
                    return false;
                }
            }
            return true;
        }
    }
}
