using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using SWPApp.Models;
using System;
using System.Linq;
using System.Threading.Tasks;

namespace SWPApp.Controllers.CustomerClient
{
    [Route("api/[controller]")]
    [ApiController]
    public class SealingRecordController : ControllerBase
    {
        private readonly DiamondAssesmentSystemDBContext _context;

        public SealingRecordController(DiamondAssesmentSystemDBContext context)
        {
            _context = context;
        }

        // Generate SealingRecords if conditions are met
        [HttpGet("generate-sealing")]
        public async Task<IActionResult> GenerateSealing()
        {
            // Check for certificates that need a sealing record generated
            var now = DateTime.Now.Date;
            var certificatesToSeal = await _context.Certificates
                .Where(c => c.IssueDate.AddDays(10) <= now)
                .ToListAsync();

            if (!certificatesToSeal.Any())
            {
                return Ok("Chưa đến thời hạn niêm phong.");
            }

            foreach (var certificate in certificatesToSeal)
            {
                var newSealingRecord = new SealingRecord
                {
                    RequestId = certificate.ResultId, // Assuming ResultId maps to RequestId
                    SealDate = now
                };

                _context.SealingRecords.Add(newSealingRecord);

                // Update the status of the associated Request
                var request = await _context.Requests.FindAsync(certificate.ResultId);
                if (request != null)
                {
                    request.Status = "Đã niêm phong";
                    _context.Requests.Update(request);
                }
            }

            await _context.SaveChangesAsync();

            return Ok("Đã niêm phong thành công");
        }
    }
}
