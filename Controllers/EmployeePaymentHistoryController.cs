using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using TMCC.Models.DTO;
using TMCC.Services.IServices;

namespace TMCC.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class EmployeePaymentHistoryController : ControllerBase
    {
        private readonly IEmployeePaymentHistoryService _service;

        public EmployeePaymentHistoryController(IEmployeePaymentHistoryService service)
        {
            _service = service;
        }

        [Authorize]
        [HttpGet("{empId}")]
        public async Task<IActionResult> GetEmployeePayments(Guid empId)
        {
            var result = await _service.GetEmployeePaymentsAsync(empId);
            return Ok(result);
        }

        [Authorize]
        [HttpGet("latest")]
        public async Task<IActionResult> GetLatestEmployeePayments()
        {
            var result = await _service.GetLatestEmployeePaymentsAsync();
            return Ok(result);
        }

        [Authorize]
        [HttpPost]
        public async Task<IActionResult> AddPayment([FromBody] EmployeePaymentHistoryDto payment)
        {
            var result = await _service.AddEmployeePaymentAsync(payment);
            return result > 0 ? Ok("Payment added successfully") : BadRequest("Failed to add payment");
        }

        [Authorize]
        [HttpPut]
        public async Task<IActionResult> UpdatePayment([FromBody] EmployeePaymentHistoryDto payment)
        {
            var result = await _service.UpdateEmployeePaymentAsync(payment);
            return result > 0 ? Ok("Payment updated successfully") : BadRequest("Failed to update payment");
        }

        [Authorize]
        [HttpDelete("{paymentId}")]
        public async Task<IActionResult> DeletePayment(Guid paymentId)
        {
            var result = await _service.DeleteEmployeePaymentAsync(paymentId);
            return result > 0 ? Ok("Payment deleted successfully") : BadRequest("Failed to delete payment");
        }
    }
}
