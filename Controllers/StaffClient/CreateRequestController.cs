using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using SWPApp.Models;
using System;
using System.Threading.Tasks;

namespace SWPApp.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class CreateRequestsController : ControllerBase
    {
        private readonly DiamondAssesmentSystemDBContext _context;

        public CreateRequestsController(DiamondAssesmentSystemDBContext context)
        {
            _context = context;
        }

        [HttpPost]
        public async Task<IActionResult> CreateRequest([FromBody] Request request)
        {
            // Kiểm tra ServiceId có tồn tại trong bảng Service không
            var service = await _context.Services.FindAsync(request.ServiceId);
            if (service == null)
            {
                return BadRequest("Invalid ServiceId");
            }

            // Kiểm tra CustomerId có tồn tại trong bảng Customer không
            var customer = await _context.Customers.FindAsync(request.CustomerId);
            if (customer == null)
            {
                return BadRequest("Invalid CustomerId");
            }

            // Cập nhật thông tin Customer            
            customer.PhoneNumber = request.PhoneNumber;
            customer.IDCard = request.IDCard;
            customer.Address = request.Address;

            // Chỉ chèn một số trường cụ thể từ request
            var newRequest = new Request
            {
                CustomerId = request.CustomerId,
                RequestDate = DateTime.Now,
                Email = request.Email,
                PhoneNumber = request.PhoneNumber,
                IDCard = request.IDCard,
                Address = request.Address,
                ServiceType = service.ServiceType, // lấy từ bảng Service
                ServiceId = request.ServiceId,
                Status = "Đã thanh toán" // Hoặc bất kỳ trạng thái mặc định nào
            };

            _context.Requests.Add(newRequest);
            await _context.SaveChangesAsync();

            return CreatedAtAction(nameof(GetRequestById), new { id = newRequest.RequestId }, newRequest);
        }

        [HttpGet("{id}")]
        public async Task<IActionResult> GetRequestById(int id)
        {
            var request = await _context.Requests.FindAsync(id);

            if (request == null)
            {
                return NotFound();
            }

            return Ok(request);
        }
    }
}
