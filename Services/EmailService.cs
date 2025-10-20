using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using System;
using System.Net.Http;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;

namespace TMCC.Services
{
    public interface IEmailService
    {
        Task SendDocumentExpiryEmailAsync(
            string entityName,
            string documentName,
            string phoneNumber,
            DateTime expiryDate,
            bool isExpired,
            string documentType,
            string entityEmail); // display only
    }

    public class EmailService : IEmailService
    {
        private readonly IConfiguration _configuration;
        private readonly ILogger<EmailService> _logger;
        private readonly HttpClient _httpClient;

        public EmailService(IConfiguration configuration, ILogger<EmailService> logger, HttpClient httpClient)
        {
            _configuration = configuration;
            _logger = logger;
            _httpClient = httpClient;
        }

        public async Task SendDocumentExpiryEmailAsync(
            string entityName,
            string documentName,
            string phoneNumber,
            DateTime expiryDate,
            bool isExpired,
            string documentType,
            string entityEmail)
        {
            try
            {
                var senderEmail = _configuration["EmailSettings:SenderEmail"];
                var accessToken = _configuration["EmailSettings:AccessToken"];
                var receiverEmail = _configuration["EmailSettings:ReceiverEmail"];

                // Preserve your original HTML body
                string htmlBody = GenerateEmailBody(
                    entityName,
                    documentName,
                    phoneNumber,
                    expiryDate,
                    isExpired,
                    documentType,
                    entityEmail
                );

                var payload = new
                {
                    message = new
                    {
                        subject = isExpired
                            ? $"[{documentType}] URGENT: {documentName} Has Expired"
                            : $"[{documentType}] Reminder: {documentName} Expiring Soon",
                        body = new
                        {
                            contentType = "HTML",
                            content = htmlBody
                        },
                        toRecipients = new[]
                        {
                            new { emailAddress = new { address = receiverEmail } }
                        }
                    }
                };

                var jsonPayload = JsonSerializer.Serialize(payload);

                var request = new HttpRequestMessage(HttpMethod.Post,
                    $"https://graph.microsoft.com/v1.0/users/{senderEmail}/sendMail")
                {
                    Content = new StringContent(jsonPayload, Encoding.UTF8, "application/json")
                };

                request.Headers.Add("Authorization", $"Bearer {accessToken}");

                var response = await _httpClient.SendAsync(request);
                var responseContent = await response.Content.ReadAsStringAsync();

                if (!response.IsSuccessStatusCode)
                {
                    _logger.LogError("[EmailService] Failed to send email: {Response}", responseContent);
                    throw new Exception($"Email sending failed: {responseContent}");
                }

                _logger.LogInformation("[EmailService] Email sent successfully for document '{DocumentName}'", documentName);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "[EmailService] Error sending email for document '{DocumentName}'", documentName);
                throw;
            }
        }

        // Keep your existing HTML body generator unchanged
        private string GenerateEmailBody(
            string entityName,
            string documentName,
            string phoneNumber,
            DateTime expiryDate,
            bool isExpired,
            string documentType,
            string entityEmail)
        {
            var status = isExpired ? "has expired" : "will expire in 30 days";
            var statusColor = isExpired ? "#dc3545" : "#ffc107";

            return $@"
<html>
<head>
    <style>
        body {{
            font-family: 'Segoe UI', Arial, sans-serif;
            background-color: #f4f6f8;
            color: #333333;
            margin: 0;
            padding: 0;
        }}
        .email-container {{
            max-width: 650px;
            margin: 40px auto;
            background-color: #ffffff;
            border-radius: 10px;
            box-shadow: 0 4px 10px rgba(0,0,0,0.08);
            overflow: hidden;
            border: 1px solid #e5e5e5;
        }}
        .header {{
            background: {statusColor};
            color: #ffffff;
            text-align: center;
            padding: 25px 15px;
        }}
        .header h2 {{
            margin: 0;
            font-size: 22px;
            letter-spacing: 0.5px;
        }}
        .content {{
            padding: 25px 30px;
        }}
        .content p {{
            font-size: 15px;
            line-height: 1.6;
            margin: 8px 0;
        }}
        .content b {{
            color: #222;
        }}
        .highlight {{
            background-color: #f0f7ff;
            border-left: 4px solid {statusColor};
            padding: 12px 16px;
            margin-top: 15px;
            border-radius: 6px;
        }}
        .status-text {{
            color: {statusColor};
            font-weight: bold;
            margin-top: 15px;
        }}
        .footer {{
            background-color: #fafafa;
            text-align: center;
            font-size: 12px;
            color: #777;
            padding: 15px 10px;
            border-top: 1px solid #eee;
        }}
    </style>
</head>
<body>
    <div class='email-container'>
        <div class='header'>
            <h2>{documentType} Document Expiry Notification</h2>
        </div>
        <div class='content'>
            <p>Dear <strong>info@tmcontracting.com.sa</strong>,</p>

            <p>
                Please be informed that the document associated with 
                <strong>{entityName}</strong> (<b>{documentType}</b>) has the following status:
            </p>

            <div class='highlight'>
                <p><b>Status:</b> {status}</p>
                <p><b>Document Name:</b> {documentName}</p>
                <p><b>Expiry Date:</b> {expiryDate:dd-MMM-yyyy}</p>
            </div>

            <p>
                <b>{entityName}'s Contact Details:</b><br/>
                📞 {phoneNumber}<br/>
                ✉️ {entityEmail}
            </p>

            <p class='status-text'>
                {(isExpired ?
                "⚠️ This document has expired. Kindly take immediate action to renew or update it." :
                "⏰ This document is approaching its expiry date. Please initiate the renewal process promptly.")}
            </p>

            <p>
                Thank you for your attention to this matter.<br/>
                <b>TM Contracting</b><br/>
                ✉️ info@tmcontracting.com.sa
            </p>
        </div>
        <div class='footer'>
            This is an automated notification. Please do not reply to this email.
        </div>
    </div>
</body>
</html>
";
        }
    }
}
