namespace ChitChatDesktop.Dtos;

public record ChatroomDto(int Id, string Topic);

public record OpenChatDto(ChatroomDto Chatroom, long? LastMessageDate);

