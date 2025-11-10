using System.ComponentModel.DataAnnotations;

namespace ChitChatApi.Dtos;

public record ChatroomDto(int Id, string Topic);

// public record ChatroomMessageWithSenderDto(int Id, EmployeeDto Employee, int Date, string Message);

// Далее идут возвращаемые данные

public record OpenChatResponseDto(ChatroomDto Chatroom, long? LastMessageDate);

public record ChatroomResponseDto(ChatroomDto Chatroom, EmployeeDto[] Members);

// Далее идут Вводимые пользователем данные

public class ChatroomCreateRequestDto
{
    [Required(ErrorMessage = "Employee_Ids is required")]
    public required int[] Employee_Ids { get; set; }
    
    [MaxLength(100, ErrorMessage = "Max Topic length is 100")]
    public string? Topic { get; set; }
}
