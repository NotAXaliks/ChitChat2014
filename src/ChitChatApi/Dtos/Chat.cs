namespace ChitChatApi.Dtos;

public record ChatroomDto(int Id, string Topic);

// Далее идут возвращаемые данные

public record OpenChatResponseDto(ChatroomDto Chatroom, long? LastMessageDate);
