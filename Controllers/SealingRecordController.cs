using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using SWPApp.Models;
using System.Threading.Tasks;

namespace SWPApp.Controllers
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

        // Create SealingRecord
        [HttpPost("create-sealing")]
        public async Task<IActionResult> CreateSealing([FromBody] SealingRecord sealingRecord)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            await _context.SealingRecords.AddAsync(sealingRecord);
            await _context.SaveChangesAsync();

            return Ok("Sealing record created successfully");
        }

        // Get SealingRecord by Id
        [HttpGet("get-sealing/{id}")]
        public async Task<IActionResult> GetSealing(int id)
        {
            var sealingRecord = await _context.SealingRecords
                .Include(sr => sr.Request)
                .FirstOrDefaultAsync(sr => sr.SealingId == id);

            if (sealingRecord == null)
            {
                return NotFound("Sealing record not found");
            }

            return Ok(sealingRecord);
        }

        // Update SealingRecord
        [HttpPut("update-sealing/{id}")]
        public async Task<IActionResult> UpdateSealing(int id, [FromBody] SealingRecord sealingRecord)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            var existingSealingRecord = await _context.SealingRecords.FindAsync(id);

            if (existingSealingRecord == null)
            {
                return NotFound("Sealing record not found");
            }

            existingSealingRecord.RequestId = sealingRecord.RequestId;
            existingSealingRecord.SealDate = sealingRecord.SealDate;

            _context.SealingRecords.Update(existingSealingRecord);
            await _context.SaveChangesAsync();

            return Ok("Sealing record updated successfully");
        }

        // Delete SealingRecord
        [HttpDelete("delete-sealing/{id}")]
        public async Task<IActionResult> DeleteSealing(int id)
        {
            var sealingRecord = await _context.SealingRecords.FindAsync(id);

            if (sealingRecord == null)
            {
                return NotFound("Sealing record not found");
            }

            _context.SealingRecords.Remove(sealingRecord);
            await _context.SaveChangesAsync();

            return Ok("Sealing record deleted successfully");
        }
    }
}
