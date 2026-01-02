using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authorization;
using ChipAccess.Api.Services;
using ChipAccess.Domain.Entities;
using System.Threading.Tasks;
using System.Collections.Generic;

namespace ChipAccess.Api.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class EmployeeController : ControllerBase
    {
        private readonly IEmployeeService _service;

        public EmployeeController(IEmployeeService service)
        {
            _service = service;
        }

        [Authorize(Roles = "Admin")]
        [HttpGet("getAllEmployees")]
        public async Task<ActionResult<List<Employee>>> GetAll()
        {
            return Ok(await _service.GetAllAsync());
        }

        [Authorize(Roles = "Admin")]
        [HttpGet("/getEmployeeById/{id}")]
        public async Task<ActionResult<Employee>> GetById(int id)
        {
            var employee = await _service.GetByIdAsync(id);
            if (employee == null)
                return NotFound();

            return Ok(employee);
        }

        [Authorize(Roles = "Admin")]
        [HttpGet("getByBamId/{bamId}")]
        public async Task<ActionResult<Employee>> GetByBamId(string bamId)
        {
            var employee = await _service.GetByBamIdAsync(bamId);
            if (employee == null)
                return NotFound();

            return Ok(employee);
        }

        [Authorize(Roles = "Admin")]
        [HttpPost("createEmployee")]
        public async Task<ActionResult<Employee>> Create([FromBody] Employee employee)
        {
            var created = await _service.CreateAsync(employee);
            return CreatedAtAction(nameof(GetById), new { id = created.Id }, created);
        }

        [Authorize(Roles = "Admin")]
        [HttpPut("updateEmployee/{id}")]
        public async Task<ActionResult> Update(int id, [FromBody] Employee employee)
        {
            if (id != employee.Id)
                return BadRequest("ID mismatch");

            var ok = await _service.UpdateAsync(employee);
            if (!ok) return NotFound();

            return Ok(new { message = "Employee updated." });
        }

        [Authorize(Roles = "Admin")]
        [HttpPut("deactivateEmployee{id}")]
        public async Task<ActionResult> Deactivate(int id)
        {
            var ok = await _service.DeactivateAsync(id);
            if (!ok) return NotFound();

            return Ok(new { message = "Employee deactivated." });
        }

        [Authorize(Roles = "Admin")]
        [HttpGet("search")]
        public async Task<ActionResult<List<string>>> SearchBamIds([FromQuery] string q)
        {
            if (string.IsNullOrWhiteSpace(q) || q.Length < 2)
                return Ok(new List<string>());

            var result = await _service.SearchBamIdsAsync(q);
            return Ok(result);
        }

    }
}
