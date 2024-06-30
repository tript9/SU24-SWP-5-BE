using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using SWPApp.DTO;
using SWPApp.Models;
using SWPApp.Services;
using System;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;
using SWPApp.Utils;
using BCrypt;
using Azure.Messaging;

namespace SWPApp.Controllers.CustomerClient
{
    // Login and Register Models
    public class LoginModel
    {
        [Required]
        [EmailAddress]
        public string Email { get; set; }

        [Required]
        [DataType(DataType.Password)]
        public string Password { get; set; }
    }

    public class RegisterModel
    {
        [Required]
        public string? CustomerName { get; set; }

        [Required]
        [EmailAddress]
        public string Email { get; set; }

        [Required]
        [MinLength(8, ErrorMessage = "The password must be at least 8 characters long.")]
        public string Password { get; set; }

        [Required]
        [Compare("Password", ErrorMessage = "The password and confirmation password do not match.")]
        public string ConfirmPassword { get; set; }
    }

    public class ConfirmEmailModel
    {
        [Required]
        public string ConfirmationCode { get; set; }
    }

    [Route("api/[controller]")]
    [ApiController]
    public class AuthController : ControllerBase
    {
        private readonly DiamondAssesmentSystemDBContext _context;
        private readonly IEmailService _emailService;
        private readonly ILogger<AuthController> _logger;

        public AuthController(DiamondAssesmentSystemDBContext context, IEmailService emailService, ILogger<AuthController> logger)
        {
            _context = context;
            _emailService = emailService;
            _logger = logger;
        }

        [HttpPost("register")]
        public async Task<IActionResult> Register([FromBody] RegisterModel model)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            var email = model.Email.Trim();
            if (await _context.Customers.AnyAsync(c => c.Email == email))
            {
                return BadRequest("Email is already registered");
            }

            var hashedPassword = BCrypt.Net.BCrypt.HashPassword(model.Password);
            var confirmationCode = GenerateConfirmationCode();
            var customer = new Customer
            {
                CustomerName = model.CustomerName, // Add CustomerName here
                Email = email,
                Password = hashedPassword,
                Status = false, // Initially not confirmed
                ConfirmationToken = confirmationCode,
                ConfirmationTokenExpires = DateTime.UtcNow.AddDays(1) // Code expires in 1 day
            };

            _context.Customers.Add(customer);
            await _context.SaveChangesAsync();

            // Send confirmation email with the code
            var emailContent = $"<p>Your confirmation code is: <strong>{confirmationCode}</strong>.</p>";
            await _emailService.SendEmailAsync(model.Email, "Email Confirmation", emailContent);

            return Ok("Please check your email for the confirmation code.");
        }

        [HttpPost("confirm-email")]
        public async Task<IActionResult> ConfirmEmail([FromBody] ConfirmEmailModel model)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            var customer = await _context.Customers.FirstOrDefaultAsync(c => c.ConfirmationToken == model.ConfirmationCode);

            if (customer == null || customer.ConfirmationTokenExpires < DateTime.UtcNow)
            {
                return BadRequest("Invalid or expired confirmation code.");
            }

            customer.EmailConfirmed = true;
            customer.ConfirmationToken = null;
            customer.ConfirmationTokenExpires = null;
            customer.Status = true; // Email confirmed

            var loginToken = GenerateToken();
            customer.LoginToken = loginToken;
            customer.LoginTokenExpires = DateTime.UtcNow.AddMinutes(30);

            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateException ex)
            {
                _logger.LogError(ex, "An error occurred while updating the customer.");
                return StatusCode(500, "An error occurred while confirming the email.");
            }



            return Ok(new
            {
                Message = "Email confirmed successfully. You are now logged in. Role = 1",                
            });

        }


        [HttpPost("login")]
        public async Task<IActionResult> Login([FromBody] LoginModel loginModel)
        {
            var email = loginModel.Email.ToLower();
            var customer = await _context.Customers.FirstOrDefaultAsync(c => c.Email == email);
            var employee = await _context.Employees.FirstOrDefaultAsync(e => e.Email.ToLower() == email);

            if (customer == null && employee == null)
            {
                return Unauthorized("Invalid email or password");
            }

            if (customer != null && BCrypt.Net.BCrypt.Verify(loginModel.Password, customer.Password))
            {
                var loginToken = GenerateToken();
                customer.LoginToken = loginToken;
                customer.LoginTokenExpires = DateTime.UtcNow.AddMinutes(30);
                customer.Status = true; // Login successful, set status to true
                await _context.SaveChangesAsync();

                return Ok(new { Message = "Customer login successful.", LoginToken = loginToken, Role = 1, customer.CustomerName, customer.CustomerId });
            }

            if (employee != null )
            {
                var loginToken = GenerateToken();
                employee.LoginToken = loginToken;
                employee.LoginTokenExpires = DateTime.UtcNow.AddMinutes(30);
                employee.Status = true; // Login successful, set status to true
                await _context.SaveChangesAsync();

                // Determine the role-specific message and role value
                string roleSpecificMessage;
                int roleValue;

                if (employee.Role == null)
                {
                    roleSpecificMessage = "Role 1";
                    roleValue = 1;
                }
                else if (employee.Role == 0)
                {
                    roleSpecificMessage = "Role 2";
                    roleValue = 2;
                }
                else if (employee.Role == 1)
                {
                    roleSpecificMessage = "Role 3";
                    roleValue = 3;
                }
                else
                {
                    roleSpecificMessage = "Login successful";
                    roleValue = (int)employee.Role; // Use the actual role value for any other roles
                }

                return Ok(new { Message = roleSpecificMessage, LoginToken = loginToken, Role = roleValue, employee.EmployeeName, employee.EmployeeId });
            }

            return Unauthorized("Invalid email or password");
        }


        [HttpPost("forgot-password")]
        public async Task<IActionResult> ForgotPassword([FromBody] ForgotPasswordModel model)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            var customer = await _context.Customers.FirstOrDefaultAsync(c => c.Email == model.Email.Trim());
            if (customer == null)
            {
                return BadRequest("Email not found");
            }

            // Generate reset code
            var resetCode = GenerateResetCode();
            customer.ResetToken = resetCode;
            customer.ResetTokenExpires = DateTime.UtcNow.AddHours(1); // Code expires in 1 hour

            await _context.SaveChangesAsync();

            // Send email with the reset code
            var message = $"<p>You requested a password reset. Your reset code is: <strong>{resetCode}</strong>.</p>";
            await _emailService.SendEmailAsync(model.Email, "Password Reset Request", message);

            return Ok("Password reset code has been sent to your email.");
        }

        [HttpPost("reset-password")]
        public async Task<IActionResult> ResetPassword([FromBody] ResetPasswordModel model)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            var customer = await _context.Customers.FirstOrDefaultAsync(c => c.ResetToken == model.ResetCode);
            if (customer == null || customer.ResetTokenExpires < DateTime.UtcNow)
            {
                return BadRequest("Invalid code or code expired.");
            }

            if (model.NewPassword != model.ConfirmPassword)
            {
                return BadRequest("Passwords do not match.");
            }

            customer.Password = BCrypt.Net.BCrypt.HashPassword(model.NewPassword);
            customer.ResetToken = null; // Invalidate the token
            customer.ResetTokenExpires = null; // Invalidate the token expiration
            await _context.SaveChangesAsync();

            return Ok(new { message = "Password reset successful.", customerName = customer.CustomerName });
        }

        // Token generation method
        private string GenerateToken()
        {
            return Convert.ToBase64String(RandomNumberGenerator.GetBytes(64));
        }

        // Confirmation code generation method
        private string GenerateConfirmationCode()
        {
            const string chars = "ABCDEFGHIJKLMNOPQRSTUVWXYZ0123456789";
            var random = new Random();
            return new string(Enumerable.Repeat(chars, 6)
              .Select(s => s[random.Next(s.Length)]).ToArray());
        }

        // Reset code generation method
        private string GenerateResetCode()
        {
            const string chars = "ABCDEFGHIJKLMNOPQRSTUVWXYZ0123456789";
            var random = new Random();
            return new string(Enumerable.Repeat(chars, 6)
              .Select(s => s[random.Next(s.Length)]).ToArray());
        }
    }
}
