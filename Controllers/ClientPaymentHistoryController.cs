using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using TMCC.Models;
using TMCC.Services.IServices;

namespace TMCC.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ClientPaymentHistoryController : ControllerBase
    {
        private readonly IClientPaymentHistoryService _service;

        public ClientPaymentHistoryController(IClientPaymentHistoryService service)
        {
            _service = service;
        }

        [Authorize]
        [HttpPost("add")]
        public async Task<IActionResult> AddClientPayment([FromBody] ClientPaymentHistory payment)
        {
            var result = await _service.AddClientPaymentAsync(payment);
            return result > 0 ? Ok("Payment added successfully") : BadRequest("Failed to add payment");
        }

        [Authorize]
        [HttpGet("latest")]
        public async Task<IActionResult> GetLatestPayments()
        {
            var result = await _service.GetLatestPaymentsAsync();
            return Ok(result);
        }

        [Authorize]
        [HttpGet("history/{clientId}")]
        public async Task<IActionResult> GetClientPaymentHistory(string clientId)
        {
            var result = await _service.GetClientPaymentHistoryAsync(clientId);
            return Ok(result);
        }
        [Authorize]
        [HttpDelete("delete/{paymentId}/{deletedBy}")]
        public async Task<IActionResult> DeleteClientPayment(string paymentId, string deletedBy)
        {
            var result = await _service.DeleteClientPaymentAsync(paymentId, deletedBy);
            return result > 0 ? Ok("Payment deleted successfully") : BadRequest("Failed to delete payment");
        }
    }
}
