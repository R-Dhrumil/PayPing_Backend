using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using PayPing.Domain.Entities;
using PayPing.Infrastructure.Persistence;

namespace PayPing.API.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class RemindersController : ControllerBase
    {
        private readonly ApplicationDbContext _context;

        public RemindersController(ApplicationDbContext context)
        {
            _context = context;
        }

        [HttpGet]
        public async Task<ActionResult<IEnumerable<Reminder>>> GetReminders()
        {
            return await _context.Reminders.ToListAsync();
        }

        [HttpGet("{id}")]
        public async Task<ActionResult<Reminder>> GetReminder(Guid id)
        {
            var reminder = await _context.Reminders.FindAsync(id);

            if (reminder == null)
            {
                return NotFound();
            }

            return reminder;
        }

        [HttpPost]
        public async Task<ActionResult<Reminder>> PostReminder(Reminder reminder)
        {
            reminder.Id = Guid.NewGuid();
            _context.Reminders.Add(reminder);
            await _context.SaveChangesAsync();

            return CreatedAtAction(nameof(GetReminder), new { id = reminder.Id }, reminder);
        }

        [HttpPatch("{id}/mark-paid")]
        public async Task<IActionResult> MarkAsPaid(Guid id)
        {
            var reminder = await _context.Reminders.FindAsync(id);
            if (reminder == null) return NotFound();

            reminder.IsPaid = true;
            
            // Add to history
            var history = new PaymentHistory
            {
                Id = Guid.NewGuid(),
                ReminderId = reminder.Id,
                AmountPaid = reminder.Amount,
                PaymentDate = DateTime.UtcNow,
                Notes = "Marked as paid from dashboard"
            };
            
            _context.PaymentHistories.Add(history);
            await _context.SaveChangesAsync();

            return NoContent();
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteReminder(Guid id)
        {
            var reminder = await _context.Reminders.FindAsync(id);
            if (reminder == null)
            {
                return NotFound();
            }

            _context.Reminders.Remove(reminder);
            await _context.SaveChangesAsync();

            return NoContent();
        }
    }
}
