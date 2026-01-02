using Microsoft.AspNetCore.Mvc;
using ChipAccess.Api.Services;
using ChipAccess.Api.DTOs.Auth;

namespace ChipAccess.Api.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class AuthController : ControllerBase
    {
        private readonly IAuthService _authService;

        public AuthController(IAuthService authService)
        {
            _authService = authService;
        }

        [HttpPost("login")]
        public async Task<IActionResult> Login([FromBody] LoginRequestDto request)
        {
            if (string.IsNullOrWhiteSpace(request.BamId))
                return BadRequest("BamId is required.");

            var result = await _authService.LoginAsync(request.BamId);

            if (result == null)
                return Unauthorized(new { message = "Invalid BamId." });

            return Ok(result);
        }
    }
}