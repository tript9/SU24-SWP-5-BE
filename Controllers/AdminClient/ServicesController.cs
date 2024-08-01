using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using SWPApp.Models;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace SWPApp.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ServicesController : ControllerBase
    {
        private readonly DiamondAssesmentSystemDBContext _context;

        public ServicesController(DiamondAssesmentSystemDBContext context)
        {
            _context = context;
        }

        public class UpdateServiceModel
        {
            public string ServiceType { get; set; }
            public string Description { get; set; }
            public int ServicePrice { get; set; }
            public int Duration { get; set; }
        }

        // GET: api/Services
        [HttpGet]
        public ActionResult<IEnumerable<object>> GetServices()
        {
            var services = _context.Services
                .AsEnumerable()
                .Select(service => new
                {
                    service.ServiceId,
                    service.ServiceType,
                    service.Description,
                    service.ServicePrice,
                    service.Duration
                })
                .ToList();

            return Ok(services);
        }

        // GET: api/Services/5
        [HttpGet("{id}")]
        public ActionResult<object> GetService(string id)
        {
            var service = _context.Services.Find(id);

            if (service == null)
            {
                return NotFound(new { message = $"Service with ID {id} not found." });
            }

            var result = new
            {
                service.ServiceId,
                service.ServiceType,
                service.Description,
                service.ServicePrice,
                service.Duration
            };

            return Ok(result);
        }

       
        // PUT: api/Services/5
        [HttpPut("{id}")]
        public async Task<IActionResult> PutService(string id, [FromBody] UpdateServiceModel updatedService)
        {
            if (updatedService == null)
            {
                return BadRequest(new { message = "Service data is required." });
            }

            var existingService = await _context.Services.Include(s => s.Employees).FirstOrDefaultAsync(s => s.ServiceId == id);
            if (existingService == null)
            {
                return NotFound(new { message = $"Service with ID {id} not found." });
            }

            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            // Update fields only if they are not "string", null or empty for strings, not 0 for integers
            if (!string.IsNullOrEmpty(updatedService.ServiceType) && updatedService.ServiceType != "string")
            {
                existingService.ServiceType = updatedService.ServiceType;
            }

            if (!string.IsNullOrEmpty(updatedService.Description) && updatedService.Description != "string")
            {
                existingService.Description = updatedService.Description;
            }

            if (updatedService.ServicePrice >= 0)
            {
                existingService.ServicePrice = updatedService.ServicePrice;
            }

            if (updatedService.Duration != 0)
            {
                existingService.Duration = updatedService.Duration;
            }

            _context.Entry(existingService).State = EntityState.Modified;

            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!ServiceExists(id))
                {
                    return NotFound(new { message = $"Service with ID {id} not found." });
                }
                else
                {
                    return StatusCode(500, new { message = "An error occurred while updating the service." });
                }
            }

            var result = new
            {
                existingService.ServiceType,
                existingService.Description,
                existingService.ServicePrice,
                existingService.Duration
            };

            return Ok(result);
        }


        // POST: api/Services
        [HttpPost]
        public async Task<ActionResult<Service>> PostService([FromBody] Service service)
        {
            if (service == null)
            {
                return BadRequest(new { message = "Service data is required." });
            }

            _context.Services.Add(service);
            await _context.SaveChangesAsync();

            return CreatedAtAction("GetService", new { id = service.ServiceId }, service);
        }

        // DELETE: api/Services/5
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteService(string id)
        {
            var service = await _context.Services.FindAsync(id);
            if (service == null)
            {
                return NotFound(new { message = $"Service with ID {id} not found." });
            }

            _context.Services.Remove(service);
            await _context.SaveChangesAsync();

            return NoContent();
        }

        private bool ServiceExists(string id)
        {
            return _context.Services.Any(e => e.ServiceId == id);
        }
    }
}
