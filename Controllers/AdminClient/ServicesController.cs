using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using SWPApp.Models;
using System.Collections.Generic;
using System.Linq;

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
                    Description = service.Description.Split('+').Select(line => line.Trim()).ToArray()
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
                Description = service.Description.Split('+').Select(line => line.Trim()).ToArray()
            };

            return Ok(result);
        }

        // PUT: api/Services/5
        [HttpPut("{id}")]
        public IActionResult PutService(string id, Service service)
        {
            if (id != service.ServiceId)
            {
                return BadRequest(new { message = "Service ID mismatch." });
            }

            _context.Entry(service).State = EntityState.Modified;

            try
            {
                _context.SaveChanges();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!ServiceExists(id))
                {
                    return NotFound(new { message = $"Service with ID {id} not found." });
                }
                else
                {
                    throw;
                }
            }

            return NoContent();
        }

        // POST: api/Services
        [HttpPost]
        public ActionResult<Service> PostService(Service service)
        {
            _context.Services.Add(service);
            _context.SaveChanges();

            return CreatedAtAction("GetService", new { id = service.ServiceId }, service);
        }

        // DELETE: api/Services/5
        [HttpDelete("{id}")]
        public IActionResult DeleteService(string id)
        {
            var service = _context.Services.Find(id);
            if (service == null)
            {
                return NotFound(new { message = $"Service with ID {id} not found." });
            }

            _context.Services.Remove(service);
            _context.SaveChanges();

            return NoContent();
        }

        private bool ServiceExists(string id)
        {
            return _context.Services.Any(e => e.ServiceId == id);
        }
    }
}
