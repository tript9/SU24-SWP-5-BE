using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using SWPApp.DTO;
using SWPApp.Models;
using System.Threading.Tasks;

namespace SWPApp.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class BookingController : ControllerBase
    {
        public class BookingAppointmentModel
        {
            public string CustomerName { get; set; }
            public string PhoneNumber { get; set; }
            public string IDCard { get; set; }
            public string Address { get; set; }

        }

        private readonly DiamondAssesmentSystemDBContext _context;

        public BookingController(DiamondAssesmentSystemDBContext context)
        {
            _context = context;
        }

        [HttpPost("book-appointment")]
        public async Task<IActionResult> BookAppointment([FromBody] BookingAppointmentModel model)
        {
            // Check if the customer is logged in
            var customer = await _context.Customers.FirstOrDefaultAsync(c => c.Status == true && c.LoginToken != null);
            if (customer != null)
            {
                // Update customer details
                customer.CustomerName = model.CustomerName;
                customer.PhoneNumber = model.PhoneNumber;
                customer.IDCard = model.IDCard;
                customer.Address = model.Address;

                // Save customer changes to the database
                await _context.SaveChangesAsync();

                return Ok("Booking appointment successful and customer details updated.");
            }
            else
            {
                return Unauthorized("You must login to use this service.");
            }
        }


    }
}
