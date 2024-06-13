using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using SWPApp.Models;
using System.Threading.Tasks;

namespace SWPApp.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class DiamondController : ControllerBase
    {
        private readonly DiamondAssesmentSystemDBContext _context;

        public DiamondController(DiamondAssesmentSystemDBContext context)
        {
            _context = context;
        }

        // Create Diamond
        [HttpPost("create-diamond")]
        public async Task<IActionResult> CreateDiamond([FromBody] Diamond diamond)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            await _context.Diamonds.AddAsync(diamond);
            await _context.SaveChangesAsync();

            return Ok("Diamond created successfully");
        }

        // Get Diamond by Id
        [HttpGet("get-diamond/{id}")]
        public async Task<IActionResult> GetDiamond(int id)
        {
            var diamond = await _context.Diamonds.FindAsync(id);

            if (diamond == null)
            {
                return NotFound("Diamond not found");
            }

            return Ok(diamond);
        }

        // Update Diamond
        [HttpPut("update-diamond/{id}")]
        public async Task<IActionResult> UpdateDiamond(int id, [FromBody] Diamond diamond)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            var existingDiamond = await _context.Diamonds.FindAsync(id);

            if (existingDiamond == null)
            {
                return NotFound("Diamond not found");
            }

            existingDiamond.Shape = diamond.Shape;
            existingDiamond.CaratWeight = diamond.CaratWeight;
            existingDiamond.Color = diamond.Color;
            existingDiamond.Status = diamond.Status;

            _context.Diamonds.Update(existingDiamond);
            await _context.SaveChangesAsync();

            return Ok("Diamond updated successfully");
        }

        // Delete Diamond
        [HttpDelete("delete-diamond/{id}")]
        public async Task<IActionResult> DeleteDiamond(int id)
        {
            var diamond = await _context.Diamonds.FindAsync(id);

            if (diamond == null)
            {
                return NotFound("Diamond not found");
            }

            _context.Diamonds.Remove(diamond);
            await _context.SaveChangesAsync();

            return Ok("Diamond deleted successfully");
        }
    }
}
