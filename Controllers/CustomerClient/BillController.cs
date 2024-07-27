using Microsoft.AspNetCore.Mvc;
using SWPApp.Models;
using System.Linq;

namespace SWPApp.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class BillsController : ControllerBase
    {
        private readonly DiamondAssesmentSystemDBContext _context;

        public BillsController(DiamondAssesmentSystemDBContext context)
        {
            _context = context;
        }

        public class BillModel
        {
            public int RequestId { get; set; } // This will act as BillNumber
            public string CustomerName { get; set; }
            public string Email { get; set; }
            public string ServiceType { get; set; }
            public decimal ServicePrice { get; set; }
            public string Status { get; set; }
      
        }

        [HttpGet("{customerId}")]
        public ActionResult<BillModel> GetByCustomerId(int customerId)
        {
            // Tìm kiếm yêu cầu dựa trên CustomerId
            var request = _context.Requests.FirstOrDefault(r => r.CustomerId == customerId);
            if (request == null)
            {
                return NotFound();
            }

            // Tìm kiếm khách hàng dựa trên CustomerId
            var customer = _context.Customers.FirstOrDefault(c => c.CustomerId == customerId);
            if (customer == null)
            {
                return NotFound();
            }

            // Tìm kiếm dịch vụ dựa trên ServiceId
            var service = _context.Services.FirstOrDefault(s => s.ServiceId == request.ServiceId);
            if (service == null)
            {
                return NotFound();
            }

            // Tạo đối tượng BillModel và trả về
            var result = new BillModel
            {
                RequestId = request.RequestId,
                CustomerName = customer.CustomerName,
                Email = customer.Email,
                ServiceType = service.ServiceType,
                ServicePrice = service.ServicePrice,
                Status = request.Status,             
            };

            return Ok(result);
        }
    }
}
