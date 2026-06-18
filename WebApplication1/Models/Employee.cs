using System.Text.Json.Serialization;

namespace WebApplication1.Models
{
    public class Employee
    {
        public int Id { get; set; }
        public string FirstName { get; set; } = "";
        public string LastName { get; set; } = "";
        public string Email { get; set; } = "";
        public string Department { get; set; } = "";

        [JsonIgnore]
        public List<LeaveRequest> LeaveRequests { get; set; } = []; //had to include this to avoid circular reference when serializing to JSON
    }
}