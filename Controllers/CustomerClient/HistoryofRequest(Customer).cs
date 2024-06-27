using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using SWPApp.Models;
using System.Linq;
using System.Threading.Tasks;

namespace SWPApp.Controllers.CustomerClient
{
    [Route("api/[controller]")]
    [ApiController]
    public class HistoryofRequestController : ControllerBase
    {
        private readonly DiamondAssesmentSystemDBContext _context;

        public HistoryofRequestController(DiamondAssesmentSystemDBContext context)
        {
            _context = context;
        }

        [HttpGet("ByCustomer/{customerId}")]
        public async Task<IActionResult> GetRequestsByCustomerId(int customerId)
        {
            var customer = await _context.Customers
                .Where(c => c.CustomerId == customerId && c.LoginToken != null)
                .FirstOrDefaultAsync();

            if (customer == null)
            {
                return BadRequest("Invalid CustomerId or Customer is not logged in.");
            }

            var requests = await _context.Requests
                .Where(r => r.CustomerId == customerId)
                .ToListAsync();

            if (requests == null || !requests.Any())
            {
                return NotFound("No requests found for this customer.");
            }

            return Ok(requests);
        }
    }
}
