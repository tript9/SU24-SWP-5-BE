using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using SWPApp.Models;
using System.Threading.Tasks;

namespace SWPApp.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class EmployeeController : ControllerBase
    {
        private readonly DiamondAssesmentSystemDBContext _context;

        public EmployeeController(DiamondAssesmentSystemDBContext context)
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

            await _context.Employees.AddAsync(employee);
            await _context.SaveChangesAsync();

            return Ok("Employee created successfully");
        }

        // Get Employee by Id
        [HttpGet("get-employee/{id}")]
        public async Task<IActionResult> GetEmployee(int id)
        {
            var employee = await _context.Employees.FindAsync(id);

            if (employee == null)
            {
                return NotFound("Employee not found");
            }

            return Ok(employee);
        }

        // Update Employee
        [HttpPut("update-employee/{id}")]
        public async Task<IActionResult> UpdateEmployee(int id, [FromBody] Employee employee)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            var existingEmployee = await _context.Employees.FindAsync(id);

            if (existingEmployee == null)
            {
                return NotFound("Employee not found");
            }

            existingEmployee.EmployeeName = employee.EmployeeName;
            existingEmployee.Email = employee.Email;
            existingEmployee.Password = employee.Password;
            existingEmployee.Phone = employee.Phone;
            existingEmployee.Role = employee.Role;
            existingEmployee.Status = employee.Status;
            existingEmployee.LoginToken = employee.LoginToken;
            existingEmployee.LoginTokenExpires = employee.LoginTokenExpires;

            _context.Employees.Update(existingEmployee);
            await _context.SaveChangesAsync();

            return Ok("Employee updated successfully");
        }

        // Delete Employee
        [HttpDelete("delete-employee/{id}")]
        public async Task<IActionResult> DeleteEmployee(int id)
        {
            var employee = await _context.Employees.FindAsync(id);

            if (employee == null)
            {
                return NotFound("Employee not found");
            }

            _context.Employees.Remove(employee);
            await _context.SaveChangesAsync();

            return Ok("Employee deleted successfully");
        }
    }
}
