using System;
using System.Net.Http;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;

namespace ChitChatDesktop.Services;

public static class NetManager
    {
        private const string Url = "http://localhost:5165/api/";
        private static readonly HttpClient HttpClient = new();

        public static async Task<T> Get<T>(string path)
        {
            var response = await HttpClient.GetAsync(Url + path);
            var content = await response.Content.ReadAsStringAsync();
            var data = JsonSerializer.Deserialize<T>(content);
            return data!;
        }

        public static async Task<HttpResponseMessage> Post<T>(string path, T data)
        {
            var jsData = JsonSerializer.Serialize(data);
            var response = await HttpClient.PostAsync(Url + path, new StringContent(jsData, Encoding.UTF8, "application/json"));
            return response;
        }
        public static async Task<HttpResponseMessage> Put<T>(string path, T data)
        {
            var jsData = JsonSerializer.Serialize(data);
            var response = await HttpClient.PutAsync(Url + path, new StringContent(jsData, Encoding.UTF8, "application/json"));
            return response;
        }

        public static async Task<HttpResponseMessage> Delete(string path)
        {
            var response = await HttpClient.DeleteAsync(Url + path);
            return response;
        }

        public static async Task<T> ParseResponse<T>(HttpResponseMessage responseMessage)
        {
            var content = await responseMessage.Content.ReadAsStringAsync();
            var data = JsonSerializer.Deserialize<T>(content);
            return data!;
        }
        
    }