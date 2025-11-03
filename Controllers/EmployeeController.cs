using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Serilog;
using TMCC.Models;
using TMCC.Services;
using TMCC.Services.IServices;

namespace TMCC.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class EmployeeController : ControllerBase
    {
        private readonly IEmployeeService _employeeService;

        public EmployeeController(IEmployeeService employeeService)
        {
            _employeeService = employeeService;
        }

        // Helper method to log the token used in the request
        private void LogToken()
        {
            var authHeader = Request.Headers["Authorization"].FirstOrDefault();
            if (!string.IsNullOrEmpty(authHeader))
            {
                Serilog.Log.Information("Authorization header received: {AuthHeader}", authHeader);
            }
            else
            {
                Serilog.Log.Warning("No Authorization header received.");
            }
        }

        [Authorize]
        [HttpGet("GetEmployees")]
        public async Task<IActionResult> GetEmployees()
        {
            LogToken();
            try
            {
                var employees = await _employeeService.GetEmployeesAsync();
                return Ok(employees);
            }
            catch (Exception ex)
            {
                Serilog.Log.Error(ex, "Error retrieving employees");
                return StatusCode(500, new { error = "Error retrieving employees.", details = ex.Message });
            }
        }

        [Authorize]
        [HttpGet("GetEmployeeById/{id}")]
        public async Task<IActionResult> GetEmployeeById(Guid id)
        {
            LogToken();
            try
            {
                var employee = await _employeeService.GetEmployeeByIdAsync(id);
                if (employee == null)
                {
                    Serilog.Log.Warning("Employee not found with Id: {EmployeeId}", id);
                    return NotFound(new { error = $"Employee with Id {id} not found." });
                }
                return Ok(employee);
            }
            catch (Exception ex)
            {
                Serilog.Log.Error(ex, "Error retrieving employee with Id: {EmployeeId}", id);
                return StatusCode(500, new { error = "Error retrieving employee.", details = ex.Message });
            }
        }

        [Authorize]
        [HttpPost("AddEmployee")]
        public async Task<IActionResult> AddEmployee([FromBody] Employee employee)
        {
            LogToken();
            try
            {
                var result = await _employeeService.AddEmployeeAsync(employee);
                return result > 0
                    ? Ok(new { message = "Employee added successfully." })
                    : BadRequest(new { error = "Failed to add employee." });
            }
            catch (Exception ex)
            {
                Serilog.Log.Error(ex, "Error adding employee");
                return StatusCode(500, new { error = "Error adding employee.", details = ex.Message });
            }
        }

        [Authorize]
        [HttpPut("UpdateEmployee")]
        public async Task<IActionResult> UpdateEmployee([FromBody] Employee employee)
        {
            LogToken();
            try
            {
                var result = await _employeeService.UpdateEmployeeAsync(employee);
                return result > 0
                    ? Ok(new { message = "Employee updated successfully." })
                    : BadRequest(new { error = "Failed to update employee." });
            }
            catch (Exception ex)
            {
                Serilog.Log.Error(ex, "Error updating employee");
                return StatusCode(500, new { error = "Error updating employee.", details = ex.Message });
            }
        }

        [Authorize]
        [HttpDelete("{empId}")]
        public async Task<IActionResult> DeleteEmployee(Guid empId, [FromQuery] string deletedBy)
        {
            LogToken();
            try
            {
                if (string.IsNullOrWhiteSpace(deletedBy))
                    return BadRequest(new { error = "DeletedBy is required." });

                var result = await _employeeService.DeleteEmployeeAsync(empId, deletedBy);

                if (result >= 0)
                {
                    return Ok(new { message = "Employee deleted successfully" });
                }

                return NotFound(new { error = "Employee not found" });
            }
            catch (Exception ex)
            {
                Serilog.Log.Error(ex, "Error deleting employee with Id: {EmployeeId}", empId);
                return StatusCode(500, new { error = "Error deleting employee.", details = ex.Message });
            }
        }

        // Similarly, add LogToken() in all other document endpoints...
        // Example:
        [Authorize]
        [HttpPost("upload/{empId}")]
        public async Task<IActionResult> UploadDocuments(
            Guid empId,
            [FromForm] IFormFileCollection files,
            [FromForm] string[] documentNames,
            [FromForm] string[] expiryDates,
            [FromForm] string uploadedBy)
        {
            LogToken();
            if (files == null || files.Count == 0)
                return BadRequest(new { error = "No files uploaded." });

            if (documentNames == null || documentNames.Length != files.Count)
                return BadRequest(new { error = "Document names must match number of files." });

            if (string.IsNullOrWhiteSpace(uploadedBy))
                return BadRequest(new { error = "UploadedBy is required." });

            try
            {
                for (int i = 0; i < files.Count; i++)
                {
                    var file = files[i];
                    var documentName = documentNames[i];
                    var expiryDate = string.IsNullOrWhiteSpace(expiryDates[i])
                        ? (DateTime?)null
                        : DateTime.Parse(expiryDates[i]);

                    byte[] fileContent;
                    using (var memoryStream = new MemoryStream())
                    {
                        await file.CopyToAsync(memoryStream);
                        fileContent = memoryStream.ToArray();
                    }

                    var document = new EmployeeDocument
                    {
                        EmpId = empId,
                        DocumentName = documentName,
                        FileName = file.FileName,
                        FileExtension = Path.GetExtension(file.FileName),
                        FileContent = fileContent,
                        FileSize = file.Length,
                        ExpiryDate = expiryDate,
                        UploadedBy = uploadedBy
                    };

                    await _employeeService.AddDocumentAsync(document);
                }

                return Ok(new { message = $"{files.Count} document(s) uploaded successfully." });
            }
            catch (Exception ex)
            {
                Serilog.Log.Error(ex, "Error uploading documents for EmployeeId: {EmployeeId}", empId);
                return StatusCode(500, new { error = "Error uploading documents.", details = ex.Message });
            }
        }
        [Authorize]
        [HttpGet("employee/{empId}/documents")]
        public async Task<IActionResult> GetDocuments(Guid empId)
        {
            LogToken();
            try
            {
                var documents = await _employeeService.GetDocumentsByEmployeeAsync(empId);
                return Ok(documents);
            }
            catch (Exception ex)
            {
                Serilog.Log.Error(ex, "Error retrieving documents for EmployeeId: {EmployeeId}", empId);
                return StatusCode(500, new { error = "Error retrieving documents.", details = ex.Message });
            }
        }

        [Authorize]
        [HttpGet("download/{documentId}")]
        public async Task<IActionResult> DownloadDocument(Guid documentId)
        {
            LogToken();
            try
            {
                var document = await _employeeService.GetDocumentByIdAsync(documentId);

                if (document == null || document.FileContent == null)
                    return NotFound(new { error = "Document not found." });

                return File(document.FileContent, "application/octet-stream", document.FileName);
            }
            catch (Exception ex)
            {
                Serilog.Log.Error(ex, "Error downloading document with Id: {DocumentId}", documentId);
                return StatusCode(500, new { error = "Error downloading document.", details = ex.Message });
            }
        }

        [Authorize]
        [HttpDelete("document/{documentId}")]
        public async Task<IActionResult> DeleteDocument(Guid documentId, [FromQuery] string deletedBy)
        {
            LogToken();
            try
            {
                if (string.IsNullOrWhiteSpace(deletedBy))
                    return BadRequest(new { error = "DeletedBy is required." });

                var result = await _employeeService.DeleteDocumentAsync(documentId, deletedBy);
                return result > 0
                    ? Ok(new { message = "Document deleted successfully" })
                    : NotFound(new { error = "Document not found" });
            }
            catch (Exception ex)
            {
                Serilog.Log.Error(ex, "Error deleting document with Id: {DocumentId}", documentId);
                return StatusCode(500, new { error = "Error deleting document.", details = ex.Message });
            }
        }

        [Authorize]
        [HttpGet("documents/expiring")]
        public async Task<IActionResult> GetDocumentsExpiring([FromQuery] DateTime expiryDate)
        {
            LogToken();
            try
            {
                var documents = await _employeeService.GetDocumentsExpiringBeforeDateAsync(expiryDate);
                return Ok(documents);
            }
            catch (Exception ex)
            {
                Serilog.Log.Error(ex, "Error retrieving expiring documents before: {ExpiryDate}", expiryDate);
                return StatusCode(500, new { error = "Error retrieving expiring documents.", details = ex.Message });
            }
        }

        [Authorize]
        [HttpGet("documents/byNameAndExpiry")]
        public async Task<IActionResult> GetDocumentsByNameAndExpiry(
            [FromQuery] string documentName,
            [FromQuery] DateTime expiryDate)
        {
            LogToken();
            try
            {
                if (string.IsNullOrWhiteSpace(documentName))
                    return BadRequest(new { error = "Document name is required." });

                var documents = await _employeeService.GetDocumentsByNameAndExpiryAsync(documentName, expiryDate);
                return Ok(documents);
            }
            catch (Exception ex)
            {
                Serilog.Log.Error(ex, "Error retrieving documents by name: {DocumentName} and expiry: {ExpiryDate}", documentName, expiryDate);
                return StatusCode(500, new { error = "Error retrieving documents by name and expiry.", details = ex.Message });
            }
        }
        [Authorize]
        [HttpPut("{empId}/documents/{documentId}/renew")]
        public async Task<IActionResult> RenewEmptDocumentExpiry(Guid empId, Guid documentId, [FromBody] RenewClientDocumentExpiryDto model)
        {
            try
            {
                if (model == null || string.IsNullOrWhiteSpace(model.NewExpiryDate) || string.IsNullOrWhiteSpace(model.UpdatedBy))
                    return BadRequest(new { error = "NewExpiryDate and UpdatedBy are required." });

                var result = await _employeeService.RenewEmpDocumentExpiryAsync(documentId, empId, model.NewExpiryDate, model.UpdatedBy);

                return result > 0
                    ? Ok(new { message = "Employee document expiry renewed successfully." })
                    : NotFound(new { error = "Document not found or could not be updated." });
            }
            catch (Exception ex)
            {
                Serilog.Log.Error(ex, "Error renewing Employee document expiry for DocumentId: {DocumentId}", documentId);
                return StatusCode(500, new { error = "Error renewing Employee document expiry.", details = ex.Message });
            }
        }
    }
}