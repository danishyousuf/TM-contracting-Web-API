using Microsoft.Extensions.Logging;
using Quartz;
using System;
using System.Linq;
using System.Threading.Tasks;
using TMCC.Repository.IRepository;
using TMCC.Services;
using TMCC.Models;

public class ClientDocumentExpiryJob : IJob
{
    private readonly IEmailService _emailService;
    private readonly IClientRepository _clientRepository;
    private readonly ILogger<ClientDocumentExpiryJob> _logger;
    private readonly int _daysBeforeExpiry = 30;

    public ClientDocumentExpiryJob(
        IEmailService emailService,
        IClientRepository clientRepository,
        ILogger<ClientDocumentExpiryJob> logger)
    {
        _emailService = emailService;
        _clientRepository = clientRepository;
        _logger = logger;
    }

    public async Task Execute(IJobExecutionContext context)
    {
        _logger.LogInformation("[ClientDocumentExpiryJob] Started at {Time}", DateTime.Now);

        try
        {
            // Fetch documents that are expiring within the next 30 days
            var expiringDocs = await _clientRepository.GetDocumentsExpiringInDaysAsync(_daysBeforeExpiry);

            if (!expiringDocs.Any())
            {
                _logger.LogInformation("[ClientDocumentExpiryJob] No expiring client documents found.");
                return;
            }

            foreach (var doc in expiringDocs)
            {
                try
                {
                    // Retrieve client details using ClientId
                    var clientDetails = await _clientRepository.GetClientByIdAsync(doc.ClientId);

                    if (clientDetails == null)
                    {
                        _logger.LogWarning("[ClientDocumentExpiryJob] Client not found for ClientId: {ClientId}", doc.ClientId);
                        continue;
                    }

                    // Send expiry notification email
                    await _emailService.SendDocumentExpiryEmailAsync(
                        clientDetails.ClientName,
                        doc.DocumentName,
                        clientDetails.Mobile ?? "N/A",
                        doc.ExpiryDate ?? DateTime.Now,
                        doc.ExpiryDate < DateTime.Now,
                        "Client",
                        clientDetails.Email ?? "info@tmcontracting.com.sa"
                    );

                    _logger.LogInformation("[ClientDocumentExpiryJob] Email sent for '{DocumentName}' (Client: {ClientName})",
                        doc.DocumentName, clientDetails.ClientName);
                }
                catch (Exception ex)
                {
                    _logger.LogError(ex,
                        "[ClientDocumentExpiryJob] Error sending email for document '{DocumentName}' (ClientId: {ClientId})",
                        doc.DocumentName, doc.ClientId);
                }
            }

            _logger.LogInformation("[ClientDocumentExpiryJob] Completed at {Time}. Total processed: {Count}",
                DateTime.Now, expiringDocs.Count());
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "[ClientDocumentExpiryJob] Failed to execute job.");
        }
    }
}
