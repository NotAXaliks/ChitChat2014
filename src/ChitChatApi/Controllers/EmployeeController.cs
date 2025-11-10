using ChitChatApi.Context;
using ChitChatApi.Dtos;
using ChitChatApi.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace ChitChatApi.Controllers
{
    [Route("api/[controller]")]
    public class EmployeeController(AppDbContext database) : BaseController
    {
        // Зарегистрироваться
        // api/employee/register
        [HttpPost("register")]
        public async Task<IActionResult> Register([FromBody] EmployeeRegisterRequestDto requestDto)
        {
            /* Валидация */
            if (!ModelState.IsValid) return SendError("Invalid validation");
            if (requestDto.Department_Id <= 0) return SendError("Department_Id is required");
            /* Конец валидации */

            if (await database.Employees.AnyAsync(e => e.Username == requestDto.Username))
                return SendError("Employee already exists");

            var department = await database.Departments.FindAsync(requestDto.Department_Id);
            if (department == null) return SendError("Department is not found");

            var hashedPassword = BCrypt.Net.BCrypt.HashPassword(requestDto.Password);

            var employee = new Employee
            {
                Name = requestDto.Name,
                Department_Id = requestDto.Department_Id,
                Username = requestDto.Username,
                Password = hashedPassword,
            };

            await database.Employees.AddAsync(employee);
            await database.SaveChangesAsync();

            var sessionToken = GenerateSession(employee.Id);

            return SendData(new EmployeeRegisterResponseDto(new EmployeeWithDepartmentDto(employee.Id, employee.Name,
                employee.Username,
                new DepartmentDto(department.Id, department.Name)), sessionToken));
        }

        // Войти в аккаунт
        // api/employee/login
        [HttpPost("login")]
        public async Task<IActionResult> Login([FromBody] EmployeeLoginRequestDto requestDto)
        {
            /* Валидация */
            if (!ModelState.IsValid) return SendError("Invalid validation");
            /* Конец валидации */

            var employee = await database.Employees.FirstOrDefaultAsync(e => e.Username == requestDto.Username);
            if (employee == null) return SendError("Employee not found");

            if (!BCrypt.Net.BCrypt.Verify(requestDto.Password, employee.Password))
                return SendError("Invalid password");

            var sessionToken = GenerateSession(employee.Id);

            return SendData(new EmployeeLoginResponseDto(
                new EmployeeDto(employee.Id, employee.Name, employee.Username, employee.Department_Id), sessionToken));
        }

        // Получить информацию о текущем работнике
        // api/employee/getMe
        [HttpGet("getMe")]
        public async Task<IActionResult> GetMe()
        {
            var userId = HttpContext.Session.GetInt32("UserId");
            if (userId == null) return SendError("Unauthorized");

            var employee = await database.Employees.FindAsync(userId);
            if (employee == null)
                return SendError("Employee is not found");

            return SendData(new EmployeeDto(employee.Id, employee.Name, employee.Username, employee.Department_Id));
        }

        // Список департаментов
        // api/employee/departments
        [HttpGet("departments")]
        public async Task<IActionResult> GetDepartments()
        {
            var departments = await database.Departments.ToListAsync();

            return SendData(departments.Select(d => new DepartmentDto(d.Id, d.Name)));
        }

        // Искать работника
        // api/employee/search
        [HttpPost("search")]
        public async Task<IActionResult> SearchEmployee([FromBody] EmployeeSearchRequestDto requestDto)
        {
            /* Валидация */
            if (!ModelState.IsValid) return SendError("Invalid validation");
            /* Конец валидации */

            var query = database.Employees
                .Where(e => requestDto.Department_Ids.Contains(e.Department_Id));

            if (!string.IsNullOrWhiteSpace(requestDto.Query))
            {
                query = query.Where(e => EF.Functions.ILike(e.Name, $"%{requestDto.Query}%"));
            }

            var employees = await query.OrderBy(e => e.Name).Take(50).ToListAsync();
            
            return SendData(employees.Select(e => new EmployeeDto(e.Id, e.Name, e.Username, e.Department_Id)));
        }
    }
}