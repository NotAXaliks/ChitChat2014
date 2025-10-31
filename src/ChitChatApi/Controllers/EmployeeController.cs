using System.ComponentModel.DataAnnotations;
using System.Text.Json;
using ChitChatApi.Context;
using ChitChatApi.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace ChitChatApi.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class EmployeeController : ControllerBase
    {
        public IActionResult SendError(string error)
        {
            return Ok(new { Error = error });
        }

        private readonly AppDbContext _context;

        public EmployeeController(AppDbContext context)
        {
            _context = context;
        }

        // api/employee/register
        [HttpPost("register")]
        public async Task<IActionResult> Register([FromBody] EmployeeRegisterDto dto)
        {
            /* Валидация */
            if (!ModelState.IsValid) return SendError("Invalid validation");
            if (dto.Department_Id <= 0) return SendError("Department_Id is required");
            /* Конец валидации */

            if (await _context.Employees.AnyAsync(e => e.Username == dto.Username))
                return SendError("Employee already exists");

            var department = await _context.Departments.FindAsync(dto.Department_Id);
            if (department == null) return SendError("Department is not found");

            var hashedPassword = BCrypt.Net.BCrypt.HashPassword(dto.Password);

            var employee = new Employee
            {
                Name = dto.Name,
                Department_Id = dto.Department_Id,
                Username = dto.Username,
                Password = hashedPassword,
            };

            await _context.Employees.AddAsync(employee);
            await _context.SaveChangesAsync();

            var sessionToken = GenerateSession(employee.Id);

            return Ok(new { employee.Id, employee.Username, employee.Name, sessionToken });
        }

        // api/employee/login
        [HttpPost("login")]
        public async Task<IActionResult> Login([FromBody] EmployeeLoginDto dto)
        {
            /* Валидация */
            if (!ModelState.IsValid) return SendError("Invalid validation");
            /* Конец валидации */

            var employee = await _context.Employees.FirstOrDefaultAsync(e => e.Username == dto.Username);
            if (employee == null) return SendError("Employee not found");

            if (!BCrypt.Net.BCrypt.Verify(dto.Password, employee.Password))
                return SendError("Invalid password");

            var sessionToken = GenerateSession(employee.Id);

            return Ok(new { employee.Id, employee.Username, employee.Name, sessionToken });
        }

        [HttpGet("getMe")]
        public async Task<IActionResult> GetMe()
        {
            var userId = HttpContext.Session.GetInt32("UserId");
            if (userId == null) return Unauthorized();

            var employee = await _context.Employees.FindAsync(userId);
            if (employee == null)
                return SendError("Employee is not found");

            return Ok(employee);
        }

        public string GenerateSession(int employeeId)
        {
            var sessionToken = Guid.NewGuid().ToString();

            HttpContext.Session.SetString("SessionToken", sessionToken);
            HttpContext.Session.SetInt32("UserId", employeeId);

            return sessionToken;
        }
    }
}

public class EmployeeRegisterDto
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

public class EmployeeLoginDto
{
    [Required(ErrorMessage = "Employee Username is required")]
    [MaxLength(32, ErrorMessage = "Max Employee Username length is 32")]
    public required string Username { get; set; }

    [Required(ErrorMessage = "Employee Password is required")]
    public required string Password { get; set; }
}