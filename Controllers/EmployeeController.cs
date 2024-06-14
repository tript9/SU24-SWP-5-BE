using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using SWPApp.DTO;
using SWPApp.Models;
using SWPApp.Services;
using System;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Security.Cryptography;
using System.Threading.Tasks;

namespace SWPApp.Controllers
{
    public class EmployeeLoginModel
    {
        [Required]
        [EmailAddress]
        public string Email { get; set; }

        [Required]
        public string Password { get; set; }
    }

    [Route("api/[controller]")]
    [ApiController]
    public class EmployeeController : ControllerBase
    {
        private readonly DiamondAssesmentSystemDBContext _context;

        public EmployeeController(DiamondAssesmentSystemDBContext context)
        {
            _context = context;
        }

        [HttpPost("login")]
        public async Task<IActionResult> Login([FromBody] EmployeeLoginModel model)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            var email = model.Email.Trim().ToLower();
            var employee = await _context.Employees
                .FirstOrDefaultAsync(e => e.Email.ToLower() == email && e.Password == model.Password.Trim());

            if (employee == null)
            {
                return Unauthorized("Invalid email or password");
            }

            var loginToken = GenerateToken();
            employee.LoginToken = loginToken;
            employee.LoginTokenExpires = DateTime.UtcNow.AddMinutes(30);
            await _context.SaveChangesAsync();

            return Ok(new { Token = loginToken });
        }

        [HttpPost("logout")]
        public async Task<IActionResult> Logout([FromBody] LogoutModel model)
        {
            var employee = await _context.Employees
                .AsNoTracking()
                .FirstOrDefaultAsync(e => e.LoginToken == model.Token);

            if (employee == null)
            {
                return Unauthorized("Invalid token");
            }

            employee.LoginToken = null;
            employee.LoginTokenExpires = null;

            _context.Employees.Update(employee);
            await _context.SaveChangesAsync();

            return Ok("Logout successful.");
        }

        // Token generation method
        private string GenerateToken()
        {
            return Convert.ToBase64String(RandomNumberGenerator.GetBytes(64));
        }
    }

    public class LogoutModel
    {
        [Required]
        public string Token { get; set; }
    }
}
