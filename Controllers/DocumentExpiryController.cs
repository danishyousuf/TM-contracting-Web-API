using Microsoft.AspNetCore.Mvc;
using Quartz;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace TMCC.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class DocumentExpiryController : ControllerBase
    {
        private readonly ISchedulerFactory _schedulerFactory;

        public DocumentExpiryController(ISchedulerFactory schedulerFactory)
        {
            _schedulerFactory = schedulerFactory;
        }

        /// <summary>
        /// Trigger Client Document Expiry Job
        /// </summary>
        [HttpPost("trigger-client")]
        public async Task<IActionResult> TriggerClientJob()
        {
            return await TriggerJob("ClientDocumentExpiryJob", "Client");
        }

        /// <summary>
        /// Trigger Employee Document Expiry Job
        /// </summary>
        [HttpPost("trigger-employee")]
        public async Task<IActionResult> TriggerEmployeeJob()
        {
            return await TriggerJob("EmployeeDocumentExpiryJob", "Employee");
        }

        /// <summary>
        /// Trigger Company Document Expiry Job
        /// </summary>
        [HttpPost("trigger-company")]
        public async Task<IActionResult> TriggerCompanyJob()
        {
            return await TriggerJob("CompanyDocumentExpiryJob", "Company");
        }

        /// <summary>
        /// Helper method to trigger a job by JobKey
        /// </summary>
        private async Task<IActionResult> TriggerJob(string jobKeyName, string jobType)
        {
            try
            {
                var scheduler = await _schedulerFactory.GetScheduler();
                var jobKey = new JobKey(jobKeyName);

                await scheduler.TriggerJob(jobKey);

                return Ok(new
                {
                    message = $"{jobType} document expiry job triggered successfully",
                    timestamp = DateTime.Now
                });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new
                {
                    message = $"Error triggering {jobType} job",
                    error = ex.Message
                });
            }
        }

        /// <summary>
        /// Get next run time for a specific job
        /// </summary>
        [HttpGet("next-run/{jobType}")]
        public async Task<IActionResult> GetNextRunTime(string jobType)
        {
            try
            {
                var scheduler = await _schedulerFactory.GetScheduler();

                JobKey jobKey = jobType.ToLower() switch
                {
                    "client" => new JobKey("ClientDocumentExpiryJob"),
                    "employee" => new JobKey("EmployeeDocumentExpiryJob"),
                    "company" => new JobKey("CompanyDocumentExpiryJob"),
                    _ => throw new ArgumentException("Invalid job type. Use 'client', 'employee', or 'company'.")
                };

                var triggers = await scheduler.GetTriggersOfJob(jobKey);

                var nextRunTimes = triggers.Select(t => t.GetNextFireTimeUtc()?.LocalDateTime).ToList();

                return Ok(new
                {
                    nextRunTimes,
                    currentTime = DateTime.Now
                });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new
                {
                    message = "Error getting next run time",
                    error = ex.Message
                });
            }
        }
    }
}
