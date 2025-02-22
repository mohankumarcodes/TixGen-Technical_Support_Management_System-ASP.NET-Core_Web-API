using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;
using System.Net.Sockets;
using System.Security.Claims;
using TixGen.Data;
using TixGen.Models;

namespace TixGen.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [Authorize] // Requires Authentication
    public class TicketsController : ControllerBase
    {
      
        private readonly AppDbContext _context;
        private readonly UserManager<User> _userManager;

        public TicketsController(AppDbContext context, UserManager<User> userManager)
        {
            _context = context;
            _userManager = userManager;
        }

        // Create a Ticket (Both)
        [Authorize]
        [HttpPost("Create-Ticket")]
        public async Task<IActionResult> CreateTicket([FromBody] SupportTicket ticket)
        {
            var user = await _userManager.GetUserAsync(User);
           // ticket.AssignedTo = user.UserName;  // Assign the creator
            _context.Tickets.Add(ticket);
            await _context.SaveChangesAsync();
            return Ok(new { message = "Ticket created successfully!", ticket });
        }

        // Assign Ticket to an Agent (Admin Only)
        [Authorize(Roles ="Admin")]
        [HttpPut("Assign/{ticketId}")]
        public async Task<IActionResult> AssignTicket(int ticketId, [FromBody] string agentUsername)
        {
            var ticket = await _context.Tickets.FindAsync(ticketId);
            if (ticket == null) return NotFound("Ticket not found");

            var agent = await _userManager.FindByNameAsync(agentUsername);
            if (agent == null) return NotFound("Agent not found");

            ticket.AssignedTo = agent.UserName;
            await _context.SaveChangesAsync();
            return Ok(new { message = "Ticket assigned successfully!", ticket });
        }

        // Get All Tickets (Admin Only)
        [Authorize]
        [HttpGet("Get-All-Tickets")]
        public async Task<IActionResult> GetAllTickets()
        {
            var tickets = await _context.Tickets.ToListAsync();
            return Ok(tickets);
        }

        // Get Tickets Assigned to Agent (Agent Only)
        [Authorize(Roles = "Agent")]
        [HttpGet("My-Tickets")]
        public async Task<IActionResult> GetMyTickets()
        {
            var user = await _userManager.GetUserAsync(User);
            var tickets = await _context.Tickets.Where(t => t.AssignedTo == user.UserName).ToListAsync();
            return Ok(tickets);
        }

        // Update Ticket Status (Both)
        [Authorize(Roles = "Admin")]
        [HttpPut("Update-Status/{ticketId}")]
        public async Task<IActionResult> UpdateTicketStatus(int ticketId, [FromBody] string status)
        {
            var user = await _userManager.GetUserAsync(User);
            var ticket = await _context.Tickets.FindAsync(ticketId);

            if (ticket == null) return NotFound("Ticket not found");
           // if (ticket.AssignedTo != user.UserName) return Forbid("You can only update your assigned tickets");

            ticket.Status = status;
            await _context.SaveChangesAsync();
            return Ok(new { message = "Ticket status updated successfully!", ticket });
        }

        // Admin only delete
        [Authorize(Roles ="Admin")]
        [HttpDelete("Delete-Ticket/{ticketId}")]
        public async Task<IActionResult> DeleteTicket(int ticketId)
        {
            var user = await _userManager.GetUserAsync(User);
            var ticket = await _context.Tickets.FindAsync(ticketId);
            if (ticket == null) return NotFound("Ticket not found");

            _context.Tickets.Remove(ticket);
            await _context.SaveChangesAsync();

            return Ok(new {message= $"Tickets Deleted Successfully! |  Id :{ticket.Id}  |  Status : {ticket.Status}" });
        }

    }
}
