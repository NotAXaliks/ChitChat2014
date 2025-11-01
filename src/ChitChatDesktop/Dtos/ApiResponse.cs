namespace ChitChatDesktop.Dtos;

public record ApiResponse<T>(
    T? Data,
    string? Error = null
)
{
    public bool IsSuccess => Error is null;
}

public record LoginDataDto(
    EmployeeDto Employee,
    string SessionToken
);
