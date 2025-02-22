using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using TixGen.Models;

namespace TixGen.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class AuthController : ControllerBase
    {

        private readonly IConfiguration _configuration; // Uses Configurations
        private readonly UserManager<User> _userManager;
        private readonly RoleManager<IdentityRole> _roleManager;
      
        // Dependency injection
        public AuthController(UserManager<User> userManager, RoleManager<IdentityRole> roleManager,IConfiguration configuration)
        {
            _userManager = userManager;
            _roleManager = roleManager;
            _configuration = configuration;
        }

        // Creation of User Registeration with Assign Role
        [HttpPost("register")]
        public async Task<IActionResult> Register([FromBody] RegisterModel model)
        {
            // If role is null or not in the list, set default role as "Agent"
            string roleToAssign = (!string.IsNullOrEmpty(model.Role) && new[] { "Admin", "Agent" }.Contains(model.Role))
                                  ? model.Role
                                  : "Agent";

            //if (!new[] { "Admin", "Agent" }.Contains(model.Role))
            //    return BadRequest("Invalid role. Choose 'Admin' or 'Agent'."); // Check Default mentioned roles in program.cs file

            var userExists = await _userManager.FindByNameAsync(model.Username); // Find UserName in userManger
            if (userExists != null)
                return BadRequest("User already exists"); // if exists throw an error

            var user = new User
            {
                UserName = model.Username,
                Email = model.Email,
                Role = model.Role
            };

            var result = await _userManager.CreateAsync(user, model.Password); // Create account using userManger
            if (!result.Succeeded)
                return BadRequest(result.Errors);

            // Ensure the role exists before assigning beacuse if enter wrong roles or strings
            if (!await _roleManager.RoleExistsAsync(model.Role))
            {
                await _roleManager.CreateAsync(new IdentityRole(model.Role)); 
            }

            // Assign role to user
            await _userManager.AddToRoleAsync(user, model.Role);

            return Ok(new { Message = $"User registered successfully As {model.Role} " });
        }


        [HttpPost("login")]
        public async Task<IActionResult> Login([FromBody] LoginModel model)
        {
            var user = await _userManager.FindByNameAsync(model.Username);   // Find Username

            if (user == null || !await _userManager.CheckPasswordAsync(user, model.Password))
                return Unauthorized("Invalid credentials");

            var role = await _userManager.GetRolesAsync(user); // Get Role

            var token = GenerateJwtToken(user); // Generate JWT Token method


            return Ok(new { token }); // Token 
        }

        // Method for GenerateJwtToken
        // FIXED: Generate JWT Token with Proper Role Handling
        private async Task<string> GenerateJwtToken(User user)
        {
            var userRoles = await _userManager.GetRolesAsync(user); //  Get user roles

            var claims = new List<Claim>
             {
                new Claim(JwtRegisteredClaimNames.Sub, user.UserName),
                new Claim(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString()),
                new Claim(ClaimTypes.NameIdentifier, user.Id),
                 new Claim("UserName", user.UserName) // Explicitly add UserName as a custom claim
             };


            // Add each role as a separate claim
            foreach (var role in userRoles)
            {
                claims.Add(new Claim(ClaimTypes.Role, role));
            }

            // Get JWT Secret Key From appSettings.json file
            var jwtConfig = _configuration.GetSection("JwtSettings");
            var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(jwtConfig["SecretKey"]));

            var creds = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);
            var token = new JwtSecurityToken(
                issuer: jwtConfig["Issuer"],
                audience: jwtConfig["Audience"],
                claims: claims,
                expires: DateTime.Now.AddHours(1),
                signingCredentials: creds
            );

            return new JwtSecurityTokenHandler().WriteToken(token);
        }
    }
}
