using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using SWPApp.DTO;
using SWPApp.Models;
using System.ComponentModel.DataAnnotations;
using System.Security.Claims;
using System.Security.Cryptography;
using System.Threading.Tasks;
using BCrypt.Net;

namespace SWPApp.Controllers.CustomerClient
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

        public class UpdateProfileModel
        {
            public string? CustomerName { get; set; }

            [EmailAddress]
            public string? Email { get; set; }

            [Phone]
            public string? PhoneNumber { get; set; }

            public string? IDCard { get; set; }

            public string? Address { get; set; }
        }


        public class ChangePasswordModel
        {
            [Required]
            public string CurrentPassword { get; set; }

            [Required]
            [RegularExpression(@"^.{8,}$", ErrorMessage = "Password must be at least 8 characters long.")]
            public string NewPassword { get; set; }

            [Required]
            [Compare("NewPassword", ErrorMessage = "The new password and confirmation password do not match.")]
            public string ConfirmNewPassword { get; set; }
        }

        // Get Customer by ID
        [HttpGet("customer/{id}")]
        public async Task<IActionResult> GetCustomer(int id)
        {
            var customer = await _context.Customers.FindAsync(id);

            if (customer == null)
            {
                return NotFound();
            }

            return Ok(customer);
        }

        [HttpPost("update-profile/{id}")]
        public async Task<IActionResult> UpdateProfile(int id, [FromBody] UpdateProfileModel model)
        {
            var customer = await _context.Customers.FindAsync(id);

            if (customer == null)
            {
                return NotFound("Customer not found");
            }

            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            if (!string.IsNullOrEmpty(model.CustomerName))
            {
                customer.CustomerName = model.CustomerName;
            }

            if (!string.IsNullOrEmpty(model.Email))
            {
                customer.Email = model.Email;
            }

            if (!string.IsNullOrEmpty(model.PhoneNumber))
            {
                customer.PhoneNumber = model.PhoneNumber;
            }

            if (!string.IsNullOrEmpty(model.IDCard))
            {
                customer.IDCard = model.IDCard;
            }

            if (!string.IsNullOrEmpty(model.Address))
            {
                customer.Address = model.Address;
            }

            await _context.SaveChangesAsync();

            return Ok("Profile updated successfully.");
        }
        //Change password
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

        // Helper method to retrieve customer based on some context (e.g., token, session, etc.)
        private async Task<Customer> GetCustomerFromContext()
        {
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


    }
}