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
using BCrypt.Net; // Ensure this is referencing the correct BCrypt package

namespace SWPApp.Controllers.ProfilePage
{
    [Route("api/[controller]")]
    [ApiController]
    public class ProfileController : ControllerBase
    {
        private readonly DiamondAssesmentSystemDBContext _context;
        private readonly IEmailService _emailService;
        private readonly ILogger<ProfileController> _logger;

        public ProfileController(DiamondAssesmentSystemDBContext context, IEmailService emailService, ILogger<ProfileController> logger)
        {
            _context = context;
            _emailService = emailService;
            _logger = logger;
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

        public class ChangePasswordModel
        {
            [Required]
            public string CurrentPassword { get; set; }

            [Required]
            [RegularExpression(@"^(?=.*[A-Za-z])(?=.*\d)(?=.*[@$!%*?&])[A-Za-z\d@$!%*?&]{8,}$",
                ErrorMessage = "Password must be at least 8 characters long and contain at least one number and one special character.")]
            public string NewPassword { get; set; }

            [Required]
            [Compare("NewPassword", ErrorMessage = "The new password and confirmation password do not match.")]
            public string ConfirmNewPassword { get; set; }
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

        // Endpoint to change password
        [HttpPut("change-password")]
        public async Task<IActionResult> ChangePassword([FromBody] ChangePasswordModel model)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            // Check if user is logged in
            var customer = await _context.Customers
                .FirstOrDefaultAsync(c => c.LoginToken != null && c.LoginTokenExpires != null && c.LoginTokenExpires > DateTime.UtcNow);

            if (customer == null)
            {
                return Unauthorized("You must be logged in to change your password.");
            }

            // Verify the current password
            if (!BCrypt.Net.BCrypt.Verify(model.CurrentPassword, customer.Password))
            {
                return Unauthorized("Invalid current password.");
            }

            // Check if the new password and confirm password match
            if (model.NewPassword != model.ConfirmNewPassword)
            {
                return BadRequest("The new password and confirmation password do not match.");
            }

            // Hash the new password and save it
            customer.Password = BCrypt.Net.BCrypt.HashPassword(model.NewPassword);
            await _context.SaveChangesAsync();

            return Ok("Password changed successfully.");
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

        // Logout endpoint
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
