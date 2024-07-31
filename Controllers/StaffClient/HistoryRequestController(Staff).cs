using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using SWPApp.Models;
using System.Collections.Generic;
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


        [HttpGet("history/processing")]
        public async Task<IActionResult> GetAllProcessingRequests([FromQuery] int? employeeId, [FromQuery] string? serviceId)
        {
            // Get all relevant requests with status "Đã nhận kim cương và đang xử lí"
            var query = _context.Requests
                .Where(r => r.Status == "Đã nhận kim cương và đang xử lí")
                .Include(r => r.Customer)
                .Include(r => r.Employee)
                .AsQueryable();

            // Apply serviceId filter if specified
            if (!string.IsNullOrEmpty(serviceId))
            {
                query = query.Where(r => r.ServiceId == serviceId);
            }

            // Apply employeeId filter if specified
            if (employeeId.HasValue)
            {
                query = query.Where(r => r.EmployeeId == employeeId.Value);
            }

            // Execute query
            var requests = await query.ToListAsync();

            return Ok(requests);
        }

        [HttpGet("HistoryOfRequest(Staff)")]
        public async Task<IActionResult> GetAllHistoryRequests([FromQuery] int? employeeId, [FromQuery] string? serviceId)
        {
            // Get all relevant requests except "Đã nhận kim cương và đang xử lí"
            var query = _context.Requests
                .Where(r => r.Status == "Đã thanh toán" ||
                            r.Status == "Kim cương đã niêm phong")
                .Include(r => r.Customer)
                .Include(r => r.Employee)
                .AsQueryable();

            // Apply serviceId filter if specified
            if (!string.IsNullOrEmpty(serviceId))
            {
                query = query.Where(r => r.ServiceId == serviceId);
            }

            // Apply employeeId filter if specified
            if (employeeId.HasValue)
            {
                query = query.Where(r => r.EmployeeId == employeeId.Value);
            }

            // Execute query
            var requests = await query.ToListAsync();

            return Ok(requests);
        }

    }
}























//using Microsoft.AspNetCore.Mvc;
//using Microsoft.EntityFrameworkCore;
//using SWPApp.Models;
//using System.Collections.Generic;
//using System.Linq;
//using System.Threading.Tasks;

//namespace SWPApp.Controllers.StaffClient
//{
//    [Route("api/[controller]")]
//    [ApiController]
//    public class HistoryRequestController : ControllerBase
//    {
//        private readonly DiamondAssesmentSystemDBContext _context;

//        public HistoryRequestController(DiamondAssesmentSystemDBContext context)
//        {
//            _context = context;
//        }

//        [HttpGet("history/paid")]
//        public async Task<IActionResult> GetPaidRequests([FromQuery] string? serviceId)
//        {
//            var query = _context.Requests
//                .Where(r => r.Status == "Đã thanh toán")
//                .Include(r => r.Customer)
//                .Include(r => r.Employee)
//                .AsQueryable();

//            if (!string.IsNullOrEmpty(serviceId))
//            {
//                query = query.Where(r => r.ServiceId == serviceId);
//            }

//            var requests = await query.ToListAsync();

//            return Ok(requests ?? new List<Request>());
//        }
//        [HttpGet("history/processing")]
//        public async Task<IActionResult> GetProcessingRequests([FromQuery] int? employeeId, [FromQuery] string? serviceId)
//        {
//            var query = _context.Requests
//                .Where(r => r.Status == "Đã nhận kim cương và đang xử lí")
//                .Include(r => r.Customer)
//                .Include(r => r.Employee)
//                .AsQueryable();

//            // Join with Employee to filter where ServiceId in Request matches ServiceId in Employee
//            if (!string.IsNullOrEmpty(serviceId))
//            {
//                query = query
//                    .Where(r => r.ServiceId == serviceId &&
//                                _context.Employees.Any(e => e.ServiceId == serviceId && e.EmployeeId == r.EmployeeId));
//            }

//            // Filter by employeeId if specified
//            if (employeeId.HasValue)
//            {
//                query = query.Where(r => r.EmployeeId == employeeId.Value);
//            }

//            var requests = await query.ToListAsync();

//            return Ok(requests ?? new List<Request>());
//        }



//        [HttpGet("history/sealed")]
//        public async Task<IActionResult> GetSealedRequests([FromQuery] string? serviceId)
//        {
//            var query = _context.Requests
//                .Where(r => r.Status == "Kim cương đã niêm phong")
//                .Include(r => r.Customer)
//                .Include(r => r.Employee)
//                .AsQueryable();

//            if (!string.IsNullOrEmpty(serviceId))
//            {
//                query = query.Where(r => r.ServiceId == serviceId);
//            }

//            var requests = await query.ToListAsync();

//            return Ok(requests ?? new List<Request>());
//        }
//    }
//}
