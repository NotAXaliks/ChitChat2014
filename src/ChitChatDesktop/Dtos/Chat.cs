namespace ChitChatDesktop.Dtos;

public record ChatroomDto(int Id, string Topic);

public record OpenChatDto(ChatroomDto Chatroom, long? LastMessageDate);

public record OpenableChat(int Id, string Topic, string LastMessageDate);
