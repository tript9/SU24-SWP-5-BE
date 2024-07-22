using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using SWPApp.Models;
using System.ComponentModel.DataAnnotations;
using System.Threading.Tasks;

namespace SWPApp.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ResultController : ControllerBase
    {
        public class ResultDTO
        {
            [Key]
            public int? ResultId { get; set; }
            public int DiamondId { get; set; }
            public int RequestId { get; set; }
            public string DiamondOrigin { get; set; }
            public string Shape { get; set; }
            public string Measurements { get; set; }
            public decimal CaratWeight { get; set; }
            public string Color { get; set; }
            public string Clarity { get; set; }
            public string Cut { get; set; }
            public string Proportions { get; set; }
            public string Polish { get; set; }
            public string Symmetry { get; set; }
            public string Fluorescence { get; set; }
        }

        private readonly DiamondAssesmentSystemDBContext _context;

        public ResultController(DiamondAssesmentSystemDBContext context)
        {
            _context = context;
        }

        [HttpPut("update/{resultId}")]
        public async Task<IActionResult> Update(int resultId, [FromBody] ResultDTO updatedResultDto)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            var result = await _context.Results.FirstOrDefaultAsync(r => r.ResultId == resultId);
            if (result == null)
            {
                return NotFound("Result not found for the specified ResultId.");
            }

            // Update the properties of the result
            result.DiamondId = updatedResultDto.DiamondId;
            result.DiamondOrigin = updatedResultDto.DiamondOrigin;
            result.Shape = updatedResultDto.Shape;
            result.Measurements = updatedResultDto.Measurements;
            result.CaratWeight = updatedResultDto.CaratWeight;
            result.Color = updatedResultDto.Color;
            result.Clarity = updatedResultDto.Clarity;
            result.Cut = updatedResultDto.Cut;
            result.Proportions = updatedResultDto.Proportions;
            result.Polish = updatedResultDto.Polish;
            result.Symmetry = updatedResultDto.Symmetry;
            result.Fluorescence = updatedResultDto.Fluorescence;

            // Find the related Request and update its status
            var request = await _context.Requests.FirstOrDefaultAsync(r => r.RequestId == result.RequestId);
            if (request != null)
            {
                request.Status = "Chờ xác nhận";
            }

            await _context.SaveChangesAsync();

            return Ok(new { Message = "Result updated successfully. Chờ xác nhận", ResultId = result.ResultId });
        }



        // Get Result by ResultId action
        [HttpGet("get/{resultId}")]
        public async Task<IActionResult> GetByResultId(int resultId)
        {
            var result = await _context.Results.FirstOrDefaultAsync(r => r.ResultId == resultId);
            if (result == null)
            {
                return NotFound("Result not found for the specified ResultId.");
            }

            var resultDto = new ResultDTO
            {
                ResultId = result.ResultId,              
                RequestId = result.RequestId,
                DiamondOrigin = result.DiamondOrigin,
                Shape = result.Shape,
                Measurements = result.Measurements,
                CaratWeight = result.CaratWeight,
                Color = result.Color,
                Clarity = result.Clarity,
                Cut = result.Cut,
                Proportions = result.Proportions,
                Polish = result.Polish,
                Symmetry = result.Symmetry,
                Fluorescence = result.Fluorescence
            };

            return Ok(resultDto);
        }

        // Delete Result by RequestId action
        [HttpDelete("delete/{requestId}")]
        public async Task<IActionResult> DeleteByRequestId(int requestId)
        {
            var result = await _context.Results.FirstOrDefaultAsync(r => r.RequestId == requestId);
            if (result == null)
            {
                return NotFound("Result not found for the specified RequestId.");
            }

            _context.Results.Remove(result);
            await _context.SaveChangesAsync();

            return Ok(new { Message = "Result deleted successfully." });
        }
    }
}
