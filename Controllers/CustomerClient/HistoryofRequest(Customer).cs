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
                .Where(c => c.CustomerId == customerId)
                .FirstOrDefaultAsync();

            if (customer == null)
            {
                return BadRequest("Invalid CustomerId or Customer is not logged in.");
            }

            var requests = await _context.Requests
                .Where(r => r.CustomerId == customerId)
                .ToListAsync();

            var requestsDto = requests.Select(r => new
            {
                r.RequestId,
                r.CustomerId,
                r.EmployeeId,
                r.RequestDate,
                r.Email,
                r.PhoneNumber,
                r.IDCard,
                r.Address,
                r.ServiceId,
                r.Status
            }).ToList();

            // Return an empty array if no requests are found
            return Ok(requestsDto.Any() ? requestsDto : new List<object>());
        }

        [HttpPut("CustomerDiamondReceived/{requestId}/{customerId}")]
        public async Task<IActionResult> UpdateStatusToDiamondReceived(int requestId, int customerId)
        {
            var request = await _context.Requests
                .Where(r => r.RequestId == requestId && r.CustomerId == customerId)
                .FirstOrDefaultAsync();

            if (request == null)
            {
                return NotFound("Request not found or does not belong to the specified customer.");
            }

            request.Status = "Đã nhận kim cương";

            _context.Requests.Update(request);
            await _context.SaveChangesAsync();

            return Ok(request);
        }
    }
}
