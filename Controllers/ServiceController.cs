using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using SWPApp.Models;
using System.Threading.Tasks;

namespace SWPApp.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ServiceController : ControllerBase
    {
        private readonly DiamondAssesmentSystemDBContext _context;

        public ServiceController(DiamondAssesmentSystemDBContext context)
        {
            _context = context;
        }

        // Create Service
        [HttpPost("create-service")]
        public async Task<IActionResult> CreateService([FromBody] Service service)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            await _context.Services.AddAsync(service);
            await _context.SaveChangesAsync();

            return Ok("Service created successfully");
        }

        // Get Service by Id
        [HttpGet("get-service/{id}")]
        public async Task<IActionResult> GetService(string id)
        {
            var service = await _context.Services.FindAsync(id);

            if (service == null)
            {
                return NotFound("Service not found");
            }

            return Ok(service);
        }

        // Update Service
        [HttpPut("update-service/{id}")]
        public async Task<IActionResult> UpdateService(string id, [FromBody] Service service)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            var existingService = await _context.Services.FindAsync(id);

            if (existingService == null)
            {
                return NotFound("Service not found");
            }

            existingService.ServiceType = service.ServiceType;
            existingService.Description = service.Description;
            existingService.ServicePrice = service.ServicePrice;
            existingService.Duration = service.Duration;

            _context.Services.Update(existingService);
            await _context.SaveChangesAsync();

            return Ok("Service updated successfully");
        }

        // Delete Service
        [HttpDelete("delete-service/{id}")]
        public async Task<IActionResult> DeleteService(string id)
        {
            var service = await _context.Services.FindAsync(id);

            if (service == null)
            {
                return NotFound("Service not found");
            }

            _context.Services.Remove(service);
            await _context.SaveChangesAsync();

            return Ok("Service deleted successfully");
        }
    }
}
