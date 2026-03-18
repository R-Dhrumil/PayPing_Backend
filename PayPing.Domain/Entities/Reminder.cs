using System;

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
        public Guid UserId { get; set; } // Foreign key to User later
    }
}
