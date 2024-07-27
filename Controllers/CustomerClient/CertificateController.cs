using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using SWPApp.DTO;
using SWPApp.Models;
using System;
using System.Linq;
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

        [HttpGet("generate-certificate-pdf-by-request/{requestId}")]
        public async Task<IActionResult> GenerateCertificatePdfByRequestId(int requestId)
        {
            var result = await _context.Results
                .Include(r => r.Request)
                .FirstOrDefaultAsync(r => r.RequestId == requestId && r.Request.Status == "kiểm định thành công");

            if (result == null)
            {
                return NotFound("Result not found or request status is not 'kiểm định thành công'");
            }

            var certificate = await _context.Certificates
                .Include(c => c.Result)
                .ThenInclude(r => r.Request)
                .FirstOrDefaultAsync(c => c.ResultId == result.ResultId);

            if (certificate == null)
            {
                // Create a new certificate
                certificate = new Certificate
                {
                    ResultId = result.ResultId,
                    IssueDate = DateTime.UtcNow
                };

                _context.Certificates.Add(certificate);
                await _context.SaveChangesAsync();
            }

            var pdfBytes = PdfHelper.CreateCertificatePdf(certificate);

            return File(pdfBytes, "application/pdf", "certificate.pdf");
        }

    }
}
