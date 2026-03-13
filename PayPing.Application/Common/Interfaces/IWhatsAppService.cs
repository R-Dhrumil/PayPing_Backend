namespace PayPing.Application.Common.Interfaces
{
    public interface IWhatsAppService
    {
        Task<bool> SendReminderAsync(string phoneNumber, string message);
    }
}
