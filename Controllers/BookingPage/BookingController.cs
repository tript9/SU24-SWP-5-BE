using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using SWPApp.DTO;
using SWPApp.Models;
using System.Threading.Tasks;

namespace SWPApp.Controllers.BookingPage
{
    [Route("api/[controller]")]
    [ApiController]
    public class BookingController : ControllerBase
    {
        private readonly DiamondAssesmentSystemDBContext _context;

        public BookingController(DiamondAssesmentSystemDBContext context)
        {
            _context = context;
        }

        [HttpPost("book-appointment")]
        public async Task<IActionResult> BookAppointment([FromBody] BookingAppointmentModel model)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            // Retrieve the login token from the model
            var loginToken = model.Token;

            if (string.IsNullOrEmpty(loginToken))
            {
                return Unauthorized("Login token is required");
            }

            // Retrieve the customer by login token
            var customer = await _context.Customers.FirstOrDefaultAsync(c => c.LoginToken == loginToken && c.LoginTokenExpires > DateTime.UtcNow);

            if (customer == null)
            {
                return Unauthorized("Invalid or expired login token");
            }

            // Update customer details
            customer.PhoneNumber = model.PhoneNumber;
            customer.IDCard = model.IDCard;
            customer.Address = model.Address;

            // Save changes to the database
            await _context.SaveChangesAsync();

            return Ok("Booking appointment successful and customer details updated.");
        }
    }
}
