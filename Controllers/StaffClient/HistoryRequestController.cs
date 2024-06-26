using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using SWPApp.Models;
using System.Linq;
using System.Threading.Tasks;

namespace SWPApp.Controllers.StaffClient
{
    [Route("api/[controller]")]
    [ApiController]
    public class HistoryRequestController : ControllerBase
    {
        private readonly DiamondAssesmentSystemDBContext _context;

        public HistoryRequestController(DiamondAssesmentSystemDBContext context)
        {
            _context = context;
        }

        // Retrieve the specific request for the customer that is marked as "Đã thanh toán"
        [HttpGet("history/{customerId}/{requestId}")]
        public async Task<IActionResult> GetRequestHistory(int customerId, int requestId)
        {
            // Retrieve the specific request for the customer that is marked as "Đã thanh toán"
            var request = await _context.Requests
                .Where(r => r.CustomerId == customerId && r.RequestId == requestId && r.Status == "Đã thanh toán")
                .Include(r => r.Customer)
                .FirstOrDefaultAsync(); // Use FirstOrDefaultAsync to get a single record

            if (request != null)
            {
                return Ok(request);
            }
            else
            {
                return NotFound("No request with the specified ID and payment status found for this customer.");
            }
        }

        // Retrieve the specific request for the customer that is marked as "Đã nhận kim cương và đang xử lí"
        [HttpGet("history/{customerId}/{requestId}/processing")]
        public async Task<IActionResult> GetProcessingRequestHistory(int customerId, int requestId)
        {
            // Retrieve the specific request for the customer that is marked as "Đã nhận kim cương và đang xử lí"
            var request = await _context.Requests
                .Where(r => r.CustomerId == customerId && r.RequestId == requestId && r.Status == "Đã nhận kim cương và đang xử lí")
                .Include(r => r.Customer)
                .FirstOrDefaultAsync(); // Use FirstOrDefaultAsync to get a single record

            if (request != null)
            {
                return Ok(request);
            }
            else
            {
                return NotFound("No request with the specified ID and status found for this customer.");
            }
        }
    }
}
