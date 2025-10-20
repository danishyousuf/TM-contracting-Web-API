using Microsoft.Extensions.Logging;
using Quartz;
using System;
using System.Linq;
using System.Text.Json;
using System.Threading.Tasks;
using TMCC.Repository.IRepository;
using TMCC.Services;

public class CompanyDocumentExpiryJob : IJob
{
    private readonly IEmailService _emailService;
    private readonly ICompanyRepository _companyRepository;
    private readonly ILogger<CompanyDocumentExpiryJob> _logger;
    private readonly int _daysBeforeExpiry = 30;

    public CompanyDocumentExpiryJob(
        IEmailService emailService,
        ICompanyRepository companyRepository,
        ILogger<CompanyDocumentExpiryJob> logger)
    {
        _emailService = emailService;
        _companyRepository = companyRepository;
        _logger = logger;
    }

    public async Task Execute(IJobExecutionContext context)
    {
        _logger.LogInformation("[CompanyDocumentExpiryJob] Started at {Time}", DateTime.Now);

        try
        {
            // Fetch all company documents expiring in the next 30 days
            var expiringDocs = await _companyRepository.GetCompanyDocumentsExpiringInDays(_daysBeforeExpiry);

            if (!expiringDocs.Any())
            {
                _logger.LogInformation("[CompanyDocumentExpiryJob] No expiring company documents found.");
                return;
            }

            // Fetch company details once (as they’re usually single-record global info)
            var companyDetails = await _companyRepository.GetCompanyDetails();

            if (companyDetails == null)
            {
                _logger.LogWarning("[CompanyDocumentExpiryJob] No company details found in the database.");
                return;
            }

            // ✅ Cast dynamic to object or string for safe logging
            string companyDetailsJson;
            try
            {
                companyDetailsJson = JsonSerializer.Serialize((object)companyDetails, new JsonSerializerOptions
                {
                    WriteIndented = true
                });
            }
            catch (Exception ex)
            {
                companyDetailsJson = $"[Serialization Error: {ex.Message}]";
            }

            _logger.LogInformation("[CompanyDocumentExpiryJob] Company details fetched: {CompanyDetails}", companyDetailsJson);

            foreach (var doc in expiringDocs)
            {
                try
                {
                    await _emailService.SendDocumentExpiryEmailAsync(
    companyDetails.company_name ?? "Company",
    doc.DocumentName,
    companyDetails.mobile_phone ?? "N/A",
    doc.ExpiryDate,
    doc.ExpiryDate < DateTime.Now,
    "Company",
    companyDetails.primary_email ?? "info@tmcontracting.com.sa"
);


                    _logger.LogInformation(
                        "[CompanyDocumentExpiryJob] Email sent for document '{DocumentName}' (CompanyId: {CompanyId})",
                        doc.DocumentName?.ToString(), doc.CompanyId.ToString()
                    );
                }
                catch (Exception ex)
                {
                    _logger.LogError(ex,
                        "[CompanyDocumentExpiryJob] Error sending email for document '{DocumentName}' (CompanyId: {CompanyId})",
                        doc.DocumentName?.ToString(), doc.CompanyId.ToString());
                }
            }

            _logger.LogInformation(
                "[CompanyDocumentExpiryJob] Completed at {Time}. Total processed: {Count}",
                DateTime.Now, expiringDocs.Count());
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "[CompanyDocumentExpiryJob] Failed to execute job.");
        }
    }
}
