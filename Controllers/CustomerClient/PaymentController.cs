using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using SWPApp.Models;
using System;
using System.Threading.Tasks;

namespace SWPApp.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class PaymentController : ControllerBase
    {
        public class PaymentDTO
        {
            public int RequestId { get; set; }
            public int CustomerId { get; set; }
        }

        public class BillModel
        {
            public int RequestId { get; set; } // This will act as BillNumber
            public string CustomerName { get; set; }
            public DateTime? RequestDate { get; set; }
            public string ServiceType { get; set; }
            public int ServicePrice { get; set; }
            public string Status { get; set; }
        }

        private readonly DiamondAssesmentSystemDBContext _context;

        public PaymentController(DiamondAssesmentSystemDBContext context)
        {
            _context = context;
        }

        // Tôi đã thanh toán
        [HttpPost("UpdatePaymentStatus")]
        public async Task<IActionResult> UpdatePaymentStatus([FromBody] PaymentDTO paymentDto)
        {
            var existingRequest = await _context.Requests
                .Include(r => r.Customer)
                .FirstOrDefaultAsync(r => r.RequestId == paymentDto.RequestId);

            if (existingRequest == null)
            {
                return BadRequest("Invalid RequestId");
            }

            var service = await _context.Services
                .FirstOrDefaultAsync(s => s.ServiceId == existingRequest.ServiceId);

            if (service == null)
            {
                return BadRequest("Invalid ServiceId");
            }

            // Update the status of the request to "Đã thanh toán"
            existingRequest.Status = "Đã thanh toán";
            await _context.SaveChangesAsync();

            // Create the BillModel
            var bill = new BillModel
            {
                RequestId = existingRequest.RequestId,
                CustomerName = existingRequest.Customer.CustomerName,
                RequestDate = existingRequest.RequestDate,
                ServiceType = service.ServiceType,
                ServicePrice = service.ServicePrice,
                Status = existingRequest.Status
            };

            return Ok(bill);
        }
    }
}
