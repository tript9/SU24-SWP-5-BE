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
                .Where(r => r.Employee.Role == 0)
                .ToListAsync();

            if (requests != null && requests.Any())
            {
                return Ok(requests);
            }
            else
            {
                return NotFound("Không tìm thấy yêu cầu với trạng thái Đã thanh toán và vai trò nhân viên được chỉ định.");
            }
        }

        // Danh sách đơn có trạng thái là "Đã nhận kim cương và đang xử lí"
        [HttpGet("history/processing")]
        public async Task<IActionResult> GetProcessingRequests()
        {
            var requests = await _context.Requests
                .Where(r => r.Status == "Đã nhận kim cương và đang xử lí")
                .Include(r => r.Customer)
                .Include(r => r.Employee)
                .Where(r => r.Employee.Role == 0)
                .ToListAsync();

            if (requests != null && requests.Any())
            {
                return Ok(requests);
            }
            else
            {
                return NotFound("Không tìm thấy yêu cầu với trạng thái Đã nhận kim cương và đang xử lí và vai trò nhân viên được chỉ định.");
            }
        }
    }
}
