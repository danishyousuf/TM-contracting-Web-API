using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using TMCC.Models;
using TMCC.Services.IServices;

namespace TMCC.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class EmployeeHistoryAndStatusController : ControllerBase
    {
        private readonly IEmployeeHistoryAndStatusService _service;

        public EmployeeHistoryAndStatusController(IEmployeeHistoryAndStatusService service)
        {
            _service = service;
        }

        [Authorize]
        [HttpGet("available")]
        public async Task<IActionResult> GetAvailableEmployees()
        {
            var result = await _service.GetAvailableEmployeesAsync();
            return Ok(result);
        }

        [Authorize]
        [HttpGet("occupied")]
        public async Task<IActionResult> GetOccupiedEmployees()
        {
            var result = await _service.GetOccupiedEmployeesAsync();
            return Ok(result);
        }

        [Authorize]
        [HttpPost("free/{empId}")]
        public async Task<IActionResult> MarkEmployeeFree(string empId, [FromQuery] DateTime lastWorkingDate)
        {
            if (lastWorkingDate == default)
                return BadRequest(new { message = "Last working date is required." });

            await _service.MarkEmployeeFreeAsync(empId, lastWorkingDate);
            return Ok(new { message = "Employee marked as free." });
        }


        [Authorize]
        [HttpPost("busy")]
        public async Task<IActionResult> MarkEmployeeBusy([FromBody] AssignEmployeeRequest request)
        {
            Serilog.Log.Information("Employee Assign Start Date is : {StartDate}", request.StartDate);
            await _service.MarkEmployeeBusyAsync(
                request.EmpId,
                request.ClientId,
                request.StartDate
            );
            return Ok(new { message = "Employee assigned to client." });
        }


        [Authorize]
        [HttpGet("history/{empId}")]
        public async Task<IActionResult> GetEmployeeHistory(string empId)
        {
            var result = await _service.GetEmployeeHistoryAsync(empId);
            return Ok(result);
        }

        [Authorize]
        [HttpGet("byclient/{clientId}")]
        public async Task<IActionResult> GetEmployeesByClient(string clientId)
        {
            var result = await _service.GetEmployeesByClientAsync(clientId);
            return Ok(result);
        }
    }
}
