using ChitChatApi.Context;
using ChitChatApi.Dtos;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace ChitChatApi.Controllers
{
    [Route("api/[controller]")]
    public class ChatsController(AppDbContext database) : BaseController
    {
        // api/chats/getOpen
        [HttpGet("getOpen")]
        public async Task<IActionResult> GetOpen()
        {
            var userId = HttpContext.Session.GetInt32("UserId");
            if (userId == null) return SendError("Unauthorized");

            var result = await database.Chatrooms
                .Where(c => c.Members.Any(m => m.Employee_Id == userId))
                .Select(c => new
                {
                    Chatroom = c,
                    LastMessageDate = c.Messages
                        .OrderByDescending(m => m.Id)
                        .Select(m => (DateTime?)m.Date)
                        .FirstOrDefault()
                })
                .ToListAsync();
            
            return SendData(result.Select(data => new OpenChatResponseDto(
                new ChatroomDto(data.Chatroom.Id, data.Chatroom.Topic),
                ((DateTimeOffset?)data.LastMessageDate)?.ToUnixTimeMilliseconds())).ToArray());
        }
    }
}