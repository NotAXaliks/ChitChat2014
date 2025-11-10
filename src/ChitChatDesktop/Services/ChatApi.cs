using System;
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

    public static async Task<ApiResponse<bool?>> Leave(int id)
    {
        var result = await NetManager.Delete<bool?>($"chats/{id}/me");

        return result;
    }

    public static async Task<ApiResponse<bool?>> AddEmployee(int chatId, int employeeId)
    {
        var result = await NetManager.Put<bool?>($"chats/{chatId}/{employeeId}");

        return result;
    }
    
    public static async Task<ApiResponse<GetChatroomDataDto>> CreateChat(int[] employeeIds, string? topic)
    {
        var data = new CreateChatDto(employeeIds, topic);
        
        var result = await NetManager.Post<GetChatroomDataDto>("chats/create", data);

        return result;
    }
    
    public static async Task<ApiResponse<ChatroomDto>> EditChat(int chatId, EditChatDto editOptions)
        {
            var result = await NetManager.Patch<ChatroomDto>($"chats/{chatId}", editOptions);
            
            return result;
        }
}