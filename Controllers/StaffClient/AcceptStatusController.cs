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

        [HttpPut("Update-status-when-received-diamond/{requestId}/{employeeId}")]
        public async Task<IActionResult> UpdateStatusWhenReceivedDiamond(int requestId, int employeeId)
        {
            var request = await _context.Requests.FindAsync(requestId);
            if (request == null)
            {
                return NotFound();
            }

            var employee = await _context.Employees.FindAsync(employeeId);
            if (employee == null)
            {
                return BadRequest($"Employee with ID {employeeId} does not exist.");
            }

            request.Status = "Đã nhận kim cương và đang xử lí";
            request.EmployeeId = employeeId;

            _context.Requests.Update(request);
            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateException ex)
            {
                // Log the exception details (ex) as needed
                return StatusCode(500, "An error occurred while updating the request.");
            }

            return Ok(request);
        }


        // Sau khi cus nhận kim cương thì staff update status="khách hàng đã nhận kim cương" (ở cuspage)="đã nhận hàng"
        [HttpPut("Update-status-done/{requestid}/{employeeId}")]
        public async Task<IActionResult> StatusDone(int requestid, int employeeId)
        {
            var request = await _context.Requests.FindAsync(requestid);

            if (request == null)
            {
                return NotFound();
            }

            request.Status = "Khách hàng đã nhận kim cương";
            request.EmployeeId = employeeId; // Assuming there's an EmployeeId field in the Request model

            _context.Requests.Update(request);
            await _context.SaveChangesAsync();

            return Ok(request);
        }

        // Khách không nhận kim cương sau n ngày
        [HttpPut("Update-status-sealed/{requestid}/{employeeId}")]
        public async Task<IActionResult> UpdateStatusSealed(int requestid, int employeeId)
        {
            var request = await _context.Requests.FindAsync(requestid);

            if (request == null)
            {
                return NotFound();
            }

            if (request.Status == "Kiểm định thành công")
            {
                request.Status = "Kim cương đã niêm phong";
                request.EmployeeId = employeeId; // Assuming there's an EmployeeId field in the Request model

                _context.Requests.Update(request);
                await _context.SaveChangesAsync();

                return Ok(request);
            }

            return BadRequest(new { message = "Trạng thái yêu cầu không phải 'kiểm định thành công'" });
        }

    }
}
