namespace TixGen.Models
{
    public class SupportTicket
    {
        public int Id { get; set; }
        public string? Title { get; set; }
        public string? Description { get; set; }
        public string? Status { get; set; } = "Open"; // Open, In Progress, Resolved
        public string? AssignedTo { get; set; } // Support Agent
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
    }
}
