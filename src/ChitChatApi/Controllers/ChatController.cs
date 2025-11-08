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

        // api/chats/{ID}
        [HttpGet("{id}")]
        public async Task<IActionResult> GetChat(int id)
        {
            var userId = HttpContext.Session.GetInt32("UserId");
            if (userId == null) return SendError("Unauthorized");

            var chatWithEmployees = await database.Chatrooms
                .Where(c => c.Id == id && c.Members.Any(cm => cm.Employee_Id == userId))
                .Select(c => new
                {
                    Chatroom = c,
                    Employees = c.Members
                        .Select(cm => cm.Employee!)
                        .ToList()
                })
                .FirstOrDefaultAsync();
            
            if (chatWithEmployees == null) return SendError("Chat not found");
            
            var members = chatWithEmployees.Employees.Select(e => new EmployeeDto(e.Id, e.Name, e.Username, e.Department_Id)).ToArray();

            return SendData(new ChatroomResponseDto(new ChatroomDto(chatWithEmployees.Chatroom.Id, chatWithEmployees.Chatroom.Topic), members));
        }
    }
}