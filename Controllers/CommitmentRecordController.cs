using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using SWPApp.Models;
using System.Threading.Tasks;

namespace SWPApp.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class CommitmentRecordController : ControllerBase
    {
        private readonly DiamondAssesmentSystemDBContext _context;

        public CommitmentRecordController(DiamondAssesmentSystemDBContext context)
        {
            _context = context;
        }

        // Create CommitmentRecord
        [HttpPost("create-commitment")]
        public async Task<IActionResult> CreateCommitment([FromBody] CommitmentRecord commitmentRecord)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            await _context.CommitmentRecords.AddAsync(commitmentRecord);
            await _context.SaveChangesAsync();

            return Ok("Commitment record created successfully");
        }

        // Get CommitmentRecord by Id
        [HttpGet("get-commitment/{id}")]
        public async Task<IActionResult> GetCommitment(int id)
        {
            var commitmentRecord = await _context.CommitmentRecords
                .Include(cr => cr.Request)
                .FirstOrDefaultAsync(cr => cr.RecordId == id);

            if (commitmentRecord == null)
            {
                return NotFound("Commitment record not found");
            }

            return Ok(commitmentRecord);
        }

        // Update CommitmentRecord
        [HttpPut("update-commitment/{id}")]
        public async Task<IActionResult> UpdateCommitment(int id, [FromBody] CommitmentRecord commitmentRecord)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            var existingRecord = await _context.CommitmentRecords.FindAsync(id);

            if (existingRecord == null)
            {
                return NotFound("Commitment record not found");
            }

            existingRecord.RequestId = commitmentRecord.RequestId;
            existingRecord.CommitDate = commitmentRecord.CommitDate;
            existingRecord.Request = commitmentRecord.Request;

            _context.CommitmentRecords.Update(existingRecord);
            await _context.SaveChangesAsync();

            return Ok("Commitment record updated successfully");
        }

        // Delete CommitmentRecord
        [HttpDelete("delete-commitment/{id}")]
        public async Task<IActionResult> DeleteCommitment(int id)
        {
            var commitmentRecord = await _context.CommitmentRecords.FindAsync(id);

            if (commitmentRecord == null)
            {
                return NotFound("Commitment record not found");
            }

            _context.CommitmentRecords.Remove(commitmentRecord);
            await _context.SaveChangesAsync();

            return Ok("Commitment record deleted successfully");
        }
    }
}
