using Microsoft.AspNetCore.Identity;

namespace TixGen.Models
{
    public class User : IdentityUser
    {
        public string? Role { get; set; }
    }
}
