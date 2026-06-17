using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using WebApplication1.Data;
using WebApplication1.DTO;
using WebApplication1.Models;

namespace WebApplication1.Controllers
{
    [ApiController]
    [Route("employees")]
    public class EmployeeController : ControllerBase
    {
        private readonly AppDbContext _context;

        public EmployeeController(AppDbContext context)
        {
            _context = context;
        }

        [HttpGet]
        public async Task<IActionResult> GetEmployees()
        {
            var employees = await _context.Employees.ToListAsync();
            return Ok(employees);
        }

        [HttpGet("{id}")] //get emp by id
        public async Task<IActionResult> GetEmployeeById(int id)
        {
            var employee = await _context.Employees.FindAsync(id);

            if (employee == null)
                return NotFound();

            return Ok(employee);
        }

        [HttpPost]
        public async Task<IActionResult> CreateEmployee(CreateEmployeeDTO dto) //create new employee using employee dto
        {
            var employee = new Employee
            {
                FirstName = dto.FirstName,
                LastName = dto.LastName,
                Email = dto.Email,
                Department = dto.Department
            };

            _context.Employees.Add(employee);
            await _context.SaveChangesAsync();

            return CreatedAtAction(nameof(GetEmployeeById), new { id = employee.Id }, employee);
        }
        [HttpPut("{id}")] //update emp info
        public async Task<IActionResult> UpdateEmployee(int id, CreateEmployeeDTO dto)
        {
            var employee = await _context.Employees.FindAsync(id);

            if (employee == null)
                return NotFound();

            employee.FirstName = dto.FirstName;
            employee.LastName = dto.LastName;
            employee.Email = dto.Email;
            employee.Department = dto.Department;

            await _context.SaveChangesAsync();

            return Ok(employee);
        }
        [HttpDelete("{id}")] /*delete emp*/
        public async Task<IActionResult> DeleteEmployee(int id)
        {
            var employee = await _context.Employees.FindAsync(id);

            if (employee == null)
                return NotFound();

            _context.Employees.Remove(employee);
            await _context.SaveChangesAsync();

            return NoContent();
        }
        [HttpGet("{id}/leaverequests")] /*Used to only retrieve requests by emp id instead of returning the entire leave requests, avoiding overhead on db*/
        public async Task<IActionResult> GetLeaveRequestsForEmployee(int id)
        {
            var employeeExists = await _context.Employees.AnyAsync(e => e.Id == id);

            if (!employeeExists)
                return NotFound("Employee not found");

            var requests = await _context.LeaveRequests
                .Where(l => l.EmployeeId == id)
                .Select(l => new LeaveRequestResponseDTO
                {
                    Id = l.Id,
                    EmployeeName = l.Employee.FirstName + " " + l.Employee.LastName,
                    StartDate = l.StartDate,
                    EndDate = l.EndDate,
                    Reason = l.Reason,
                    Status = l.Status
                })
                .ToListAsync();

            return Ok(requests);
        }
    }
}