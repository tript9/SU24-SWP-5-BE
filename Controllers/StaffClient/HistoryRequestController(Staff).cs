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
        [HttpGet("history/paid")]
        public async Task<IActionResult> GetPaidRequests()
        {
            var requests = await _context.Requests
                .Where(r => r.Status == "Đã thanh toán")
                .Include(r => r.Customer)
                .Include(r => r.Employee)
                .ToListAsync();

            return Ok(requests ?? new List<Request>());
        }

[HttpGet("history/processing")]
public async Task<IActionResult> GetProcessingRequests([FromQuery] int? employeeId)
{
    var query = _context.Requests
        .Where(r => r.Status == "Đã nhận kim cương và đang xử lí" )
        .Include(r => r.Customer)
        .Include(r => r.Employee)
        .AsQueryable();

    if (employeeId.HasValue)
    {
        query = query.Where(r => r.EmployeeId == employeeId.Value);
    }

    var requests = await query.ToListAsync();

    return Ok(requests ?? new List<Request>());
}

    }
}
