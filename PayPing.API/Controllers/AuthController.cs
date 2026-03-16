using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using PayPing.Application.Common.Interfaces;
using PayPing.Application.DTOs;
using PayPing.Domain.Entities;

namespace PayPing.API.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class AuthController : ControllerBase
    {
        private readonly UserManager<AppUser> _userManager;
        private readonly SignInManager<AppUser> _signInManager;
        private readonly ITokenService _tokenService;

        public AuthController(
            UserManager<AppUser> userManager,
            SignInManager<AppUser> signInManager,
            ITokenService tokenService)
        {
            _userManager = userManager;
            _signInManager = signInManager;
            _tokenService = tokenService;
        }

        [HttpPost("register")]
        public async Task<ActionResult<AuthResponseDto>> Register([FromBody] RegisterDto dto)
        {
            var existingUser = await _userManager.Users.FirstOrDefaultAsync(u => u.PhoneNumber == dto.PhoneNumber);

            if (existingUser != null)
                return BadRequest("Number already in use.");

            var user = new AppUser
            {
                UserName = dto.PhoneNumber,
                Email = dto.Email,
                FullName = dto.FullName,
                PhoneNumber = dto.PhoneNumber
            };

            var result = await _userManager.CreateAsync(user, dto.Password);

            if (!result.Succeeded)
                return BadRequest(result.Errors.Select(e => e.Description));

            return Ok(new AuthResponseDto
            {
                Token = _tokenService.CreateToken(user.Id, user.Email!, user.FullName, user.PhoneNumber),
                Email = user.Email!,
                FullName = user.FullName,
                PhoneNumber = user.PhoneNumber
            });
        }

        [HttpPost("login")]
        public async Task<ActionResult<AuthResponseDto>> Login([FromBody] LoginDto dto)
        {
            var user = await _userManager.Users.FirstOrDefaultAsync(u => u.PhoneNumber == dto.PhoneNumber);
            if (user == null) return Unauthorized("Invalid credentials.");

            var result = await _signInManager.CheckPasswordSignInAsync(user, dto.Password, false);
            if (!result.Succeeded) return Unauthorized("Invalid credentials.");

            return Ok(new AuthResponseDto
            {
                Token = _tokenService.CreateToken(user.Id, user.Email!, user.FullName, user.PhoneNumber),
                Email = user.Email!,
                FullName = user.FullName,
                PhoneNumber = user.PhoneNumber
            });
        }
    }
}
