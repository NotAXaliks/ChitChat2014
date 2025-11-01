namespace ChitChatApi.Dtos;

public class ApiResponse<T>
{
    public bool IsSuccess => Error is null;
    
    public string? Error { get; set; }
    
    public T? Data { get; set; }

    public static ApiResponse<T> FromError(string error)
        => new() { Error = error };

    public static ApiResponse<T> FromData(T data)
        => new() { Data = data };
}
