using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using SWPApp.Models;
using System.Threading.Tasks;

namespace SWPApp.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ServiceDetailController : ControllerBase
    {
        private readonly DiamondAssesmentSystemDBContext _context;

        public ServiceDetailController(DiamondAssesmentSystemDBContext context)
        {
            _context = context;
        }

        // Create ServiceDetail
        [HttpPost("create-service-detail")]
        public async Task<IActionResult> CreateServiceDetail([FromBody] ServiceDetail serviceDetail)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            await _context.ServiceDetails.AddAsync(serviceDetail);
            await _context.SaveChangesAsync();

            return Ok("Service detail created successfully");
        }

        // Get ServiceDetail by Id
        [HttpGet("get-service-detail/{id}")]
        public async Task<IActionResult> GetServiceDetail(string id)
        {
            var serviceDetail = await _context.ServiceDetails
                .Include(sd => sd.Service)
                .FirstOrDefaultAsync(sd => sd.ServiceId == id);

            if (serviceDetail == null)
            {
                return NotFound("Service detail not found");
            }

            return Ok(serviceDetail);
        }

        // Update ServiceDetail
        [HttpPut("update-service-detail/{id}")]
        public async Task<IActionResult> UpdateServiceDetail(string id, [FromBody] ServiceDetail serviceDetail)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            var existingServiceDetail = await _context.ServiceDetails.FindAsync(id);

            if (existingServiceDetail == null)
            {
                return NotFound("Service detail not found");
            }

            existingServiceDetail.AssessmentStep = serviceDetail.AssessmentStep;
            existingServiceDetail.StepDetail = serviceDetail.StepDetail;

            _context.ServiceDetails.Update(existingServiceDetail);
            await _context.SaveChangesAsync();

            return Ok("Service detail updated successfully");
        }

        // Delete ServiceDetail
        [HttpDelete("delete-service-detail/{id}")]
        public async Task<IActionResult> DeleteServiceDetail(string id)
        {
            var serviceDetail = await _context.ServiceDetails.FindAsync(id);

            if (serviceDetail == null)
            {
                return NotFound("Service detail not found");
            }

            _context.ServiceDetails.Remove(serviceDetail);
            await _context.SaveChangesAsync();

            return Ok("Service detail deleted successfully");
        }
    }
}
