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
        //Tôi đã thanh toán 
        [HttpPost]
        public async Task<IActionResult> CreateRequestDetail([FromBody] RequestDetail requestDetail)
        {
            // Kiểm tra RequestId có tồn tại trong bảng Request không
            var request = await _context.Requests.FindAsync(requestDetail.RequestId);
          
            if (request == null)
            {
                return BadRequest("Invalid RequestId");
            }

            // Lấy ServiceId từ bảng Request
            var serviceId = request.ServiceId;

            // Kiểm tra ServiceId có tồn tại trong bảng Service không
            var service = await _context.Services.FindAsync(serviceId);
            if (service == null)
            {
                return BadRequest("Invalid ServiceId");
            }

            // Tạo RequestDetail mới với các giá trị mặc định
            var newRequestDetail = new RequestDetail
            {
                RequestId = requestDetail.RequestId,
                ServiceId = serviceId,
                PaymentStatus = true, // Thiết lập mặc định thành true
                PaymentMethod = requestDetail.PaymentMethod,
                Request = request,
                Service = service
            };

            _context.RequestDetails.Add(newRequestDetail);
            await _context.SaveChangesAsync();

            return CreatedAtAction(nameof(GetRequestDetailById), new { id = newRequestDetail.RequestId }, newRequestDetail);
        }

        [HttpGet("{id}")]
        public async Task<IActionResult> GetRequestDetailById(int id)
        {
            var requestDetail = await _context.RequestDetails
                .Include(rd => rd.Request)
                .Include(rd => rd.Service)
                .FirstOrDefaultAsync(rd => rd.RequestId == id);

            if (requestDetail == null)
            {
                return NotFound();
            }

            return Ok(requestDetail);
        }
    }
}
