using System;
using System.Net.Http;
using System.Runtime.Caching;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;
using ChitChatDesktop.Dtos;

namespace ChitChatDesktop.Services;

public static class NetManager
{
    private const string Url = "http://localhost:40002/api/";
    private static readonly HttpClient HttpClient = new();
    public static readonly MemoryCache Cache = MemoryCache.Default;

    public static async Task<ApiResponse<T>> Get<T>(string path)
    {
        try
        {
            var response = await HttpClient.GetAsync(Url + path);

            return await GetResponse<T>(response);
        }
        catch (HttpRequestException e)
        {
            return new ApiResponse<T>(default, $"Request Error: {e.Message}");
        }
        catch (Exception e)
        {
            return new ApiResponse<T>(default, $"Unknown error: {e.Message}");
        }
    }

    public static async Task<ApiResponse<T>> Post<T>(string path, object data)
    {
        try
        {
            var jsData = JsonSerializer.Serialize(data);
            var response = await HttpClient.PostAsync(Url + path,
                new StringContent(jsData, Encoding.UTF8, "application/json"));

            return await GetResponse<T>(response);
        }
        catch (HttpRequestException e)
        {
            return new ApiResponse<T>(default, $"Request Error: {e.Message}");
        }
        catch (Exception e)
        {
            return new ApiResponse<T>(default, $"Unknown error: {e.Message}");
        }
    }

    public static async Task<ApiResponse<T>> Put<T>(string path, object? data = null)
    {
        try
        {
            HttpResponseMessage response;
            
            if (data != null)
            {
                var jsData = JsonSerializer.Serialize(data);
                response =
                    await HttpClient.PutAsync(Url + path, new StringContent(jsData, Encoding.UTF8, "application/json"));

                return await GetResponse<T>(response);
            }

            response = await HttpClient.PutAsync(Url + path, null);

            return await GetResponse<T>(response);
        }
        catch (HttpRequestException e)
        {
            return new ApiResponse<T>(default, $"Request Error: {e.Message}");
        }
        catch (Exception e)
        {
            return new ApiResponse<T>(default, $"Unknown error: {e.Message}");
        }
    }

    public static async Task<ApiResponse<T>> Delete<T>(string path)
    {
        try
        {
            var response = await HttpClient.DeleteAsync(Url + path);

            return await GetResponse<T>(response);
        }
        catch (HttpRequestException e)
        {
            return new ApiResponse<T>(default, $"Request Error: {e.Message}");
        }
        catch (Exception e)
        {
            return new ApiResponse<T>(default, $"Unknown error: {e.Message}");
        }
    }

    private static async Task<ApiResponse<T>> GetResponse<T>(HttpResponseMessage response)
    {
        if (!response.IsSuccessStatusCode) return new ApiResponse<T>(default, response.Content.ToString());

        var content = string.Empty;
        try
        {
            content = await response.Content.ReadAsStringAsync();
            
            return JsonSerializer.Deserialize<ApiResponse<T>>(content)!;
        }
        catch (JsonException e)
        {
            return new ApiResponse<T>(default, $"Invalid Response: {content}");
        }
        catch (Exception e)
        {
            return new ApiResponse<T>(default, $"Unknown error: {e.Message}");
        }
    }
}