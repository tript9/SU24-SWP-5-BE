using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using SWPApp.DTO;
using SWPApp.Models;
using System;
using System.ComponentModel.DataAnnotations;
using System.Threading.Tasks;
using System.Security.Cryptography;

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

            var email = model.Email;
            var employee = await _context.Employees.FirstOrDefaultAsync(e => e.Email.ToLower() == email);

            if (employee == null)
            {
                return Unauthorized("Invalid email or password");
            }

            var loginToken = GenerateToken();
            employee.LoginToken = loginToken;
            employee.LoginTokenExpires = DateTime.UtcNow.AddMinutes(30);
            employee.Status = true; // Set status to indicate logged in
            await _context.SaveChangesAsync();

            return Ok("Login successful");
        }

        [HttpPost("logout")]
        public async Task<IActionResult> Logout()
        {
            var employee = await _context.Employees.FirstOrDefaultAsync(c => c.Status==true);

            if (employee == null)
            {
                return Unauthorized("ERROR");
            }

            // Invalidate the login token
            employee.LoginToken = null;
            employee.LoginTokenExpires = null;

            // Set status to indicate logged out
            employee.Status = false;

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

   
}
