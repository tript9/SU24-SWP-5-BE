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

        // Danh sách đơn có trạng thái là "Đã thanh toán"
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

        // Danh sách đơn có trạng thái là "Đã nhận kim cương và đang xử lí"
        [HttpGet("history/processing")]
        public async Task<IActionResult> GetProcessingRequests()
        {
            var requests = await _context.Requests
                .Where(r => r.Status == "Đã nhận kim cương và đang xử lí")
                .Include(r => r.Customer)
                .Include(r => r.Employee)
                .ToListAsync();

            return Ok(requests ?? new List<Request>());
        }
    }
}
