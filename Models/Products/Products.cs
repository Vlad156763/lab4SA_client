using System.Collections.Generic;

namespace lab4.Models {
    public static class Products {
        private static  List<Product> products = new List<Product>{};
        private class Product {
            public string? Name { get; set; } = "";
            public string? Amount { get; set; } = "";
            public string? Price { get; set; } = "";
            public string? Measurement { get; set; } = "";
        }
        private static void add(string Name, string Amount, string Price, string Measurement) {
            products.Add(new Product {
                Name = Name,
                Amount = Amount,
                Price = Price,
                Measurement = Measurement
            });
        }
        // public static async void fillFromDB() {
            
        // }
    }
}