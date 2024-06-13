using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using SWPApp.DTO;
using SWPApp.Models;
using System.Threading.Tasks;

namespace SWPApp.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class RequestController : ControllerBase
    {
        private readonly DiamondAssesmentSystemDBContext _context;

        public RequestController(DiamondAssesmentSystemDBContext context)
        {
            _context = context;
        }

        [HttpPost("validate-customer")]
        public async Task<IActionResult> ValidateCustomer([FromBody] int customerId)
        {
            var customer = await _context.Customers.FindAsync(customerId);

            if (customer == null)
            {
                return BadRequest("Invalid CustomerId");
            }

            return Ok("CustomerId is valid, please fill out the request form.");
        }

        [HttpPost("insert-request")]
        public async Task<IActionResult> InsertRequest([FromBody] RequestDTO requestDTO)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            // Retrieve the customer ID from some external source (e.g., authenticated user, token, etc.)
            var customerId = GetCustomerIdFromContext(); // Implement this method according to your authentication logic

            var customer = await _context.Customers.FindAsync(customerId);

            if (customer == null)
            {
                return BadRequest("Invalid CustomerId");
            }

            var request = new Request
            {
                CustomerId = customerId,
                RequestDate = requestDTO.RequestDate,
                ServiceType = requestDTO.ServiceType,
                EmployeeId = requestDTO.EmployeeId,
                DiamondId = requestDTO.DiamondId,
                ServiceId = requestDTO.ServiceId,
                Status = requestDTO.Status
            };

            await _context.Requests.AddAsync(request);
            await _context.SaveChangesAsync();

            // Log to console for confirmation
            Console.WriteLine($"Inserted request for CustomerId: {request.CustomerId}");

            return Ok("Request inserted successfully");
        }

        // Example method to get customer ID (you need to implement this according to your authentication logic)
        private int GetCustomerIdFromContext()
        {
            // Your logic to retrieve the customer ID, e.g., from the authenticated user's claims or a token
            return 1; // Placeholder, replace with actual logic
        }


        [HttpGet("info-customer/{customerId}")]
public async Task<IActionResult> InfoCustomer(int customerId)
{
    var customer = await _context.Customers
        .Select(c => new CustomerInfoDTO
        {
            CustomerId = c.CustomerId,
            CustomerName = c.CustomerName,
            Email = c.Email,
            PhoneNumber = c.PhoneNumber,
            IDCard = c.IDCard,
            Address = c.Address,
            Status = c.Status
        })
        .FirstOrDefaultAsync(c => c.CustomerId == customerId);

    if (customer == null)
    {
        return NotFound("Customer not found");
    }

    return Ok(customer);
}

    }
}
