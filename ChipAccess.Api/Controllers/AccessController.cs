using System.Collections.Generic;
using System.Security.Claims;
using System.Threading.Tasks;
using ChipAccess.Api.DTOs.Access;
using ChipAccess.Api.Services;
using ChipAccess.Domain.Entities;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace ChipAccess.Api.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class AccessController : ControllerBase
    {
        private readonly IAccessService _service;

        public AccessController(IAccessService service)
        {
            _service = service;
        }

        [Authorize(Roles = "Admin,IT")]
        [HttpGet("all")]
        public async Task<ActionResult<List<AccessResponseDto>>> GetAll()
        {
            var list = await _service.GetAllDtoAsync();
            return Ok(list);
        }

        [Authorize(Roles = "Admin,IT")]
        [HttpGet("{id:int}")]
        public async Task<ActionResult<AccessResponseDto>> GetById(int id)
        {
            var dto = await _service.GetByIdDtoAsync(id);
            if (dto == null) return NotFound();
            return Ok(dto);
        }

        [Authorize]
        [HttpGet("by-bam/{bamId}")]
        public async Task<ActionResult<List<AccessResponseDto>>> GetByBamId(string bamId)
        {
            var list = await _service.GetByBamIdDtoAsync(bamId);
            return Ok(list);
        }

        [Authorize(Roles = "Admin,IT")]
        [HttpGet("by-status/{status}")]
        public async Task<ActionResult<List<AccessResponseDto>>> GetByStatus(AccessStatus status)
        {
            var list = await _service.GetByStatusDtoAsync(status);
            return Ok(list);
        }

        [Authorize]
        [HttpGet("my")]
        public async Task<ActionResult<List<AccessResponseDto>>> GetMyAccesses()
        {
            var bamId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            if (string.IsNullOrWhiteSpace(bamId))
                return Unauthorized();

            var list = await _service.GetByBamIdDtoAsync(bamId);
            return Ok(list);
        }

        [Authorize(Roles = "Manager")]
        [HttpGet("managed-by/me")]
        public async Task<ActionResult<List<AccessResponseDto>>> GetManagedByMe()
        {
            var bamId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            if (string.IsNullOrWhiteSpace(bamId))
                return Unauthorized();

            var list = await _service.GetManagedByDtoAsync(bamId);
            return Ok(list);
        }

        [Authorize]
        [HttpPost("create")]
        public async Task<ActionResult<AccessResponseDto>> Create([FromBody] CreateAccessRequestDto dto)
        {
            var bamId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            if (string.IsNullOrWhiteSpace(bamId))
                return Unauthorized();

            if (dto.ExpirationDate <= System.DateTime.UtcNow)
                return BadRequest("Expiration date must be in the future.");

            var created = await _service.CreateFromDtoAsync(dto, bamId);
            var createdDto = await _service.GetByIdDtoAsync(created.Id);

            return CreatedAtAction(nameof(GetById), new { id = created.Id }, createdDto);
        }

        [Authorize]
        [HttpPut("edit/{id:int}")]
        public async Task<IActionResult> Update(int id, [FromBody] UpdateAccessRequestDto dto)
        {
            var bamId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            var role = User.FindFirst(ClaimTypes.Role)?.Value ?? "";

            if (string.IsNullOrWhiteSpace(bamId))
                return Unauthorized();

            var result = await _service.UpdateFromDtoAsync(id, dto, bamId, role);

            return result switch
            {
                UpdateResult.NotFound => NotFound(),
                UpdateResult.Forbidden => Forbid(),
                UpdateResult.InvalidState => BadRequest("Cannot update this request due to its current status or invalid data."),
                _ => Ok(new { message = "Access request updated." })
            };
        }

        [Authorize(Roles = "Manager,Admin,IT")]
        [HttpPut("approve/{id:int}")]
        public async Task<IActionResult> Approve(int id)
        {
            var role = User.FindFirst(ClaimTypes.Role)?.Value ?? "";
            var bamId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value ?? "";

            var ok = await _service.ApproveAsync(id, role, bamId);
            if (!ok)
                return BadRequest("Cannot approve at this workflow stage.");

            return Ok(new { message = "Access request approved." });
        }

        [Authorize(Roles = "Manager,Admin,IT")]
        [HttpPut("reject/{id:int}")]
        public async Task<IActionResult> Reject(int id)
        {
            var role = User.FindFirst(ClaimTypes.Role)?.Value ?? "";
            var bamId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value ?? "";

            var ok = await _service.RejectAsync(id, bamId, role);
            if (!ok)
                return BadRequest("Cannot reject at this workflow stage.");

            return Ok(new { message = "Access request rejected and archived." });
        }

        [Authorize(Roles = "Admin,IT")]
        [HttpPost("revoke/{id:int}")]
        public async Task<IActionResult> Revoke(int id)
        {
            var ok = await _service.RevokeAsync(id);
            if (!ok) return NotFound();
            return Ok(new { message = "Access revoked." });
        }
    }
}
