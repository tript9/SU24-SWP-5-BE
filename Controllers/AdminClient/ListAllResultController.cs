using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using SWPApp.Models;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace SWPApp.Controllers.AdminClient
{
    [Route("api/[controller]")]
    [ApiController]
    public class ListAllResultController : ControllerBase
    {
        private readonly DiamondAssesmentSystemDBContext _context;

        public ListAllResultController(DiamondAssesmentSystemDBContext context)
        {
            _context = context;
        }

        [HttpGet("list-results")]
        public async Task<ActionResult<IEnumerable<object>>> ListResults([FromQuery] int? employeeId = null)
        {
            IQueryable<Result> query = _context.Results
                .Where(r => r.Request.Status == "Chờ xác nhận" || r.Request.Status == "Kiểm định thành công"||r.Request.Status== "Yêu cầu bị từ chối"||r.Request.Status== "Kim cương đã niêm phong");

            if (employeeId.HasValue)
            {
                query = query.Where(r => r.Request.EmployeeId == employeeId.Value);
            }

            var results = await query
                .Select(r => new
                {
                    r.ResultId,
                    r.DiamondId,
                    r.RequestId,
                    r.DiamondOrigin,
                    r.Shape,
                    r.Measurements,
                    r.CaratWeight,
                    r.Color,
                    r.Clarity,
                    r.Cut,
                    r.Proportions,
                    r.Polish,
                    r.Symmetry,
                    r.Fluorescence,
                    r.Certification,
                    r.Price,
                    r.Comments,
                    RequestStatus = r.Request.Status // Include the status in the response
                })
                .ToListAsync();

            return Ok(results);
        }

        // Accept status = "kiểm định thành công "
        [HttpPut("update-request-status/{requestid}")]
        public async Task<IActionResult> UpdateRequestStatus(int requestid)
        {
            var request = await _context.Requests.FindAsync(requestid);
            if (request == null)
            {
                return NotFound();
            }

            request.Status = "Kiểm định thành công";
            _context.Requests.Update(request);
            await _context.SaveChangesAsync();

            return Ok(request);
        }

        // Update request status to "Yêu cầu bị từ chối"
        [HttpPut("reject-request-status/{requestid}")]
        public async Task<IActionResult> RejectRequestStatus(int requestid)
        {
            var request = await _context.Requests.FindAsync(requestid);
            if (request == null)
            {
                return NotFound();
            }

            request.Status = "Yêu cầu bị từ chối";
            _context.Requests.Update(request);
            await _context.SaveChangesAsync();

            return Ok(request);
        }
    }
}
