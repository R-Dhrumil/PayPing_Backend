using Microsoft.AspNetCore.Mvc;
using PayPing.Application.Common.Interfaces;
using Microsoft.AspNetCore.Authorization;

namespace PayPing.API.Controllers
{
    [Authorize]
    [ApiController]
    [Route("api/[controller]")]
    public class MessagingController : ControllerBase
    {
        private readonly IWhatsAppService _whatsAppService;

        public MessagingController(IWhatsAppService whatsAppService)
        {
            _whatsAppService = whatsAppService;
        }

        [HttpPost("whatsapp")]
        public async Task<IActionResult> SendWhatsApp([FromBody] WhatsAppRequest request)
        {
            if (string.IsNullOrEmpty(request.PhoneNumber) || string.IsNullOrEmpty(request.Message))
            {
                return BadRequest("Phone number and message are required.");
            }

            var result = await _whatsAppService.SendReminderAsync(request.PhoneNumber, request.Message);

            if (result)
            {
                return Ok(new { Success = true, Message = "WhatsApp reminder sent successfully." });
            }

            return StatusCode(500, "Failed to send WhatsApp reminder.");
        }
    }

    public class WhatsAppRequest
    {
        public string PhoneNumber { get; set; } = string.Empty;
        public string Message { get; set; } = string.Empty;
    }
}
