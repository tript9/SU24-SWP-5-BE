using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using SWPApp.DTO;
using SWPApp.Models;
using SWPApp.Services;
using System;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;

namespace SWPApp.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class LoginController : ControllerBase
    {
        private readonly DiamondAssesmentSystemDBContext _context;
        private readonly IEmailService _emailService;

        public LoginController(DiamondAssesmentSystemDBContext context, IEmailService emailService)
        {
            _context = context;
            _emailService = emailService;
        }

        [HttpPost("register")]
        public async Task<IActionResult> Register(RegisterModel model)
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

            var hashedPassword = HashPassword(model.Password);

            var confirmationToken = GenerateToken();
            var customer = new Customer
            {
                Email = email,
                Password = hashedPassword,
                Status = true,
                ConfirmationToken = confirmationToken,
                ConfirmationTokenExpires = DateTime.UtcNow.AddDays(1) // Token expires in 1 day
            };

            _context.Customers.Add(customer);
            await _context.SaveChangesAsync();

            // Send confirmation email
            var confirmationLink = Url.Action("ConfirmEmail", "Login", new { email = model.Email, token = confirmationToken }, Request.Scheme);
            await _emailService.SendEmailAsync(model.Email, "Email Confirmation", $"Please confirm your email by clicking <a href='{confirmationLink}'>here</a>.");

            return Ok("Registration successful. Please check your email to confirm your email address.");
        }

        [HttpPost("login")]
        public async Task<IActionResult> Login(LoginModel loginModel)
        {
            var hashedPassword = HashPassword(loginModel.Password.Trim());
            var email = loginModel.Email.Trim();

            Console.WriteLine($"Attempting login for email: {email}, role: {loginModel.Role}, hashedPassword: {hashedPassword}");

            if (string.IsNullOrEmpty(loginModel.Role) || loginModel.Role == "0" || loginModel.Role.Equals("Customer", StringComparison.OrdinalIgnoreCase))
            {
                var customer = await _context.Customers.FirstOrDefaultAsync(c => c.Email == email && c.Password == hashedPassword);

                if (customer == null)
                {
                    Console.WriteLine("Customer login failed: Invalid email or password");
                    return Unauthorized("Invalid email or password");
                }

                var loginToken = GenerateToken();
                customer.LoginToken = loginToken;
                customer.LoginTokenExpires = DateTime.UtcNow.AddMinutes(30);
                await _context.SaveChangesAsync();

                Console.WriteLine("Customer login successful");
                return Ok(new { Token = loginToken, Role = "Customer" });
            }
            else if (loginModel.Role == "1" || loginModel.Role.Equals("Staff", StringComparison.OrdinalIgnoreCase))
            {
                var employee = await _context.Employees.FirstOrDefaultAsync(e => e.Email == email && e.Password == hashedPassword);

                if (employee == null)
                {
                    Console.WriteLine("Staff login failed: Invalid email or password");
                    return Unauthorized("Invalid email or password");
                }

                var loginToken = GenerateToken();
                employee.LoginToken = loginToken;
                employee.LoginTokenExpires = DateTime.UtcNow.AddMinutes(30);
                await _context.SaveChangesAsync();

                Console.WriteLine("Staff login successful");
                return Ok(new { Token = loginToken, Role = "Staff" });
            }
            else
            {
                var customer = await _context.Customers.FirstOrDefaultAsync(c => c.Email == email && c.Password == hashedPassword);

                if (customer == null)
                {
                    Console.WriteLine("Default customer login failed: Invalid email or password");
                    return Unauthorized("Invalid email or password");
                }

                var loginToken = GenerateToken();
                customer.LoginToken = loginToken;
                customer.LoginTokenExpires = DateTime.UtcNow.AddMinutes(30);
                await _context.SaveChangesAsync();

                Console.WriteLine("Default customer login successful");
                return Ok(new { Token = loginToken, Role = "Customer" });
            }
        }

        [HttpPost("forgot-password")]
        public async Task<IActionResult> ForgotPassword(ForgotPasswordModel model)
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
            var message = $"<p>You requested a password reset. Use the following code to reset your password:</p><p>{resetCode}</p>";
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

            var customer = await _context.Customers.FirstOrDefaultAsync(c => c.ResetToken == model.Token);
            if (customer == null || customer.ResetTokenExpires < DateTime.UtcNow)
            {
                return BadRequest("Invalid code or code expired.");
            }

            if (model.NewPassword != model.ConfirmPassword)
            {
                return BadRequest("Passwords do not match.");
            }

            customer.Password = HashPassword(model.NewPassword);
            customer.ResetToken = null; // Invalidate the token
            customer.ResetTokenExpires = null; // Invalidate the token expiration
            await _context.SaveChangesAsync();

            return Ok("Password reset successful.");
        }

        [HttpGet("confirm-email")]
        public async Task<IActionResult> ConfirmEmail(string email, string token)
        {
            var customer = await _context.Customers.FirstOrDefaultAsync(c => c.Email == email.Trim() && c.ConfirmationToken == token);

            if (customer == null || customer.ConfirmationTokenExpires < DateTime.UtcNow)
            {
                return BadRequest("Invalid token or token expired.");
            }

            customer.EmailConfirmed = true;
            customer.ConfirmationToken = null;
            customer.ConfirmationTokenExpires = null;

            await _context.SaveChangesAsync();

            // Redirect to login page after confirmation
            return Redirect("/login");
        }

        // Hashing method
        private string HashPassword(string password)
        {
            using (SHA256 sha256 = SHA256.Create())
            {
                byte[] hashedBytes = sha256.ComputeHash(Encoding.UTF8.GetBytes(password));
                return Convert.ToBase64String(hashedBytes);
            }
        }

        // Token generation method
        private string GenerateToken()
        {
            return Convert.ToBase64String(RandomNumberGenerator.GetBytes(64));
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
