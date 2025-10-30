using System.Net.Http;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;

namespace CampusBenchDesktop.Services;

public static class NetManager
    {
        public static string Url = "http://localhost:5165/";
        private static readonly HttpClient httpClient = new();

        public static async Task<T> Get<T>(string path)
        {
            var response = await httpClient.GetAsync(Url + path);
            var content = await response.Content.ReadAsStringAsync();
            var data = JsonSerializer.Deserialize<T>(content);
            return data!;
        }

        public static async Task<HttpResponseMessage> Post<T>(string path, T data)
        {
            var jsData = JsonSerializer.Serialize(data);
            var response = await httpClient.PostAsync(Url + path, new StringContent(jsData, Encoding.UTF8, "application/json"));
            return response;
        }
        public static async Task<HttpResponseMessage> Put<T>(string path, T data)
        {
            var jsData = JsonSerializer.Serialize(data);
            var response = await httpClient.PutAsync(Url + path, new StringContent(jsData, Encoding.UTF8, "application/json"));
            return response;
        }

        public static async Task<HttpResponseMessage> Delete(string path)
        {
            var response = await httpClient.DeleteAsync(Url + path);
            return response;
        }

        public static async Task<T> ParseResponse<T>(HttpResponseMessage responseMessage)
        {
            var content = await responseMessage.Content.ReadAsStringAsync();
            var data = JsonSerializer.Deserialize<T>(content);
            return data!;
        }
        
    }