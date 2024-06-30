using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using SWPApp.Models;
using Microsoft.Extensions.Logging;
using System.Threading.Tasks;

namespace SWPApp.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class LogoutController : ControllerBase
    {
        private readonly DiamondAssesmentSystemDBContext _context;
        private readonly ILogger<LogoutController> _logger;

        public LogoutController(DiamondAssesmentSystemDBContext context, ILogger<LogoutController> logger)
        {
            _context = context;
            _logger = logger;
        }

        [HttpPost("logout")]
        public async Task<IActionResult> Logout()
        {
            var customer = await _context.Customers.FirstOrDefaultAsync(c => c.Status == true && c.LoginToken != null);
            var employee = await _context.Employees.FirstOrDefaultAsync(e => e.Status == true && e.LoginToken != null);

            if (customer == null && employee == null)
            {
                return Unauthorized("You must login.");
            }

            if (customer != null)
            {
                // Invalidate the login token for customer
                customer.LoginToken = null;
                customer.LoginTokenExpires = null;

                // Set status to indicate logged out
                customer.Status = false;

                _context.Customers.Update(customer);
                await _context.SaveChangesAsync();

                _logger.LogInformation("Customer logged out successfully.");
            }

            if (employee != null)
            {
                // Invalidate the login token for employee
                employee.LoginToken = null;
                employee.LoginTokenExpires = null;

                // Set status to indicate logged out
                employee.Status = false;

                _context.Employees.Update(employee);
                await _context.SaveChangesAsync();

                _logger.LogInformation("Employee logged out successfully.");
            }

            return Ok("Logout successful.");
        }
    }
}
