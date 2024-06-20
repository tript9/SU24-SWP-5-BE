using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using SWPApp.Controllers.Loginpage;
using SWPApp.Models;
using Microsoft.Extensions.Logging;
using System.Threading.Tasks;

namespace SWPApp.Controllers.Homepage
{
    [Route("api/[controller]")]
    [ApiController]
    public class LogoutController : ControllerBase
    {
        private readonly DiamondAssesmentSystemDBContext _context;
        private readonly IEmailService _emailService;
        private readonly ILogger<LogoutController> _logger;

        public LogoutController(DiamondAssesmentSystemDBContext context, IEmailService emailService, ILogger<LogoutController> logger)
        {
            _context = context;
            _emailService = emailService;
            _logger = logger;
        }

        [HttpPost("logout")]
        public async Task<IActionResult> Logout()
        {
            var customer = await _context.Customers.FirstOrDefaultAsync(c => c.Status == true && c.LoginToken != null);

            if (customer == null)
            {
                return Unauthorized("You must login.");
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
