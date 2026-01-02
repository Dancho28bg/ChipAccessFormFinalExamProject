using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using ChipAccess.Api.Services;
using System.Security.Claims;
using System.Threading.Tasks;

namespace ChipAccess.Api.Controllers
{
    [ApiController]
    [Route("api/access/archive")]
    [Authorize]
    public class AccessArchiveController : ControllerBase
    {
        private readonly IAccessArchiveService _archive;

        public AccessArchiveController(IAccessArchiveService archive)
        {
            _archive = archive;
        }

        [Authorize(Roles = "Admin,IT")]
        [HttpGet("all")]
        public async Task<IActionResult> GetAll()
            => Ok(await _archive.GetAllAsync());
            

        [HttpGet("my")]
        public async Task<IActionResult> GetMy()
        {
            var bamId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            if (string.IsNullOrEmpty(bamId)) return Unauthorized();

            return Ok(await _archive.GetByBamIdAsync(bamId));
        }

        [Authorize(Roles = "Manager")]
        [HttpGet("managed-by/me")]
        public async Task<IActionResult> GetManagedByMe()
        {
            var bamId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            if (string.IsNullOrEmpty(bamId)) return Unauthorized();

            return Ok(await _archive.GetManagedByAsync(bamId));
        }
    }
}
