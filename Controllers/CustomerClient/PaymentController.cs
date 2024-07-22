using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using SWPApp.Models;
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
        private readonly DiamondAssesmentSystemDBContext _context;

        public PaymentController(DiamondAssesmentSystemDBContext context)
        {
            _context = context;
        }

        // Tôi đã thanh toán
        [HttpPost("UpdatePaymentStatus")]
        public async Task<IActionResult> UpdatePaymentStatus([FromBody] PaymentDTO paymentDto)
        {
            var existingRequest = await _context.Requests.FindAsync(paymentDto.RequestId);

            if (existingRequest == null)
            {
                return BadRequest("Invalid RequestId");
            }

            // Assuming ServiceId is always valid and exists
            var service = await _context.Services.FindAsync(existingRequest.ServiceId);

            // Cập nhật trạng thái của Request thành "Đã thanh toán"
            existingRequest.Status = "Đã thanh toán";
            await _context.SaveChangesAsync();

            // Xác định mô tả gói dịch vụ dựa trên ServiceId
            string packageDescription = existingRequest.ServiceId switch
            {
                "1" => "Gói cơ bản",
                "2" => "Gói nâng cao",
                "3" => "Gói cao cấp",
                _ => "Gói không xác định"
            };

            return Ok(new
            {
                Request = existingRequest,
                PackageDescription = packageDescription
            });
        }
    }
}