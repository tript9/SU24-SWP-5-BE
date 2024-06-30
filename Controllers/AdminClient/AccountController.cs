using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using SWPApp.Models;
using System.Linq;
using System.Threading.Tasks;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

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

        // DTO for Employee
        public class EmployeeDTO
        {
            [Required]
            public string EmployeeName { get; set; }

            [Required]
            [EmailAddress]
            public string Email { get; set; }

            [Required]
            public string Password { get; set; }

            public string Phone { get; set; }
        }

        // Create Employee
        [HttpPost("create-employee")]
        public async Task<IActionResult> CreateEmployee([FromBody] EmployeeDTO employeeDto)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            var employee = new Employee
            {
                EmployeeName = employeeDto.EmployeeName,
                Email = employeeDto.Email,
                Password = employeeDto.Password, // No hashing
                Phone = employeeDto.Phone
            };

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
                employee.Password = updatedEmployee.Password; // No hashing
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
                customer.Password = updatedCustomer.Password; // No hashing
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
                    c.Address,
                    c.IDCard,
                    c.PhoneNumber
                })
                .ToListAsync();

            return Ok(customers);
        }

        // List all Employees where role != 1
        [HttpGet("list-employees-except-role1")]
        public async Task<ActionResult<IEnumerable<Employee>>> ListEmployeesExceptRole1()
        {
            var employees = await _context.Employees
                .Where(e => e.Role != 1)
                .Select(e => new
                {
                    e.EmployeeId,
                    e.EmployeeName,
                    e.Email,
                    e.Role,
                    e.Phone
                })
                .ToListAsync();

            return Ok(employees);
        }

        [HttpGet("list-requests")]
        public async Task<ActionResult<IEnumerable<object>>> ListRequests()
        {
            var requests = await _context.Requests
                .Select(r => new
                {
                    r.RequestId,
                    r.CustomerId,
                    r.RequestDate,
                    r.Email,
                    r.PhoneNumber,
                    r.IDCard,
                    r.Address,
                    r.ServiceId,
                    r.Status
                })
                .ToListAsync();

            return Ok(requests);
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
