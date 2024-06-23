using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using SWPApp.Models;
using System.Linq;
using System.Threading.Tasks;
using System.Collections.Generic;

namespace SWPApp.Controllers.AdminClient
{
    [Route("api/[controller]")]
    [ApiController]
    public class AccountController : ControllerBase
    {
        private readonly DiamondAssesmentSystemDBContext _context;

        public AccountController(DiamondAssesmentSystemDBContext context)
        {
            _context = context;
        }

        // Create Employee
        [HttpPost("create-employee")]
        public async Task<IActionResult> CreateEmployee([FromBody] Employee employee)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            employee.Password = BCrypt.Net.BCrypt.HashPassword(employee.Password);

            _context.Employees.Add(employee);
            await _context.SaveChangesAsync();

            return Ok(employee);
        }

        // Create Customer
        [HttpPost("create-customer")]
        public async Task<IActionResult> CreateCustomer([FromBody] Customer customer)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            customer.Password = BCrypt.Net.BCrypt.HashPassword(customer.Password);

            _context.Customers.Add(customer);
            await _context.SaveChangesAsync();

            return Ok(customer);
        }

        // Get Employee by ID
        [HttpGet("employee/{id}")]
        public async Task<IActionResult> GetEmployee(int id)
        {
            var employee = await _context.Employees.FindAsync(id);

            if (employee == null)
            {
                return NotFound();
            }

            return Ok(employee);
        }

        // Get Customer by ID
        [HttpGet("customer/{id}")]
        public async Task<IActionResult> GetCustomer(int id)
        {
            var customer = await _context.Customers.FindAsync(id);

            if (customer == null)
            {
                return NotFound();
            }

            return Ok(customer);
        }

        // Update Employee
        [HttpPut("update-employee/{id}")]
        public async Task<IActionResult> UpdateEmployee(int id, [FromBody] Employee updatedEmployee)
        {
            if (id != updatedEmployee.EmployeeId)
            {
                return BadRequest("Employee ID mismatch");
            }

            var employee = await _context.Employees.FindAsync(id);
            if (employee == null)
            {
                return NotFound();
            }

            employee.EmployeeName = updatedEmployee.EmployeeName;
            employee.Email = updatedEmployee.Email;
            if (!string.IsNullOrEmpty(updatedEmployee.Password))
            {
                employee.Password = BCrypt.Net.BCrypt.HashPassword(updatedEmployee.Password);
            }
            employee.Phone = updatedEmployee.Phone;
            employee.Role = updatedEmployee.Role;
            employee.Status = updatedEmployee.Status;

            _context.Employees.Update(employee);
            await _context.SaveChangesAsync();

            return Ok(employee);
        }

        // Update Customer
        [HttpPut("update-customer/{id}")]
        public async Task<IActionResult> UpdateCustomer(int id, [FromBody] Customer updatedCustomer)
        {
            if (id != updatedCustomer.CustomerId)
            {
                return BadRequest("Customer ID mismatch");
            }

            var customer = await _context.Customers.FindAsync(id);
            if (customer == null)
            {
                return NotFound();
            }

            customer.CustomerName = updatedCustomer.CustomerName;
            customer.Email = updatedCustomer.Email;
            if (!string.IsNullOrEmpty(updatedCustomer.Password))
            {
                customer.Password = BCrypt.Net.BCrypt.HashPassword(updatedCustomer.Password);
            }
            customer.PhoneNumber = updatedCustomer.PhoneNumber;
            customer.IDCard = updatedCustomer.IDCard;
            customer.Address = updatedCustomer.Address;
            customer.Status = updatedCustomer.Status;

            _context.Customers.Update(customer);
            await _context.SaveChangesAsync();

            return Ok(customer);
        }

        // Delete Employee
        [HttpDelete("delete-employee/{id}")]
        public async Task<IActionResult> DeleteEmployee(int id)
        {
            var employee = await _context.Employees.FindAsync(id);
            if (employee == null)
            {
                return NotFound();
            }

            _context.Employees.Remove(employee);
            await _context.SaveChangesAsync();

            return Ok();
        }

        // Delete Customer
        [HttpDelete("delete-customer/{id}")]
        public async Task<IActionResult> DeleteCustomer(int id)
        {
            var customer = await _context.Customers.FindAsync(id);
            if (customer == null)
            {
                return NotFound();
            }

            _context.Customers.Remove(customer);
            await _context.SaveChangesAsync();

            return Ok();
        }

        // List all Customers
        [HttpGet("list-customers")]
        public async Task<ActionResult<IEnumerable<Customer>>> ListCustomers()
        {
            var customers = await _context.Customers
                .Select(c => new
                {
                    c.CustomerId,
                    c.CustomerName,
                    c.Email,
                })
                .ToListAsync();

            return Ok(customers);
        }

        // List all Employees
        [HttpGet("list-employees")]
        public async Task<ActionResult<IEnumerable<Employee>>> ListEmployees()
        {
            var employees = await _context.Employees
                .Select(e => new
                {
                    e.EmployeeId,
                    e.EmployeeName,
                    e.Email,
                })
                .ToListAsync();

            return Ok(employees);
        }
        // List all Requests with Details
        [HttpGet("list-requests-with-details")]
        public async Task<ActionResult<IEnumerable<object>>> ListRequestsWithDetails()
        {
            var requestsWithDetails = await _context.Requests
                .Join(_context.RequestDetails,
                    r => r.RequestId,
                    rd => rd.RequestId,
                    (r, rd) => new
                    {
                        r.RequestId,
                        r.CustomerId,
                        r.RequestDate,
                        r.ServiceType,
                        rd.ServiceId,
                        rd.PaymentStatus,
                        rd.PaymentMethod
                    })
                .ToListAsync();

            return Ok(requestsWithDetails);
        }
        // Accept Request
        [HttpPost("accept-request/{id}")]
        public async Task<IActionResult> AcceptRequest(int id)
        {
            var request = await _context.Requests.FindAsync(id);

            if (request == null)
            {
                return NotFound();
            }

            request.Status = true; // Assuming 1 corresponds to 'true' and 0 corresponds to 'false'

            _context.Requests.Update(request);
            await _context.SaveChangesAsync();

            return Ok(request);
        }

    }
}
