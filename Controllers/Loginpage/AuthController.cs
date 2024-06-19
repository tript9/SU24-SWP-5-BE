﻿using Microsoft.AspNetCore.Mvc;
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

namespace SWPApp.Controllers.Loginpage
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
        [EmailAddress]
        public string Email { get; set; }

        [Required]
        [RegularExpression(@"^(?=.*[A-Za-z])(?=.*\d)(?=.*[@$!%*?&])[A-Za-z\d@$!%*?&]{8,}$",
            ErrorMessage = "Password must be at least 8 characters long and contain at least one number and one special character.")]
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

        public AuthController(DiamondAssesmentSystemDBContext context, IEmailService emailService)
        {
            _context = context;
            _emailService = emailService;
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

            var confirmationCode = GenerateConfirmationCode();
            var customer = new Customer
            {
                Email = email,
                Password = model.Password,
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

            return Ok(new { Message = "Email confirmed successfully. You are now logged in.", LoginToken = loginToken });
        }

        [HttpPost("login")]
        public async Task<IActionResult> Login([FromBody] LoginModel loginModel)
        {
            var email = loginModel.Email;
            var customer = await _context.Customers.FirstOrDefaultAsync(c => c.Email == email);

            if (customer == null)
            {
                return Unauthorized("Invalid email or password");
            }

            var loginToken = GenerateToken();
            customer.LoginToken = loginToken;
            customer.LoginTokenExpires = DateTime.UtcNow.AddMinutes(30);
            customer.Status = true; // Login successful, set status to 1
            await _context.SaveChangesAsync();

            // Return the login token
            return Ok(new { Message = "Login successful.", LoginToken = loginToken });
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
    }
}
