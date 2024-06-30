using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using SWPApp.Models;
using System.Threading.Tasks;

namespace SWPApp.Controllers.StaffClient
{
    [Route("api/[controller]")]
    [ApiController]
    public class AcceptStatusController : ControllerBase
    {
        private readonly DiamondAssesmentSystemDBContext _context;

        public AcceptStatusController(DiamondAssesmentSystemDBContext context)
        {
            _context = context;
        }

        // Accept Request sau khi nhận kim cương thì staff chuyển status thành đã nhận và đang xử lí
        [HttpPut("Update-status-when-received-diamond/{requestId}")]
        public async Task<IActionResult> UpdateStatusWhenReceivedDiamond(int requestId)
        {
            var request = await _context.Requests.FindAsync(requestId);

            if (request == null)
            {
                return NotFound();
            }

            request.Status = "Đã nhận kim cương và đang xử lí";

            _context.Requests.Update(request);
            await _context.SaveChangesAsync();

            return Ok(request);
        }

        // Sau khi cus nhận kim cương thì staff update status="khách hàng đã nhận kim cương" (ở cuspage)="đã nhận hàng"
        [HttpPut("Update-status-done/{id}")]
        public async Task<IActionResult> StatusDone(int id)
        {
            var request = await _context.Requests.FindAsync(id);

            if (request == null)
            {
                return NotFound();
            }

            request.Status = "Khách hàng đã nhận kim cương";

            _context.Requests.Update(request);
            await _context.SaveChangesAsync();

            return Ok(request);
        }

        // Khách không nhận kim cương sau n ngày
        [HttpPut("Update-status-sealed/{id}")]
        public async Task<IActionResult> UpdateStatusSealed(int id)
        {
            var request = await _context.Requests.FindAsync(id);

            if (request == null)
            {
                return NotFound();
            }

            request.Status = "Kim cương đã niêm phong";

            _context.Requests.Update(request);
            await _context.SaveChangesAsync();

            return Ok(new { message = "Lý do khách không nhận sau n ngày" });
        }
    }
}
