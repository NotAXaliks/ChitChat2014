using System;
using System.Net.Http;
using System.Text.Json;
using System.Threading.Tasks;
using ChitChatDesktop.Dtos;

namespace ChitChatDesktop.Services;

public class EmployeeApi
{
    public static async Task<ApiResponse<LoginDataDto>> Login(string username, string password)
    {
        var data = new EmployeeLoginDto(username, password);

        try
        {
            var response = await NetManager.Post("employee/login", data);
            
            if (!response.IsSuccessStatusCode) return new ApiResponse<LoginDataDto>(null, response.Content.ToString());

            var result = await NetManager.ParseResponse<ApiResponse<LoginDataDto>>(response);
            return result;
        }
        catch (HttpRequestException e)
        {
            return new ApiResponse<LoginDataDto>(null, $"Сетевая ошибка: {e.Message}");
        }
        catch (JsonException e)
        {
            return new ApiResponse<LoginDataDto>(null, $"Ошибка разбора JSON: {e.Message}");
        }
        catch (Exception e)
        {
            return new ApiResponse<LoginDataDto>(null, $"Неизвестная ошибка: {e.Message}");
        }
    }
}