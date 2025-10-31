using System;
using System.Net.Http;
using System.Text.Json;
using System.Threading.Tasks;

namespace ChitChatDesktop.Services;

public class EmployeeApi
{
    public class LoginRequest
    {
        public string Username { get; set; } = string.Empty;
        public string Password { get; set; } = string.Empty;
    }

    public class LoginResponse
    {
        public int Id { get; set; }
        public string Name { get; set; } = string.Empty;
        public string SessionId { get; set; } = string.Empty;
        public string? Error { get; set; }
    }
    
    public static async Task<LoginResponse> Login(string username, string password)
    {
        var data = new LoginRequest { Username = username, Password = password };

        try
        {
            var response = await NetManager.Post("employee/login", data);
            
            if (!response.IsSuccessStatusCode) return new LoginResponse { Error = response.Content.ToString() };

            var result = await NetManager.ParseResponse<LoginResponse>(response);
            return result;
        }
        catch (HttpRequestException e)
        {
            return new LoginResponse { Error = $"Сетевая ошибка: {e.Message}" };
        }
        catch (JsonException e)
        {
            return new LoginResponse { Error = $"Ошибка разбора JSON: {e.Message}" };
        }
        catch (Exception e)
        {
            return new LoginResponse { Error = $"Неизвестная ошибка: {e.Message}" };
        }
    }
}