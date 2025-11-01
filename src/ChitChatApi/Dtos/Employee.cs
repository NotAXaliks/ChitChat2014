using System.ComponentModel.DataAnnotations;

namespace ChitChatApi.Dtos;

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

// Далее идут возвращаемые данные

public record EmployeeRegisterResponseDto(EmployeeWithDepartmentDto Employee, string SessionToken);
public record EmployeeLoginResponseDto(EmployeeDto Employee, string SessionToken);

// Далее идут Вводимые пользователем данные

public class EmployeeRegisterRequestDto
{
    [Required(ErrorMessage = "Employee Name is required")]
    [MaxLength(100, ErrorMessage = "Max Employee Name length is 100")]
    public required string Name { get; set; }

    public int Department_Id { get; set; }

    [Required(ErrorMessage = "Employee Username is required")]
    [MaxLength(32, ErrorMessage = "Max Employee Username length is 32")]
    public required string Username { get; set; }

    [Required(ErrorMessage = "Employee Password is required")]
    public required string Password { get; set; }
}

public class EmployeeLoginRequestDto
{
    [Required(ErrorMessage = "Employee Username is required")]
    [MaxLength(32, ErrorMessage = "Max Employee Username length is 32")]
    public required string Username { get; set; }

    [Required(ErrorMessage = "Employee Password is required")]
    public required string Password { get; set; }
}
