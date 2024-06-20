using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using SWPApp.Models;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;

namespace SWPApp.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class UpdatePriceController : ControllerBase
    {
        private readonly DiamondAssesmentSystemDBContext _context;
        private readonly ILogger<UpdatePriceController> _logger;

        public UpdatePriceController(DiamondAssesmentSystemDBContext context, ILogger<UpdatePriceController> logger)
        {
            _context = context;
            _logger = logger;
        }

        [HttpPut("{serviceId}")]
        public async Task<IActionResult> UpdateServicePrice(string serviceId, [FromBody] decimal newPrice)
        {
            // Retrieve the employee by LoginToken and LoginTokenExpires
            var employee = await _context.Employees
                .FirstOrDefaultAsync(e => e.LoginToken != null && e.LoginTokenExpires != null);
            if (employee == null)
            {
                return BadRequest("User is not authorized to update the price.");
            }

            // Retrieve the service by serviceId
            var service = await _context.Services.FirstOrDefaultAsync(s => s.ServiceId == serviceId);
            if (service == null)
            {
                return NotFound("Service not found.");
            }

            // Update the service price
            service.ServicePrice = newPrice;

            // Mark the ServicePrice property as modified
            _context.Entry(service).Property(s => s.ServicePrice).IsModified = true;
            await _context.SaveChangesAsync();

            return Ok("Service price updated successfully.");
        }
    }
}
