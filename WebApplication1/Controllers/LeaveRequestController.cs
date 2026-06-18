using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using WebApplication1.Data;
using WebApplication1.DTO;
using WebApplication1.Models;

namespace WebApplication1.Controllers
{
    [ApiController]
    [Route("leaverequests")]
    public class LeaveRequestsController : ControllerBase
    {
        private readonly AppDbContext _context;

        public LeaveRequestsController(AppDbContext context)
        {
            _context = context;
        }

        [HttpGet]
        public async Task<IActionResult> GetLeaveRequests()
        {
            var requests = await _context.LeaveRequests
            .Include(l => l.Employee)
            .Select(l => new LeaveRequestResponseDTO
    {
            Id = l.Id,

            EmployeeName =
                l.Employee.FirstName + " " +
                l.Employee.LastName,

            StartDate = l.StartDate,

            EndDate = l.EndDate,

            Reason = l.Reason,

            Status = l.Status
            })
            .ToListAsync();
             return Ok(requests);
        }

        [HttpPost]
        public async Task<IActionResult> CreateLeaveRequest(CreateLeaveRequestDTO dto)
        {
            var employeeExists = await _context.Employees.AnyAsync(e => e.Id == dto.EmployeeId);
            if (!employeeExists)
                return NotFound("Employee not found");
            if (dto.EndDate <= dto.StartDate)
            {
                return BadRequest("End date cannot be before start date, ValidationError"); //cant have end date before start date, validation 
            }
            var request = new LeaveRequest
            {
                EmployeeId = dto.EmployeeId,
                StartDate = dto.StartDate,
                EndDate = dto.EndDate,
                Reason = dto.Reason,
                Status = "Pending"
            };

            _context.LeaveRequests.Add(request);
            await _context.SaveChangesAsync();

            return Ok(request);
        }
        [HttpPut("{id}/approve")] //update based on id, approve leave request
        public async Task<IActionResult> ApproveLeaveRequest(int id)
        {
            var request = await _context.LeaveRequests.FindAsync(id);

            if (request == null)
                return NotFound("Leave request not found");

            request.Status = "Approved";
            await _context.SaveChangesAsync();

            return Ok(request);
        }

        [HttpPut("{id}/reject")] //update based on id, reject leave request
        public async Task<IActionResult> RejectLeaveRequest(int id)
        {
            var request = await _context.LeaveRequests.FindAsync(id);

            if (request == null)
                return NotFound("Leave request not found");

            request.Status = "Rejected";
            await _context.SaveChangesAsync();

            return Ok(request);
        }
    }
}