using ChitChatApi.Context;
using ChitChatApi.Dtos;
using ChitChatApi.Models;
using System.Text.Json;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace ChitChatApi.Controllers
{
    [Route("api/[controller]")]
    public class EmployeeController : BaseController
    {
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

            return SendData(new
            {
                employee = new EmployeeWithDepartmentDto(employee.Id, employee.Name, employee.Username,
                    new DepartmentDto(department.Id, department.Name)),
                sessionToken,
            });
            
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

            return SendData(new
            {
                Employee = new EmployeeDto(employee.Id, employee.Name, employee.Username, employee.Department_Id),
                SessionToken = sessionToken,
            });
        }

        [HttpGet("getMe")]
        public async Task<IActionResult> GetMe()
        {
            var userId = HttpContext.Session.GetInt32("UserId");
            if (userId == null) return SendError("Unauthorized");

            var employee = await _context.Employees.FindAsync(userId);
            if (employee == null)
                return SendError("Employee is not found");

            return SendData(new EmployeeDto(employee.Id, employee.Name, employee.Username, employee.Department_Id));
        }
    }
}