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
        public class CertificateDTO
        {
            public int ResultId { get; set; }
            public DateTime IssueDate { get; set; }
            
        }

        private readonly DiamondAssesmentSystemDBContext _context;

        public CertificateController(DiamondAssesmentSystemDBContext context)
        {
            _context = context;
        }



        //Sau khi status="kiểm định thành công, tự động xuất certificate
        [HttpPost("create-certificate")]
        public async Task<IActionResult> CreateCertificate([FromBody] CertificateDTO certificateDTO)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            // Check if the Result exists and has the correct status
            var result = await _context.Results
                .Include(r => r.Request)
                .FirstOrDefaultAsync(r => r.ResultId == certificateDTO.ResultId && r.Request.Status == "kiểm định thành công");

            if (result == null)
            {
                return NotFound("Result not found or request status is not 'kiểm định thành công'");
            }

            // Create the certificate
            var certificate = new Certificate
            {
                ResultId = certificateDTO.ResultId,
                IssueDate = certificateDTO.IssueDate,
                Result = result
                // Map other properties if needed
            };

            await _context.Certificates.AddAsync(certificate);
            await _context.SaveChangesAsync();

            // Create a response DTO
            var response = new Certificate
            {
                CertificateId = certificate.CertificateId,
                ResultId = certificate.ResultId,
                IssueDate = certificate.IssueDate,
                
            };

            return Ok(response);
        }


        [HttpGet("get-certificate/{certificateId}")]
        public async Task<IActionResult> GetCertificate(int certificateId)
        {
            var certificate = await _context.Certificates
                .Include(c => c.Result)
                .ThenInclude(r => r.Request)
                .FirstOrDefaultAsync(c => c.CertificateId == certificateId && c.Result.Request.Status == "kiểm định thành công");

            if (certificate == null)
            {
                return NotFound("Certificate not found or request status is not 'kiểm định thành công'");
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
