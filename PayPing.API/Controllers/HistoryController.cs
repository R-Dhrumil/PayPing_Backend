using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using PayPing.Domain.Entities;
using PayPing.Infrastructure.Persistence;

namespace PayPing.API.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class HistoryController : ControllerBase
    {
        private readonly ApplicationDbContext _context;

        public HistoryController(ApplicationDbContext context)
        {
            _context = context;
        }

        [HttpGet]
        public async Task<ActionResult<IEnumerable<PaymentHistory>>> GetHistory()
        {
            return await _context.PaymentHistories
                .Include(h => h.Reminder)
                .OrderByDescending(h => h.PaymentDate)
                .ToListAsync();
        }

        [HttpGet("reminder/{reminderId}")]
        public async Task<ActionResult<IEnumerable<PaymentHistory>>> GetHistoryByReminder(Guid reminderId)
        {
            return await _context.PaymentHistories
                .Where(h => h.ReminderId == reminderId)
                .OrderByDescending(h => h.PaymentDate)
                .ToListAsync();
        }
    }
}
