using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Microsoft.EntityFrameworkCore;
using PayPing.Application.Common.Interfaces;
using PayPing.Domain.Entities;
using PayPing.Infrastructure.Persistence;
using System;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace PayPing.Infrastructure.Services
{
    public class ReminderBackgroundService : BackgroundService
    {
        private readonly IServiceScopeFactory _scopeFactory;
        private readonly ILogger<ReminderBackgroundService> _logger;

        public ReminderBackgroundService(IServiceScopeFactory scopeFactory, ILogger<ReminderBackgroundService> logger)
        {
            _scopeFactory = scopeFactory;
            _logger = logger;
        }

        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            _logger.LogInformation("Reminder Background Service is starting.");

            while (!stoppingToken.IsCancellationRequested)
            {
                try
                {
                    await ProcessRemindersAsync();
                }
                catch (Exception ex)
                {
                    _logger.LogError(ex, "Error occurred while processing reminders.");
                }

                // Wait for 1 minute before next check
                await Task.Delay(TimeSpan.FromMinutes(1), stoppingToken);
            }

            _logger.LogInformation("Reminder Background Service is stopping.");
        }

        private async Task ProcessRemindersAsync()
        {
            using var scope = _scopeFactory.CreateScope();
            var dbContext = scope.ServiceProvider.GetRequiredService<ApplicationDbContext>();
            var whatsAppService = scope.ServiceProvider.GetRequiredService<IWhatsAppService>();

            var now = DateTime.UtcNow;

            var dueReminders = await dbContext.Reminders
                .Where(r => !r.IsPaid && r.NextReminderDate <= now && (r.EndDate == null || r.EndDate > now))
                .ToListAsync();

            if (dueReminders.Any())
            {
                _logger.LogInformation("Found {Count} due reminders.", dueReminders.Count);

                foreach (var reminder in dueReminders)
                {
                    try
                    {
                        _logger.LogInformation("Processing reminder for {Name} ({PhoneNumber})", reminder.Name, reminder.PhoneNumber);
                        
                        var sent = await whatsAppService.SendReminderAsync(reminder.PhoneNumber, reminder.Message);

                        if (sent)
                        {
                            if (reminder.Frequency.Equals("One-time", StringComparison.OrdinalIgnoreCase))
                            {
                                // One-time reminder: Mark as paid (completed) so it's not processed again
                                reminder.IsPaid = true;
                                _logger.LogInformation("One-time reminder {Id} completed.", reminder.Id);
                            }
                            else
                            {
                                // Recurring reminder: Calculate next reminder date
                                reminder.NextReminderDate = CalculateNextDate(reminder.NextReminderDate, reminder.Frequency);
                                _logger.LogInformation("Scheduled next reminder for {Id} at {NextDate}", reminder.Id, reminder.NextReminderDate);
                            }
                            
                            dbContext.Reminders.Update(reminder);
                        }
                    }
                    catch (Exception ex)
                    {
                        _logger.LogError(ex, "Failed to process reminder {Id} for {Name}", reminder.Id, reminder.Name);
                    }
                }

                await dbContext.SaveChangesAsync();
            }
        }

        private DateTime CalculateNextDate(DateTime currentDate, string frequency)
        {
            if (string.IsNullOrWhiteSpace(frequency)) return currentDate.AddMonths(1);

            var normalizedFrequency = frequency.Trim().ToLowerInvariant();

            // Handle shorthand
            if (normalizedFrequency == "daily") return currentDate.AddDays(1);
            if (normalizedFrequency == "weekly") return currentDate.AddDays(7);
            if (normalizedFrequency == "monthly") return currentDate.AddMonths(1);
            if (normalizedFrequency == "yearly") return currentDate.AddYears(1);

            // Handle "Every X Units"
            var parts = normalizedFrequency.Split(' ', StringSplitOptions.RemoveEmptyEntries);
            if (parts.Length >= 3 && parts[0] == "every")
            {
                if (int.TryParse(parts[1], out int value))
                {
                    var unit = parts[2];
                    if (unit.Contains("day")) return currentDate.AddDays(value);
                    if (unit.Contains("week")) return currentDate.AddDays(value * 7);
                    if (unit.Contains("month")) return currentDate.AddMonths(value);
                    if (unit.Contains("year")) return currentDate.AddYears(value);
                }
            }

            // Fallback: Default to monthly if parsing fails
            _logger.LogWarning("Failed to parse frequency '{Frequency}'. Defaulting to +1 month.", frequency);
            return currentDate.AddMonths(1);
        }
    }
}
