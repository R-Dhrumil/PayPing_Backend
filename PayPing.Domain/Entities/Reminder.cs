using System;
using System.Linq;

namespace PayPing.Domain.Entities
{
    public class Reminder
    {
        public Guid Id { get; set; }
        public string Name { get; set; } = string.Empty;
        public decimal Amount { get; set; }
        public string PhoneNumber { get; set; } = string.Empty;
        public string Message { get; set; } = string.Empty;
        public string Frequency { get; set; } = string.Empty; // e.g., "Every 3 days", "Monthly"

        public DateTime NextReminderDate { get; set; }
        public DateTime? EndDate { get; set; }
        public string? AvatarUrl { get; set; }
        public bool IsPaid { get; set; } = false;
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
        public string UserId { get; set; } = string.Empty;
        public virtual AppUser User { get; set; } = null!;

        public DateTime CalculateNextDate()
        {
            if (string.IsNullOrWhiteSpace(Frequency) || Frequency.Equals("One-time", StringComparison.OrdinalIgnoreCase))
                return NextReminderDate;

            var normalizedFrequency = Frequency.Trim().ToLowerInvariant();

            // Handle shorthand
            if (normalizedFrequency == "daily") return NextReminderDate.AddDays(1);
            if (normalizedFrequency == "weekly") return NextReminderDate.AddDays(7);
            if (normalizedFrequency == "monthly") return NextReminderDate.AddMonths(1);
            if (normalizedFrequency == "yearly") return NextReminderDate.AddYears(1);

            // Handle "Every X Units"
            var parts = normalizedFrequency.Split(' ', StringSplitOptions.RemoveEmptyEntries);
            if (parts.Length >= 3 && parts[0] == "every")
            {
                if (int.TryParse(parts[1], out int value))
                {
                    var unit = parts[2];
                    if (unit.Contains("day")) return NextReminderDate.AddDays(value);
                    if (unit.Contains("week")) return NextReminderDate.AddDays(value * 7);
                    if (unit.Contains("month")) return NextReminderDate.AddMonths(value);
                    if (unit.Contains("year")) return NextReminderDate.AddYears(value);
                }
            }

            // Fallback: Default to monthly if parsing fails
            return NextReminderDate.AddMonths(1);
        }
    }
}
