using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using ChipAccess.Api.Services;
using System.Threading.Tasks;
using ChipAccess.Api.DTOs.AuditLog;
using System.Text;


namespace ChipAccess.Api.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class AuditLogController : ControllerBase
    {
        private readonly IAuditLogService _auditService;

        public AuditLogController(IAuditLogService auditService)
        {
            _auditService = auditService;
        }

        [Authorize(Roles = "Admin,IT")]
        [HttpPost("export")]
        public async Task<IActionResult> Export([FromBody] AuditLogQueryDto filters)
        {
            var logs = await _auditService.QueryAllAsync(filters);

            var sb = new StringBuilder();
            sb.AppendLine("Timestamp,PerformedBy,Action,TargetType,TargetId,Details");

            foreach (var log in logs)
            {
                sb.AppendLine(
                    $"{log.Timestamp:u}," +
                    $"{log.PerformedBy}," +
                    $"{log.Action}," +
                    $"{log.TargetType}," +
                    $"{log.TargetId?.ToString() ?? ""}," +
                    $"{log.Details}"
                );
            }

            var bytes = Encoding.UTF8.GetBytes(sb.ToString());
            return File(bytes, "text/plain", "audit-log.txt");
        }

        [Authorize(Roles = "Admin,IT")]
        [HttpPost("query")]
        public async Task<IActionResult> Query([FromBody] AuditLogQueryDto filters)
        {
            var result = await _auditService.QueryAsync(filters);
            return Ok(result);
        }
        
    }
    
}