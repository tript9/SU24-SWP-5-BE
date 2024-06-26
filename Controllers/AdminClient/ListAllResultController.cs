﻿using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using SWPApp.Models;

namespace SWPApp.Controllers.AdminClient
{
    [Route("api/[controller]")]
    [ApiController]
    public class ListAllResultController : ControllerBase
    {
        private readonly DiamondAssesmentSystemDBContext _context;
        public ListAllResultController(DiamondAssesmentSystemDBContext context)
        {
            _context = context;
        }
        [HttpGet("list-results")]
        public async Task<ActionResult<IEnumerable<Result>>> ListResults()
        {
            var results = await _context.Results
                .Where(r => r.Request.Status == "Chờ xác nhận" || r.Request.Status == "Kiểm định thành công")
                .Select(r => new
                {
                    r.ResultId,
                    r.DiamondId,
                    r.RequestId,
                    r.DiamondOrigin,
                    r.Shape,
                    r.Measurements,
                    r.CaratWeight,
                    r.Color,
                    r.Clarity,
                    r.Cut,
                    r.Proportions,
                    r.Polish,
                    r.Symmetry,
                    r.Fluorescence,
                    RequestStatus = r.Request.Status // Include the status in the response
                })
                .ToListAsync();

            return Ok(results);
        }



        //accept status = "kiểm định thành công "        
        [HttpPut("update-request-status/{requestid}")]
        public async Task<IActionResult> UpdateRequestStatus(int requestid)
        {
            var request = await _context.Requests.FindAsync(requestid);
            if (request == null)
            {
                return NotFound();
            }

            request.Status = "kiểm định thành công";
            _context.Requests.Update(request);
            await _context.SaveChangesAsync();

            return Ok(request);
        }
    }
}
