using System;

namespace PayPing.Domain.Entities
{
    public class PaymentHistory
    {
        public Guid Id { get; set; }
        public Guid ReminderId { get; set; }
        public Reminder? Reminder { get; set; }
        public decimal AmountPaid { get; set; }
        public DateTime PaymentDate { get; set; } = DateTime.UtcNow;
        public string? Notes { get; set; }
    }
}
