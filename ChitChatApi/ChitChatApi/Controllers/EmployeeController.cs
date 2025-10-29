using System.ComponentModel.DataAnnotations;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using ChitChatApi.Context;
using ChitChatApi.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;

namespace ChitChatApi.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class EmployeeController : ControllerBase
    {
        private readonly AppDbContext _context;
        private readonly IConfiguration _configuration;

        public EmployeeController(AppDbContext context, IConfiguration configuration)
        {
            _context = context;
            _configuration = configuration;
        }

        // api/employee/register
        [HttpPost("register")]
        public async Task<IActionResult> Register([FromBody] EmployeeRegisterDto dto)
        {
            /* Валидация */
            if (!ModelState.IsValid) return BadRequest(ModelState);
            if (dto.Department_Id <= 0) return BadRequest("Department_Id is required");
            /* Конец валидации */

            if (await _context.Employees.AnyAsync(e => e.Username == dto.Username))
                return BadRequest("Employee already exists");

            var department = await _context.Departments.FindAsync(dto.Department_Id);
            if (department == null) return BadRequest("Department is not found");

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

            return Ok(new { employee.Id, employee.Username });
        }

        // api/employee/login
        [HttpPost("login")]
        public async Task<IActionResult> Login([FromBody] EmployeeLoginDto dto)
        {
            /* Валидация */
            if (!ModelState.IsValid) return BadRequest(ModelState);
            /* Конец валидации */

            var employee = await _context.Employees.SingleAsync(e => e.Username == dto.Username);
            if (employee == null) return BadRequest("Employee not found");

            if (!BCrypt.Net.BCrypt.Verify(dto.Password, employee.Password))
                return BadRequest("Invalid password");

            var token = GenerateJwtToken(employee);

            return Ok(new { token });
        }

        [Authorize]
        [HttpGet("getMe")]
        public async Task<IActionResult> GetMe()
        {
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            if (userId == null) return Unauthorized();

            var employee = await _context.Employees.FindAsync(int.Parse(userId));
            if (employee == null)
                return NotFound();

            return Ok(employee);
        }

        private string GenerateJwtToken(Employee employee)
        {
            var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_configuration["Jwt:Key"]!));
            var creds = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);

            var claims = new[]
            {
                new Claim(JwtRegisteredClaimNames.Sub, employee.Id.ToString()),   // userId
                // new Claim(JwtRegisteredClaimNames.UniqueName, employee.Username), // username
            };

            var token = new JwtSecurityToken(
                issuer: _configuration["Jwt:Issuer"],
                audience: _configuration["Jwt:Audience"],
                claims: claims ?? [],
                expires: DateTime.UtcNow.AddMinutes(Convert.ToDouble(_configuration["Jwt:ExpiresInMinutes"])),
                signingCredentials: creds
            );

            return new JwtSecurityTokenHandler().WriteToken(token);
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
