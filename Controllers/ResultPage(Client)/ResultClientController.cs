using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using SWPApp.Models;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace SWPApp.Controllers.ResultPage_Client_
{
    [Route("api/[controller]")]
    [ApiController]
    public class ResultClientController : ControllerBase
    {
        private readonly DiamondAssesmentSystemDBContext _context;

        public ResultClientController(DiamondAssesmentSystemDBContext context)
        {
            _context = context;
        }

        [HttpGet]
        public async Task<ActionResult<IEnumerable<Result>>> GetResults()
        {
            return await _context.Results.ToListAsync();
        }

        [HttpGet("{id}")]
        public async Task<ActionResult<Result>> GetResult(int id)
        {
            var result = await _context.Results.FindAsync(id);

            if (result == null)
            {
                return NotFound();
            }

            return result;
        }

        [HttpGet("LinkCertificate/{id}")]
        public ActionResult<string> LinkCertificate(int id)
        {
            var result = _context.Results.Find(id);
            if (result == null)
            {
                return NotFound();
            }

            // Assuming the certificate link is stored in the database, adjust as necessary
            string certificateLink = $"https://certificates.example.com/{result.ResultId}";
            return Ok(certificateLink);
        }

        [HttpGet("Chatbox")]
        public ActionResult<string> Chatbox()
        {
            // This is a placeholder implementation for the chatbox feature
            // In a real application, this would likely be more complex
            return Ok("Chatbox is available here.");
        }

        [HttpPost("Feedback")]
        public IActionResult Feedback()
        {
            // Redirect to feedback page (assuming it is a separate page or service)
            return Redirect("https://feedback.example.com");
        }
    }
}
