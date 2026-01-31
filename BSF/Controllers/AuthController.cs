using Application.Services.AuthService;
using Application.Services.AuthService.DTOs;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace BSF.Controllers
{
    [Authorize]
    [Route("api/[controller]")]
    [ApiController]
    public class AuthController : ControllerBase
    {
        private readonly IAuthService _authService;
        public AuthController(IAuthService authService)
        {
            _authService = authService;
        }

        [AllowAnonymous]
        [HttpPost("login")]
        public async Task<IActionResult> Login([FromBody] LoginRequest request)
        {
            var result = await _authService.Login(request);

            if (result == null)
            {
                return Unauthorized(new { Message = "Invalid username or password." });
            }

            return Ok(result);
        }

        [HttpPost("RefreshToken")]
        public async Task<IActionResult> RefreshToken(string refreshToken)
        {
            var newAccessToken = await _authService.GenerateNewAccessToken(refreshToken);
            if (newAccessToken == null)
            {
                return Unauthorized(new { Message = "Invalid refresh token." });
            }
            return Ok(newAccessToken);
        }

        [HttpPost("ChangeMyPassword")]
        public async Task<IActionResult> ChangeMyPassword([FromBody] ChangeMyPasswordRequest request)
        {
            await _authService.ChangeMyPassword(request);
            return Ok();
        }

        [HttpPost("RegisterFirbaseToken")]
        public async Task<IActionResult> RegisterFirbaseToken([FromBody]RegisterFirebaseTokenRequest request)
        {
            await _authService.RegisterFirbaseToken(request);
            return Ok();
        }

        [Authorize(Roles = "Admin")]
        [HttpPost("ResetUserPasswordByAdmin")]
        public async Task<IActionResult> ResetUserPasswordByAdmin([FromBody] ResetUserPasswordRequest request)
        {
            await _authService.ResetUserPasswordByAdmin(request);
            return Ok();
        }

    }
}
