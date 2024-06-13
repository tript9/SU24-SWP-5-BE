using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using SWPApp.DTO;
using SWPApp.Models;
using System.Threading.Tasks;

namespace SWPApp.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class CertificateController : ControllerBase
    {
        private readonly DiamondAssesmentSystemDBContext _context;

        public CertificateController(DiamondAssesmentSystemDBContext context)
        {
            _context = context;
        }

        [HttpPost("create-certificate")]
        public async Task<IActionResult> CreateCertificate([FromBody] CertificateDTO certificateDTO)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            var certificate = new Certificate
            {
                ResultId = certificateDTO.ResultId,
                IssueDate = certificateDTO.IssueDate
                // Map other properties
            };

            await _context.Certificates.AddAsync(certificate);
            await _context.SaveChangesAsync();

            return Ok("Certificate created successfully");
        }

        [HttpGet("get-certificate/{certificateId}")]
        public async Task<IActionResult> GetCertificate(int certificateId)
        {
            var certificate = await _context.Certificates
                .FirstOrDefaultAsync(c => c.CertificateId == certificateId);

            if (certificate == null)
            {
                return NotFound("Certificate not found");
            }

            return Ok(certificate);
        }

        [HttpPut("update-certificate/{certificateId}")]
        public async Task<IActionResult> UpdateCertificate(int certificateId, [FromBody] CertificateDTO certificateDTO)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            var certificate = await _context.Certificates.FindAsync(certificateId);

            if (certificate == null)
            {
                return NotFound("Certificate not found");
            }

            certificate.ResultId = certificateDTO.ResultId;
            certificate.IssueDate = certificateDTO.IssueDate;
            // Update other properties

            _context.Certificates.Update(certificate);
            await _context.SaveChangesAsync();

            return Ok("Certificate updated successfully");
        }

        [HttpDelete("delete-certificate/{certificateId}")]
        public async Task<IActionResult> DeleteCertificate(int certificateId)
        {
            var certificate = await _context.Certificates.FindAsync(certificateId);

            if (certificate == null)
            {
                return NotFound("Certificate not found");
            }

            _context.Certificates.Remove(certificate);
            await _context.SaveChangesAsync();

            return Ok("Certificate deleted successfully");
        }
    }
}
