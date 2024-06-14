using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using SWPApp.Controllers.Loginpage;
using SWPApp.DTO;
using SWPApp.Models;
using System.ComponentModel.DataAnnotations;
using System.Security.Claims;
using System.Security.Cryptography;
using System.Threading.Tasks;

namespace SWPApp.Controllers.ProfilePage
{
    [Route("api/[controller]")]
    [ApiController]
    public class ProfileController : ControllerBase
    {
        private readonly DiamondAssesmentSystemDBContext _context;
        private readonly IEmailService _emailService;
        public ProfileController(DiamondAssesmentSystemDBContext context)
        {
            _context = context;
        }

        public class ProfileModels
        {
            public string? CustomerName { get; set; }

            [EmailAddress]
            public string Email { get; set; }

            public string Password { get; set; }

            [Phone]
            public string? PhoneNumber { get; set; } // Nullable string for PhoneNumber

            public string? IDCard { get; set; } // Nullable string for IDCard

            public string? Address { get; set; } // Nullable string for Address
        }

        // Endpoint to update CustomerName
        [HttpPut("update-customername")]
        public async Task<IActionResult> UpdateCustomerName([FromBody] string customerName)
        {
            var customer = await GetCustomerFromContext();

            if (customer == null)
            {
                return NotFound("Customer not found");
            }

            customer.CustomerName = customerName;
            await _context.SaveChangesAsync();

            return Ok($"Customer name updated to: {customerName}");
        }

        // Endpoint to update Email
        [HttpPut("update-email")]
        public async Task<IActionResult> UpdateEmail([FromBody] string email)
        {
            var customer = await GetCustomerFromContext();

            if (customer == null)
            {
                return NotFound("Customer not found");
            }

            customer.Email = email;
            await _context.SaveChangesAsync();

            return Ok($"Email updated to: {email}");
        }

        // Endpoint to update Password
        [HttpPut("update-password")]
        public async Task<IActionResult> UpdatePassword([FromBody] string password)
        {
            var customer = await GetCustomerFromContext();

            if (customer == null)
            {
                return NotFound("Customer not found");
            }

            customer.Password = password;
            await _context.SaveChangesAsync();

            return Ok("Password updated");
        }

        // Endpoint to update PhoneNumber
        [HttpPut("update-phonenumber")]
        public async Task<IActionResult> UpdatePhoneNumber([FromBody] string phoneNumber)
        {
            var customer = await GetCustomerFromContext();

            if (customer == null)
            {
                return NotFound("Customer not found");
            }

            customer.PhoneNumber = phoneNumber;
            await _context.SaveChangesAsync();

            return Ok($"Phone number updated to: {phoneNumber}");
        }

        // Endpoint to update IDCard
        [HttpPut("update-idcard")]
        public async Task<IActionResult> UpdateIDCard([FromBody] string idCard)
        {
            var customer = await GetCustomerFromContext();

            if (customer == null)
            {
                return NotFound("Customer not found");
            }

            customer.IDCard = idCard;
            await _context.SaveChangesAsync();

            return Ok($"ID card updated to: {idCard}");
        }

        // Endpoint to update Address
        [HttpPut("update-address")]
        public async Task<IActionResult> UpdateAddress([FromBody] string address)
        {
            var customer = await GetCustomerFromContext();

            if (customer == null)
            {
                return NotFound("Customer not found");
            }

            customer.Address = address;
            await _context.SaveChangesAsync();

            return Ok($"Address updated to: {address}");
        }

        // Helper method to retrieve customer based on some context (e.g., token, session, etc.)
        private async Task<Customer> GetCustomerFromContext()
        {
            // Example: Retrieve customer based on token or any other context
            var customerId = HttpContext.User.FindFirst(ClaimTypes.NameIdentifier)?.Value;

            if (customerId == null)
            {
                return null;
            }

            return await _context.Customers.FindAsync(customerId);
        }
        //Change Password!!!!
        [HttpPost("change-password")]
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

            customer.Password = model.NewPassword;
            customer.ResetToken = null; // Invalidate the token
            customer.ResetTokenExpires = null; // Invalidate the token expiration
            await _context.SaveChangesAsync();

            return Ok("Password reset successful.");
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
        //Logout???
        private readonly ILogger<AuthController> _logger;

        [HttpPost("logout")]
        public async Task<IActionResult> Logout()
        {
            var customer = await _context.Customers.FirstOrDefaultAsync(c => c.Status == true);

            if (customer == null)
            {
                return Unauthorized("You must Login");
            }

            // Invalidate the login token
            customer.LoginToken = null;
            customer.LoginTokenExpires = null;

            // Set status to indicate logged out
            customer.Status = false;

            _context.Customers.Update(customer);
            await _context.SaveChangesAsync();

            return Ok("Logout successful.");
        }
    }
}

