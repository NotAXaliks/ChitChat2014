using ChitChatApi.Context;
using ChitChatApi.Dtos;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace ChitChatApi.Controllers
{
    [Route("api/[controller]")]
    public class ChatsController(AppDbContext database) : BaseController
    {
        // Получить открытые чаты
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

        // Получить информацию о чате
        // api/chats/{ID}
        [HttpGet("{chatId:int}")]
        public async Task<IActionResult> GetChat(int chatId)
        {
            var userId = HttpContext.Session.GetInt32("UserId");
            if (userId == null) return SendError("Unauthorized");

            var chatWithEmployees = await database.Chatrooms
                .Where(c => c.Id == chatId && c.Members.Any(cm => cm.Employee_Id == userId))
                .Select(c => new
                {
                    Chatroom = c,
                    Employees = c.Members
                        .Select(cm => cm.Employee!)
                        .ToList()
                })
                .FirstOrDefaultAsync();

            if (chatWithEmployees == null) return SendError("Chat not found");

            var members = chatWithEmployees.Employees
                .Select(e => new EmployeeDto(e.Id, e.Name, e.Username, e.Department_Id)).ToArray();

            return SendData(new ChatroomResponseDto(
                new ChatroomDto(chatWithEmployees.Chatroom.Id, chatWithEmployees.Chatroom.Topic), members));
        }

        // Выйти из чата
        // api/chats/{CHAT_ID}/me
        [HttpDelete("{chatId:int}/me")]
        public async Task<IActionResult> LeaveChat(int chatId)
        {
            var userId = HttpContext.Session.GetInt32("UserId");
            if (userId == null) return SendError("Unauthorized");

            await using var transaction = await database.Database.BeginTransactionAsync();

            var deletedCount = await database.ChatMembers
                .Where(c => c.Chatroom_Id == chatId && c.Employee_Id == userId)
                .ExecuteDeleteAsync();

            // Если никого не осталось в чате, удаляем чат
            bool hasMembers = await database.ChatMembers
                .AnyAsync(c => c.Chatroom_Id == chatId);
            if (!hasMembers)
            {
                await database.Chatrooms
                    .Where(cr => cr.Id == chatId)
                    .ExecuteDeleteAsync();
            }

            await transaction.CommitAsync();

            return SendData(deletedCount != 0);
        }

        // Выгнать пользователя из чата
        // api/chats/{chatId}/{employeeId}
        [HttpDelete("{chatId:int}/{employeeId:int}")]
        public async Task<IActionResult> LeaveChat(int chatId, int employeeId)
        {
            var userId = HttpContext.Session.GetInt32("UserId");
            if (userId == null) return SendError("Unauthorized");

            var chatWithEmployees = await database.ChatMembers
                .Where(c => c.Chatroom_Id == chatId && c.Employee_Id == employeeId)
                .ExecuteDeleteAsync();

            return SendData(chatWithEmployees != 0);
        }
    }
}