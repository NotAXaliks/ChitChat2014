namespace ChitChatApi.Dtos;

public record ChatroomDto(int Id, string Topic);

// public record ChatroomMessageWithSenderDto(int Id, EmployeeDto Employee, int Date, string Message);

// Далее идут возвращаемые данные

public record OpenChatResponseDto(ChatroomDto Chatroom, long? LastMessageDate);

public record ChatroomResponseDto(ChatroomDto Chatroom, EmployeeDto[] Members);
