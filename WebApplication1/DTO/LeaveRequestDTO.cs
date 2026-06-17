using System.ComponentModel.DataAnnotations;


namespace WebApplication1.DTO
{
    public class CreateLeaveRequestDTO
    {
        public int EmployeeId { get; set; }
        [Required]
        public DateTime StartDate { get; set; }
        [Required]
        public DateTime EndDate { get; set; }
        [Required]
        [MinLength(5)]
        public string Reason { get; set; } = "";
    }
}