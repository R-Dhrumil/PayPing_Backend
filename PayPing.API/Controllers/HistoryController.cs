using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using PayPing.Domain.Entities;
using PayPing.Infrastructure.Persistence;
using Microsoft.AspNetCore.Authorization;
using System.Security.Claims;

namespace PayPing.API.Controllers
{
    [Authorize]
    [ApiController]
    [Route("api/[controller]")]
    public class HistoryController : ControllerBase
    {
        private readonly ApplicationDbContext _context;

        public HistoryController(ApplicationDbContext context)
        {
            _context = context;
        }

        private string GetCurrentUserId() => User.FindFirst(ClaimTypes.NameIdentifier)?.Value ?? string.Empty;

        [HttpGet]
        public async Task<ActionResult<IEnumerable<PaymentHistory>>> GetHistory()
        {
            var userId = GetCurrentUserId();
            return await _context.PaymentHistories
                .Include(h => h.Reminder)
                .Where(h => h.Reminder.UserId == userId)
                .OrderByDescending(h => h.PaymentDate)
                .ToListAsync();
        }

        [HttpGet("reminder/{reminderId}")]
        public async Task<ActionResult<IEnumerable<PaymentHistory>>> GetHistoryByReminder(Guid reminderId)
        {
            var userId = GetCurrentUserId();
            return await _context.PaymentHistories
                .Include(h => h.Reminder)
                .Where(h => h.ReminderId == reminderId && h.Reminder.UserId == userId)
                .OrderByDescending(h => h.PaymentDate)
                .ToListAsync();
        }
    }
}
