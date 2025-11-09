using System.Threading.Tasks;
using ChitChatDesktop.Dtos;

namespace ChitChatDesktop.Services;

public class ChatApi
{
    public static async Task<ApiResponse<OpenChatDto[]>> GetOpen()
    {
        var result = await NetManager.Get<OpenChatDto[]>("chats/getOpen");

        return result;
    }

    public static async Task<ApiResponse<GetChatroomDataDto>> Get(int id)
    {
        var result = await NetManager.Get<GetChatroomDataDto>($"chats/{id}");

        return result;
    }
    
    public static async Task<ApiResponse<bool>> Leave(int id)
    {
        var result = await NetManager.Delete<bool>($"chats/{id}/me");

        return result;
    }
}