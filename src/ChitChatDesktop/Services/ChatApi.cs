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
}