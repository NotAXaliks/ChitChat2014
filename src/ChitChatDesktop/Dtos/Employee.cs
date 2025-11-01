namespace ChitChatDesktop.Dtos;

public record EmployeeDto(
    int Id,
    string Name,
    string Username,
    int Department_Id
);
public record EmployeeWithDepartmentDto(
    int Id,
    string Name,
    string Username,
    DepartmentDto Department
);

// Далее идут Вводимые пользователем данные

public record EmployeeRegisterDto(string Username, string Password, string Name, int Department_Id);

public record EmployeeLoginDto(string Username, string Password);
