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
            // Check login or not
            var customer = await _context.Customers.FirstOrDefaultAsync(c => c.Status == true && c.LoginToken != null);
            if (customer != null)
            {
                // Update customer details
                customer.CustomerName = model.CustomerName;
                customer.PhoneNumber = model.PhoneNumber;
                customer.IDCard = model.IDCard;
                customer.Address = model.Address;

                // Save changes to the database
                await _context.SaveChangesAsync();

                return Ok("Booking appointment successful and customer details updated.");
            }
            else
            {
                return Unauthorized("You must login to use this service.");
            }
        }

        [HttpPost("logout")]
        public async Task<IActionResult> Logout()
        {
            var customer = await _context.Customers.FirstOrDefaultAsync(c => c.Status == true && c.LoginToken != null);

            if (customer == null)
            {
                return Unauthorized("You must login.");
            }

            // Invalidate the login token
            customer.LoginToken = null;
            customer.LoginTokenExpires = null;

            // Set status to indicate logged out
            customer.Status = false;

            _context.Customers.Update(customer);
            await _context.SaveChangesAsync();

            return Ok("Logout successful.");
        }

    }
}
