using System;
using System.Threading.Tasks;
using ChitChatDesktop.Dtos;

namespace ChitChatDesktop.Services;

public class EmployeeApi
{
    public static async Task<ApiResponse<LoginDataDto>> Login(string username, string password)
    {
        var data = new EmployeeLoginDto(username, password);

        var result = await NetManager.Post<LoginDataDto>("employee/login", data);

        if (result.Data != null)
        {
            NetManager.Cache.Set("me", result.Data.Employee, DateTimeOffset.Now.AddMinutes(20));
        }

        return result;
    }

    public static async Task<ApiResponse<EmployeeDto>> GetMe()
    {
        if (NetManager.Cache.Contains("me"))
        {
            return new ApiResponse<EmployeeDto>((EmployeeDto)NetManager.Cache.Get("me")!);
        }

        var result = await NetManager.Get<EmployeeDto>("employee/getMe");

        if (result.Data != null)
        {
            NetManager.Cache.Set("me", result.Data, DateTimeOffset.Now.AddMinutes(20));
        }

        return result;
    }
}