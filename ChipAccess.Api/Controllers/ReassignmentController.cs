using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using ChipAccess.Api.Services;
using ChipAccess.Api.DTOs.Reassignment;
using ChipAccess.Api.Mappers;

namespace ChipAccess.Api.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class ReassignmentController : ControllerBase
    {
        private readonly IReassignmentService _service;

        public ReassignmentController(IReassignmentService service)
        {
            _service = service;
        }

        [Authorize(Roles = "Admin")]
        [HttpPost("reassignAccess")]
        public async Task<IActionResult> CreateJob([FromBody] CreateReassignmentDto dto)
        {
            var adminBamId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            var job = await _service.CreateJobFromDtoAsync(dto, adminBamId);
            return Ok(job.ToDto());
        }

        [Authorize(Roles = "Admin")]
        [HttpGet("pending")]
        public async Task<IActionResult> GetPending()
        {
            var jobs = await _service.GetPendingAsync();
            return Ok(jobs.Select(j => j.ToDto()));
        }

        [Authorize]
        [HttpGet("my-pending")]
        public async Task<IActionResult> GetMyPending()
        {
            var bamId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            var jobs = await _service.GetPendingForNewUserAsync(bamId);
            return Ok(jobs.Select(j => j.ToDto()));
        }

        [Authorize]
        [HttpPost("accept/{id}")]
        public async Task<IActionResult> Accept(int id)
        {
            var bamId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            var ok = await _service.AcceptAsync(id, bamId);
            return Ok(new { accepted = ok });
        }

        [Authorize]
        [HttpPost("reject/{id}")]
        public async Task<IActionResult> Reject(int id, [FromBody] RejectDto dto)
        {
            var bamId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            var ok = await _service.RejectAsync(id, bamId, dto.Reason);
            return Ok(new { rejected = ok });
        }

    }
}
