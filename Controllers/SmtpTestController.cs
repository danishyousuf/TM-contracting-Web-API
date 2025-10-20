using MailKit.Net.Smtp;
using MailKit.Security;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using System;
using System.Threading.Tasks;

namespace TMCC.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class SmtpTestController : ControllerBase
    {
        private readonly IConfiguration _configuration;
        private readonly ILogger<SmtpTestController> _logger;

        public SmtpTestController(IConfiguration configuration, ILogger<SmtpTestController> logger)
        {
            _configuration = configuration;
            _logger = logger;
        }

        [HttpGet("CheckGmailCredentials")]
        public async Task<IActionResult> CheckGmailCredentials()
        {
            var smtpServer = _configuration["EmailSettings:SmtpHost"] ?? "smtp.gmail.com";
            var smtpPort = int.TryParse(_configuration["EmailSettings:SmtpPort"], out var port) ? port : 587;
            var email = _configuration["EmailSettings:SenderEmail"];
            var appPassword = _configuration["EmailSettings:SenderPassword"];

            if (string.IsNullOrWhiteSpace(email) || string.IsNullOrWhiteSpace(appPassword))
            {
                return BadRequest(new { success = false, message = "Sender email or password is missing in configuration." });
            }

            using var client = new SmtpClient();
            try
            {
                _logger.LogInformation("[SMTP Test] Connecting to {SmtpServer}:{SmtpPort}", smtpServer, smtpPort);
                await client.ConnectAsync(smtpServer, smtpPort, SecureSocketOptions.StartTls);

                _logger.LogInformation("[SMTP Test] Authenticating as {Email}", email);
                await client.AuthenticateAsync(email, appPassword);

                await client.DisconnectAsync(true);

                _logger.LogInformation("[SMTP Test] ✅ Credentials are correct!");
                return Ok(new { success = true, message = "Credentials are correct! Logged in successfully." });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "[SMTP Test] ❌ Failed to authenticate as {Email}", email);
                return BadRequest(new { success = false, message = "Failed to authenticate: " + ex.Message });
            }
        }
    }
}
