using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using SWPApp.Models;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.ComponentModel.DataAnnotations;

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
            public string? DiamondOrigin { get; set; } = "none";
            public string? Shape { get; set; } = "none";
            public string? Measurements { get; set; } = "none";
            public decimal? CaratWeight { get; set; }
            public string? Color { get; set; } = "none";
            public string? Clarity { get; set; } = "none";
            public string? Cut { get; set; } = "none";
            public string? Proportions { get; set; } = "none";
            public string? Polish { get; set; } = "none";
            public string? Symmetry { get; set; } = "none";
            public string? Fluorescence { get; set; } = "none";
            public string? Certification { get; set; } = "none";
            public decimal? Price { get; set; }
            public string? Comments { get; set; } = "none";
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
            result.DiamondOrigin = updatedResultDto.DiamondOrigin ?? "none";
            result.Shape = updatedResultDto.Shape ?? "none";
            result.Measurements = updatedResultDto.Measurements ?? "none";
            result.CaratWeight = updatedResultDto.CaratWeight;
            result.Color = updatedResultDto.Color ?? "none";
            result.Clarity = updatedResultDto.Clarity ?? "none";
            result.Cut = updatedResultDto.Cut ?? "none";
            result.Proportions = updatedResultDto.Proportions ?? "none";
            result.Polish = updatedResultDto.Polish ?? "none";
            result.Symmetry = updatedResultDto.Symmetry ?? "none";
            result.Fluorescence = updatedResultDto.Fluorescence ?? "none";
            result.Certification = updatedResultDto.Certification ?? "none";
            result.Price = updatedResultDto.Price;
            result.Comments = updatedResultDto.Comments ?? "none";

            // Find the related Request and update its status
            var request = await _context.Requests.FirstOrDefaultAsync(r => r.RequestId == result.RequestId);
            if (request != null)
            {
                request.Status = "Chờ xác nhận";
            }

            await _context.SaveChangesAsync();

            return Ok(new { Message = "Result updated successfully. Chờ xác nhận", ResultId = result.ResultId });
        }

        [HttpPost("create")]
        public async Task<IActionResult> Create([FromBody] ResultDTO resultDto)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            // Check if the Request exists using ResultDto.RequestId
            var request = await _context.Requests.FindAsync(resultDto.RequestId);
            if (request == null)
            {
                return NotFound(new { Message = "Request not found for the given RequestId." });
            }

            // Check if a Result already exists for the given RequestId
            var existingResult = await _context.Results.FirstOrDefaultAsync(r => r.RequestId == resultDto.RequestId);
            if (existingResult != null)
            {
                return BadRequest(new { Message = "A Result already exists for the given RequestId." });
            }

            // Create the Result object
            var result = new Result
            {
                DiamondId = resultDto.DiamondId,
                RequestId = resultDto.RequestId,
                DiamondOrigin = resultDto.DiamondOrigin ?? "none",
                Shape = resultDto.Shape ?? "none",
                Measurements = resultDto.Measurements ?? "none",
                CaratWeight = resultDto.CaratWeight,
                Color = resultDto.Color ?? "none",
                Clarity = resultDto.Clarity ?? "none",
                Cut = resultDto.Cut ?? "none",
                Proportions = resultDto.Proportions ?? "none",
                Polish = resultDto.Polish ?? "none",
                Symmetry = resultDto.Symmetry ?? "none",
                Fluorescence = resultDto.Fluorescence ?? "none",
                Certification = resultDto.Certification ?? "none",
                Price = resultDto.Price,
                Comments = resultDto.Comments ?? "none"
            };

            // Add the Result to the database
            _context.Results.Add(result);
            await _context.SaveChangesAsync();

            // Update the status of the related Request
            request.Status = "Chờ xác nhận";
            await _context.SaveChangesAsync();

            return Ok(new { Message = "Result created successfully. Chờ xác nhận", ResultId = result.ResultId });
        }

        // Get Results by ServiceId action
        [HttpGet("get-by-serviceid/{serviceId}")]
        public async Task<IActionResult> GetByServiceId(string serviceId)
        {
            var results = await _context.Results
                .Include(r => r.Request)
                .Where(r => r.Request.ServiceId == serviceId)
                .ToListAsync();

            if (results == null || !results.Any())
            {
                return NotFound("No results found for the specified ServiceId.");
            }

            var resultDtos = results.Select(result => new ResultDTO
            {
                ResultId = result.ResultId,
                DiamondId = result.DiamondId ?? 0, // Default to 0 if null
                RequestId = result.RequestId,
                DiamondOrigin = result.DiamondOrigin ?? "none",
                Shape = result.Shape ?? "none",
                Measurements = result.Measurements ?? "none",
                CaratWeight = result.CaratWeight ?? 0, // Default to 0 if null
                Color = result.Color ?? "none",
                Clarity = result.Clarity ?? "none",
                Cut = result.Cut ?? "none",
                Proportions = result.Proportions ?? "none",
                Polish = result.Polish ?? "none",
                Symmetry = result.Symmetry ?? "none",
                Fluorescence = result.Fluorescence ?? "none",
                Certification = result.Certification ?? "none",
                Price = result.Price ?? 0, // Default to 0 if null
                Comments = result.Comments ?? "none"
            }).ToList();

            return Ok(resultDtos);
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
