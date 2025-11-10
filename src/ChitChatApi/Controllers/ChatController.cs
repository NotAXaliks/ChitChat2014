using ChitChatApi.Context;
using ChitChatApi.Dtos;
using ChitChatApi.Models;
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

            var result = await RemoveUserFromChat(chatId, userId.Value);

            return SendData(result);
        }

        // Добавить пользователя в чат
        // api/chats/{CHAT_ID}/{EMPLOYEE_ID}
        [HttpPut("{chatId:int}/{employeeId:int}")]
        public async Task<IActionResult> AddToChat(int chatId, int employeeId)
        {
            var userId = HttpContext.Session.GetInt32("UserId");
            if (userId == null) return SendError("Unauthorized");
            
            if (!(await database.Employees.AnyAsync(e => e.Id == employeeId))) return SendError("Employee is not found");

            var employeeAndUserInChat = await database.ChatMembers
                .Where(m => m.Chatroom_Id == chatId && (m.Employee_Id == employeeId || m.Employee_Id == userId))
                .Select(m => m.Employee_Id)
                .ToListAsync();
            
            if (!employeeAndUserInChat.Contains(userId.Value)) 
            {
                return SendError("You are not in this chat.");
            }
            if (employeeAndUserInChat.Contains(employeeId)) 
            {
                return SendError("Employee is already in this chat.");
            }
            
            await database.ChatMembers
                .AddAsync(new ChatMembers { Chatroom_Id = chatId, Employee_Id = employeeId });
            await database.SaveChangesAsync();
            
            return SendData(true);
        }

        private async Task<bool> RemoveUserFromChat(int chatId, int userId)
        {
            await using var transaction = await database.Database.BeginTransactionAsync();

            var hasChat = await database.Chatrooms.AnyAsync(c => c.Id == chatId);
            if (!hasChat) return false;

            var deletedCount = await database.ChatMembers
                .Where(c => c.Chatroom_Id == chatId && c.Employee_Id == userId)
                .ExecuteDeleteAsync();

            // Если никого не осталось в чате, удаляем чат
            var hasMembers = await database.ChatMembers
                .AnyAsync(c => c.Chatroom_Id == chatId);
            if (!hasMembers)
            {
                await database.Chatrooms
                    .Where(cr => cr.Id == chatId)
                    .ExecuteDeleteAsync();
            }

            await transaction.CommitAsync();

            return deletedCount != 0;
        }
    }
}