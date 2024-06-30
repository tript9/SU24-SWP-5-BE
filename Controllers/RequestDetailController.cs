using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using SWPApp.DTO;
using SWPApp.Models;
using System.Threading.Tasks;

namespace SWPApp.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class RequestDetailController : ControllerBase
    {
        private readonly DiamondAssesmentSystemDBContext _context;

        public RequestDetailController(DiamondAssesmentSystemDBContext context)
        {
            _context = context;
        }

        [HttpPost("create-request-detail")]
        public async Task<IActionResult> CreateRequestDetail([FromBody] RequestDetailDTO requestDetailDTO)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            var request = await _context.Requests.FindAsync(requestDetailDTO.RequestId);

            if (request == null)
            {
                return BadRequest("Invalid RequestId");
            }

            var requestDetail = new RequestDetail
            {
                RequestId = requestDetailDTO.RequestId,
                ServiceId = requestDetailDTO.ServiceId,                
                PaymentMethod = requestDetailDTO.PaymentMethod
            };

            await _context.RequestDetails.AddAsync(requestDetail);
            await _context.SaveChangesAsync();

            return Ok("Request detail created successfully");
        }

        [HttpGet("get-request-detail/{requestId}")]
        public async Task<IActionResult> GetRequestDetail(int requestId)
        {
            var requestDetail = await _context.RequestDetails
                .Include(rd => rd.Request)
                .Include(rd => rd.Service)
                .FirstOrDefaultAsync(rd => rd.RequestId == requestId);

            if (requestDetail == null)
            {
                return NotFound("Request detail not found");
            }

            return Ok(requestDetail);
        }

        [HttpPut("update-request-detail/{requestId}")]
        public async Task<IActionResult> UpdateRequestDetail(int requestId, [FromBody] RequestDetailDTO requestDetailDTO)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            var requestDetail = await _context.RequestDetails.FindAsync(requestId);

            if (requestDetail == null)
            {
                return NotFound("Request detail not found");
            }

            requestDetail.ServiceId = requestDetailDTO.ServiceId;          
            requestDetail.PaymentMethod = requestDetailDTO.PaymentMethod;

            _context.RequestDetails.Update(requestDetail);
            await _context.SaveChangesAsync();

            return Ok("Request detail updated successfully");
        }

        [HttpDelete("delete-request-detail/{requestId}")]
        public async Task<IActionResult> DeleteRequestDetail(int requestId)
        {
            var requestDetail = await _context.RequestDetails.FindAsync(requestId);

            if (requestDetail == null)
            {
                return NotFound("Request detail not found");
            }

            _context.RequestDetails.Remove(requestDetail);
            await _context.SaveChangesAsync();

            return Ok("Request detail deleted successfully");
        }
    }
}
