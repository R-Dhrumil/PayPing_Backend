using System;

namespace PayPing.Application.DTOs
{
    public class ReminderDto
    {
        public Guid Id { get; set; }
        public string Name { get; set; } = string.Empty;
        public decimal Amount { get; set; }
        public string PhoneNumber { get; set; } = string.Empty;
        public string Message { get; set; } = string.Empty;
        public string Frequency { get; set; } = string.Empty;
        public DateTime NextReminderDate { get; set; }
        public DateTime? EndDate { get; set; }
        public string? AvatarUrl { get; set; }
        public bool IsPaid { get; set; }
        public DateTime CreatedAt { get; set; }
        public string UserId { get; set; } = string.Empty;
    }

    public class CreateReminderDto
    {
        public string Name { get; set; } = string.Empty;
        public decimal Amount { get; set; }
        public string PhoneNumber { get; set; } = string.Empty;
        public string Message { get; set; } = string.Empty;
        public string Frequency { get; set; } = string.Empty;
        public DateTime NextReminderDate { get; set; }
        public DateTime? EndDate { get; set; }
        public string? AvatarUrl { get; set; }
        public string? UserId { get; set; } = string.Empty;

    }
}
