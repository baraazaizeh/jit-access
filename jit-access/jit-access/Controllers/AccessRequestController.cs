using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using MimeKit;
using Microsoft.Extensions.Logging;
using Hangfire;
using static jit_access.AccessRequest;

namespace jit_access.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class AccessRequestController : ControllerBase
    {
        private readonly MyDbContext _context;
        private readonly ILogger<AccessRequestController> _logger;
        public AccessRequestController(MyDbContext context, ILogger<AccessRequestController> logger)
        {
            _context = context;
            _logger = logger;
        }
        public AccessRequestController(MyDbContext context)
        {
            _context = context;
        }
        [HttpPost]
        public async Task<IActionResult> CreateAccessRequest([FromBody] AccessRequest request)
        {
            // Validate the request data
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            // Set the request time
            request.RequestTime = DateTime.UtcNow;
            request.Status = AccessRequestStatus.Pending;

            // Save the request to the database
            _context.AccessRequests.Add(request);
            await _context.SaveChangesAsync();


            _logger.LogInformation("Access request created successfully: {Request}", request);
            // Send notifications (e.g., email, Teams)
            // ...

            return Ok(request);
        }
        [HttpPut("{id}/approve")]
        [Authorize(Policy = "ApproverPolicy")]
        public async Task<IActionResult> ApproveAccessRequest(int id)
        {
            var request = await _context.AccessRequests.FindAsync(id);
            if (request == null)
            {
                return NotFound();
            }

            // Implement authorization checks to ensure only authorized users can approve

            request.Status = AccessRequestStatus.Approved;
            request.ApprovalTime = DateTime.UtcNow;

            // Grant database permissions (implement specific logic)
            string userName = "YourUserName"; // Replace with actual user information
            string sql = $"GRANT SELECT, INSERT, UPDATE, DELETE ON YourDatabase.dbo.YourTable TO {userName}"; // change this to role based like SA


            var pendingRequests = await _context.AccessRequests
        .Where(r => r.Status == AccessRequestStatus.Pending)
        .ToListAsync();

            await _context.Database.ExecuteSqlRawAsync(sql);
            await _context.AccessRequests.AddAsync(request);
            await _context.SaveChangesAsync();

            // ... inside the ApproveAccessRequest method

            // Send email notification
            var message = new MimeMessage();
            message.From.Add(new MailboxAddress("Your Name", "your_email@example.com"));
            message.To.Add(new MailboxAddress(request.UserName, request.UserEmail));
            message.Subject = "Access Request Approved";
            message.Body = new TextPart("plain") { Text = "Your access request has been approved." };

            using (var client = new SmtpClient())
            {
                client.Connect("smtp.example.com", 587, false);
                client.Authenticate("your_email@example.com", "your_password");
                client.Send(message);
                client.Disconnect(true);
                client.Dispose();
            }

            return Ok(request);
        }
    }
}
