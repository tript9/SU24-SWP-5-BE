using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using SWPApp.Models;
using System;
using System.Threading.Tasks;

namespace SWPApp.Controllers.CustomerClient
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
                EmployeeId = request.EmployeeId, // Assign EmployeeId from request (may be null)
                RequestDate = DateTime.Now,
                Email = request.Email,
                PhoneNumber = request.PhoneNumber,
                IDCard = request.IDCard,
                Address = request.Address,
                ServiceId = request.ServiceId,
                Status = "Chờ thanh toán" // Hoặc bất kỳ trạng thái mặc định nào
            };

            _context.Requests.Add(newRequest);
            await _context.SaveChangesAsync();

            // Xác định mô tả gói dịch vụ dựa trên ServiceId
            string packageDescription = service.ServiceId switch
            {
                "1" => "Gói cơ bản",
                "2" => "Gói nâng cao",
                "3" => "Gói cao cấp",
                _ => "Gói không xác định"
            };

            return CreatedAtAction(nameof(GetRequestById), new { id = newRequest.RequestId }, new
            {
                Request = newRequest,
                PackageDescription = packageDescription
            });
        }

        //Search By RequestId
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
        //List all request if (Staff are login)


        
    }
}
