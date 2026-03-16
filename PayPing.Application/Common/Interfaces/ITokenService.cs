using PayPing.Application.DTOs;

namespace PayPing.Application.Common.Interfaces
{
    public interface ITokenService
    {
        string CreateToken(string userId, string email, string fullName, string PhoneNumber);
    }
}
