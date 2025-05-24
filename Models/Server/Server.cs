using System;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;

namespace lab4.Models {
    public static class Server {
        public static async Task sendQuery() {
            var url = "https://lab4sa-server.onrender.com/api/test";
            var json = "{\"message\":\"Привіт серверу!\"}";  // JSON-рядок, який відправляєш
            using var client = new HttpClient();
            var content = new StringContent(json, Encoding.UTF8, "application/json");
            var response = await client.PostAsync(url, content);
            var responseString = await response.Content.ReadAsStringAsync();
            Console.WriteLine("Відповідь від сервера: " + responseString);
        }
    }
}

