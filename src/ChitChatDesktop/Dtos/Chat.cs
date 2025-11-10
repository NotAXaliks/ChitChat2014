namespace ChitChatDesktop.Dtos;

public record ChatroomDto(int Id, string Topic);

public record OpenChatDto(ChatroomDto Chatroom, long? LastMessageDate);

public record OpenableChat(int Id, string Topic, string LastMessageDate);

// Далее идут Вводимые пользователем данные

public record CreateChatDto(int[] Employee_Ids, string? Topic);

public class EditChatDto 
{
    public required string Topic { get; set; }
};
