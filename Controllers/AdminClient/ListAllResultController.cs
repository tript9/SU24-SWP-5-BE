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

        [HttpGet("list-results-Admin")]
        public async Task<ActionResult<IEnumerable<object>>> ListResults()
        {
            IQueryable<Result> query = _context.Results
                .Where(r => r.Request.Status == "Chờ xác nhận" ||
                            r.Request.Status == "Kiểm định thành công" ||
                            r.Request.Status == "Yêu cầu bị từ chối" ||
                            r.Request.Status == "Kim cương đã niêm phong"||
                            r.Request.Status== "Đã nhận kim cương")                           
                            ;

            var results = await query
                .Select(r => new
                {
                    r.Request.EmployeeId, // Include EmployeeId in the response
                    r.Request.Employee.ServiceId, // Include ServiceId in the response
                    r.ResultId,
                    r.DiamondId,
                    r.RequestId,
                    DiamondOrigin = r.DiamondOrigin ?? "none",
                    Shape = r.Shape ?? "none",
                    Measurements = r.Measurements ?? "none",
                    CaratWeight = r.CaratWeight ?? 0,
                    Color = r.Color ?? "none",
                    Clarity = r.Clarity ?? "none",
                    Cut = r.Cut ?? "none",
                    Proportions = r.Proportions ?? "none",
                    Polish = r.Polish ?? "none",
                    Symmetry = r.Symmetry ?? "none",
                    Fluorescence = r.Fluorescence ?? "none",
                    Certification = r.Certification ?? "none",
                    Price = r.Price ?? 0,
                    Comments = r.Comments ?? "none",
                    RequestStatus = r.Request.Status // Include the status in the response
                })
                .ToListAsync();

            return Ok(results);
        }

        // Accept status = "Kiểm định thành công"
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
