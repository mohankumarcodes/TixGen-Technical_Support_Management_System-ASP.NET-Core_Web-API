namespace TixGen.Models
{
    public class RegisterModel
    {
        public string? Username { get; set; }
        public string? Email { get; set; }

        //public string FullName { get; set; }
        public string? Password { get; set; }
        public string? Role { get; set; }  // User chooses role: "Admin" or "Agent"
    }
}
