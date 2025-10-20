using Microsoft.Extensions.Logging;
using Quartz;
using System;
using System.Linq;
using System.Threading.Tasks;
using TMCC.Repository.IRepository;
using TMCC.Services;
using TMCC.Models;

public class EmployeeDocumentExpiryJob : IJob
{
    private readonly IEmailService _emailService;
    private readonly IEmployeeRepository _employeeRepository;
    private readonly ILogger<EmployeeDocumentExpiryJob> _logger;

    public EmployeeDocumentExpiryJob(
        IEmailService emailService,
        IEmployeeRepository employeeRepository,
        ILogger<EmployeeDocumentExpiryJob> logger)
    {
        _emailService = emailService;
        _employeeRepository = employeeRepository;
        _logger = logger;
    }

    public async Task Execute(IJobExecutionContext context)
    {
        _logger.LogInformation("[EmployeeDocumentExpiryJob] Started at {Time}", DateTime.Now);

        try
        {
            // Target expiry date (documents expiring within 30 days)
            DateTime expiryThreshold = DateTime.Now.AddDays(30);

            var expiringDocs = await _employeeRepository.GetDocumentsExpiringBeforeDateAsync(expiryThreshold);
            _logger.LogInformation("expiring info {@ExpiringDocs}", expiringDocs);
            if (!expiringDocs.Any())
            {
                _logger.LogInformation("[EmployeeDocumentExpiryJob] No expiring employee documents found before {Date}", expiryThreshold);
                return;
            }

            foreach (var doc in expiringDocs)
            {
                try
                {
                    // Fetch full employee details using EmpId
                    var employee = await _employeeRepository.GetEmployeeByIdAsync(doc.EmployeeId);
                    if (employee == null)
                    {
                        _logger.LogWarning("[EmployeeDocumentExpiryJob] Employee not found for EmpId: {EmpId}", doc.EmployeeId);
                        continue;
                    }

                    _logger.LogInformation("[EmployeeDocumentExpiryJob] Sending email to {EmpName} ({EmpEmail}) for document '{DocumentName}'",
                        employee.FullName, employee.Email, doc.DocumentName);

                    // Send email
                    await _emailService.SendDocumentExpiryEmailAsync(
                        employee.FullName,                // Employee Name
                        doc.DocumentName,                 // Document Name
                        employee.Mobile,                  // Employee Mobile
                        doc.ExpiryDate ?? DateTime.Now,  // Expiry Date fallback
                        doc.ExpiryDate < DateTime.Now,   // IsExpired flag
                        "Employee",                       // Document Type
                        employee.Email                    // Employee Email
                    );

                    _logger.LogInformation("[EmployeeDocumentExpiryJob] Email sent for document '{DocumentName}' (EmpId: {EmpId})",
                        doc.DocumentName, doc.EmployeeId);
                }
                catch (Exception ex)
                {
                    _logger.LogError(ex, "[EmployeeDocumentExpiryJob] Error sending email for document '{DocumentName}'", doc.DocumentName);
                }
            }

            _logger.LogInformation("[EmployeeDocumentExpiryJob] Completed at {Time}. Total processed: {Count}",
                DateTime.Now, expiringDocs.Count());
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "[EmployeeDocumentExpiryJob] Failed to execute job.");
        }
    }
}
