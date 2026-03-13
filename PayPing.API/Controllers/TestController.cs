using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using PayPing.Infrastructure.Persistence;
using PayPing.Domain.Entities;

namespace PayPing.API.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class TestController : ControllerBase
    {
        private readonly ApplicationDbContext _context;

        public TestController(ApplicationDbContext context)
        {
            _context = context;
        }

        [HttpGet]
        public IActionResult Get()
        {
            return Ok("API is running");
        }

        [HttpGet("insert")]
        public async Task<IActionResult> TestInsert()
        {
            try
            {
                var user = new AppUser
                {
                    UserName = "test_" + Guid.NewGuid().ToString().Substring(0, 8),
                    Email = "test@test.com",
                    FullName = "Test User"
                };
                Console.WriteLine("[Test] Starting direct insert...");
                _context.Users.Add(user);
                await _context.SaveChangesAsync();
                Console.WriteLine("[Test] Direct insert finished.");
                return Ok("Direct insert worked!");
            }
            catch (Exception ex)
            {
                return BadRequest($"Insert Test Failed: {ex.Message}");
            }
        }

        [HttpGet("hash")]
        public IActionResult TestHash([FromServices] IPasswordHasher<AppUser> hasher)
        {
            try
            {
                Console.WriteLine("[Test] Starting hash test...");
                var user = new AppUser { Email = "test@test.com" };
                var hashed = hasher.HashPassword(user, "Password123!");
                Console.WriteLine("[Test] Hash test finished.");
                return Ok($"Hashed password: {hashed.Substring(0, 10)}...");
            }
            catch (Exception ex)
            {
                return BadRequest($"Hash Test Failed: {ex.Message}");
            }
        }
    }
}
