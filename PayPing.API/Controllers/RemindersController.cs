using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using PayPing.Domain.Entities;
using PayPing.Infrastructure.Persistence;
using PayPing.Application.Common.Interfaces;
using PayPing.Application.DTOs;
using Microsoft.AspNetCore.Authorization;
using System.Security.Claims;

namespace PayPing.API.Controllers
{
    [Authorize]
    [ApiController]
    [Route("api/[controller]")]
    public class RemindersController : ControllerBase
    {
        private readonly ApplicationDbContext _context;

        public RemindersController(ApplicationDbContext context)
        {
            _context = context;
        }

        private string GetCurrentUserId() => User.FindFirst(ClaimTypes.NameIdentifier)?.Value ?? string.Empty;

        [HttpGet]
        public async Task<ActionResult<IEnumerable<ReminderDto>>> GetReminders()
        {
            var userId = GetCurrentUserId();
            var reminders = await _context.Reminders
                .Where(r => r.UserId == userId)
                .ToListAsync();

            return Ok(reminders.Select(r => new ReminderDto
            {
                Id = r.Id,
                Name = r.Name,
                Amount = r.Amount,
                PhoneNumber = r.PhoneNumber,
                Message = r.Message,
                Frequency = r.Frequency,
                NextReminderDate = r.NextReminderDate,
                EndDate = r.EndDate,
                AvatarUrl = r.AvatarUrl,
                IsPaid = r.IsPaid,
                CreatedAt = r.CreatedAt,
                UserId = r.UserId
            }));
        }

        [HttpGet("{id}")]
        public async Task<ActionResult<ReminderDto>> GetReminder(Guid id)
        {
            var userId = GetCurrentUserId();
            var reminder = await _context.Reminders
                .FirstOrDefaultAsync(r => r.Id == id && r.UserId == userId);

            if (reminder == null)
            {
                return NotFound();
            }

            return Ok(new ReminderDto
            {
                Id = reminder.Id,
                Name = reminder.Name,
                Amount = reminder.Amount,
                PhoneNumber = reminder.PhoneNumber,
                Message = reminder.Message,
                Frequency = reminder.Frequency,
                NextReminderDate = reminder.NextReminderDate,
                EndDate = reminder.EndDate,
                AvatarUrl = reminder.AvatarUrl,
                IsPaid = reminder.IsPaid,
                CreatedAt = reminder.CreatedAt,
                UserId = reminder.UserId
            });
        }

        [HttpPost]
        public async Task<ActionResult<ReminderDto>> PostReminder(CreateReminderDto createDto)
        {
            var userId = GetCurrentUserId();
            
            var reminder = new Reminder
            {
                Id = Guid.NewGuid(),
                Name = createDto.Name,
                Amount = createDto.Amount,
                PhoneNumber = createDto.PhoneNumber,
                Message = createDto.Message,
                Frequency = createDto.Frequency,
                NextReminderDate = createDto.NextReminderDate,
                EndDate = createDto.EndDate,
                AvatarUrl = createDto.AvatarUrl,
                UserId = userId, // Always use the authenticated user's ID
                CreatedAt = DateTime.UtcNow,
                IsPaid = false
            };

            _context.Reminders.Add(reminder);
            await _context.SaveChangesAsync();

            var responseDto = new ReminderDto
            {
                Id = reminder.Id,
                Name = reminder.Name,
                Amount = reminder.Amount,
                PhoneNumber = reminder.PhoneNumber,
                Message = reminder.Message,
                Frequency = reminder.Frequency,
                NextReminderDate = reminder.NextReminderDate,
                EndDate = reminder.EndDate,
                AvatarUrl = reminder.AvatarUrl,
                IsPaid = reminder.IsPaid,
                CreatedAt = reminder.CreatedAt,
                UserId = reminder.UserId
            };

            return CreatedAtAction(nameof(GetReminder), new { id = reminder.Id }, responseDto);
        }

        [HttpPost("{id}/send-now")]
        public async Task<IActionResult> SendNow(Guid id, [FromServices] IWhatsAppService whatsAppService)
        {
            var userId = GetCurrentUserId();
            var reminder = await _context.Reminders
                .FirstOrDefaultAsync(r => r.Id == id && r.UserId == userId);
            
            if (reminder == null) return NotFound();

            var result = await whatsAppService.SendReminderAsync(reminder.PhoneNumber, reminder.Message);

            if (result)
            {
                return Ok(new { Success = true, Message = "WhatsApp reminder sent manually." });
            }

            return StatusCode(500, "Failed to send WhatsApp reminder.");
        }

        [HttpPatch("{id}/mark-paid")]
        public async Task<IActionResult> MarkAsPaid(Guid id)
        {
            var userId = GetCurrentUserId();
            var reminder = await _context.Reminders
                .FirstOrDefaultAsync(r => r.Id == id && r.UserId == userId);
            
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
            var userId = GetCurrentUserId();
            var reminder = await _context.Reminders
                .FirstOrDefaultAsync(r => r.Id == id && r.UserId == userId);
            
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
