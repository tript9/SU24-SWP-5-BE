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
        private readonly DiamondAssesmentSystemDBContext _context;

        public PaymentController(DiamondAssesmentSystemDBContext context)
        {
            _context = context;
        }

        // Tôi đã thanh toán
        [HttpPost("UpdatePaymentStatus")]
        public async Task<IActionResult> UpdatePaymentStatus([FromBody] Request request)
        {
            // Kiểm tra RequestId có tồn tại trong bảng Request không
            var existingRequest = await _context.Requests.FindAsync(request.RequestId);

            if (existingRequest == null)
            {
                return BadRequest("Invalid RequestId");
            }

            // Lấy ServiceId từ bảng Request
            var serviceId = existingRequest.ServiceId;

            // Kiểm tra ServiceId có tồn tại trong bảng Service không
            var service = await _context.Services.FindAsync(serviceId);
            if (service == null)
            {
                return BadRequest("Invalid ServiceId");
            }

            // Cập nhật trạng thái của Request thành "Đã thanh toán"
            existingRequest.Status = "Đã thanh toán";
            await _context.SaveChangesAsync();

            // Xác định mô tả gói dịch vụ dựa trên ServiceId
            string packageDescription = serviceId switch
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
