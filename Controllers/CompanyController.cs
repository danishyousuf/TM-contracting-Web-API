using Dapper;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Serilog;
using TMCC.Models.DTO;
using TMCC.Services.IServices;

namespace TMCC.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class CompanyController : ControllerBase
    {
        private readonly ICompanyService _service;

        public CompanyController(ICompanyService service)
        {
            _service = service;
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
        [HttpGet("details")]
        public async Task<IActionResult> GetCompanyDetails()
        {
            LogToken();
            try
            {
                var result = await _service.GetCompanyDetails();
                return Ok(result);
            }
            catch (Exception ex)
            {
                Serilog.Log.Error(ex, "Error retrieving company details");
                return StatusCode(500, new { error = "Error retrieving company details.", details = ex.Message });
            }
        }

        [Authorize]
        [HttpPut("update")]
        public async Task<IActionResult> UpdateCompany([FromBody] CompanyUpdateDto model)
        {
            LogToken();
            try
            {
                var parameters = new DynamicParameters();
                parameters.Add("p_company_id", model.company_id);
                parameters.Add("p_company_name", model.company_name);
                parameters.Add("p_address", model.address);
                parameters.Add("p_mobile_phone", model.mobile_phone);
                parameters.Add("p_landline_phone", model.landline_phone);
                parameters.Add("p_primary_email", model.primary_email);
                parameters.Add("p_secondary_email", model.secondary_email);
                parameters.Add("p_updated_by", model.updated_by);

                var result = await _service.UpdateCompanyDetails(parameters);
                return Ok(result);
            }
            catch (Exception ex)
            {
                Serilog.Log.Error(ex, "Error updating company");
                return StatusCode(500, new { error = "Error updating company.", details = ex.Message });
            }
        }
        
        [Authorize]
        [HttpPost("document/upload")]
        public async Task<IActionResult> UploadDocument([FromForm] DocumentUploadDto model)
        {
            LogToken();
            try
            {
                if (model.doc_content == null || model.doc_content.Length == 0)
                    return BadRequest(new { error = "No file uploaded" });

                byte[] fileBytes;
                using (var memoryStream = new MemoryStream())
                {
                    await model.doc_content.CopyToAsync(memoryStream);
                    fileBytes = memoryStream.ToArray();
                }

                var parameters = new DynamicParameters();
                parameters.Add("p_doc_id", model.doc_id);
                parameters.Add("p_company_id", model.company_id);
                parameters.Add("p_doc_name", model.doc_name);
                parameters.Add("p_doc_content", fileBytes);
                parameters.Add("p_doc_expiry", model.doc_expiry);
                parameters.Add("p_uploaded_by", model.uploaded_by);

                var result = await _service.UploadCompanyDocument(parameters);
                return Ok(result);
            }
            catch (Exception ex)
            {
                Serilog.Log.Error(ex, "Error uploading company document");
                return StatusCode(500, new { error = "Error uploading document.", details = ex.Message });
            }
        }

        [Authorize]
        [HttpDelete("document/delete")]
        public async Task<IActionResult> DeleteDocument([FromBody] DocumentDeleteDto model)
        {
            LogToken();
            try
            {
                var parameters = new DynamicParameters();
                parameters.Add("p_doc_id", model.doc_id);
                parameters.Add("p_deleted_by", model.deleted_by);

                var result = await _service.DeleteCompanyDocument(parameters);
                return Ok(result);
            }
            catch (Exception ex)
            {
                Serilog.Log.Error(ex, "Error deleting company document");
                return StatusCode(500, new { error = "Error deleting document.", details = ex.Message });
            }
        }

        [Authorize]
        [HttpGet("documents/{companyId}")]
        public async Task<IActionResult> GetDocuments(string companyId)
        {
            LogToken();
            try
            {
                var result = await _service.GetCompanyDocuments(companyId);
                return Ok(result);
            }
            catch (Exception ex)
            {
                Serilog.Log.Error(ex, "Error retrieving company documents for CompanyId: {CompanyId}", companyId);
                return StatusCode(500, new { error = "Error retrieving documents.", details = ex.Message });
            }
        }

        [Authorize]
        [HttpGet("documents/expiring/{days}")]
        public async Task<IActionResult> GetCompanyDocumentsExpiringInDays(int days)
        {
            LogToken();
            try
            {
                var result = await _service.GetCompanyDocumentsExpiringInDays(days);
                return Ok(result);
            }
            catch (Exception ex)
            {
                Serilog.Log.Error(ex, "Error retrieving expiring company documents for Days: {Days}", days);
                return StatusCode(500, new { error = "Error retrieving expiring documents.", details = ex.Message });
            }
        }
        [Authorize]
        [HttpPut("documents/{documentId}/renew")]
        public async Task<IActionResult> RenewDocumentExpiry(Guid documentId, [FromBody] RenewDocumentExpiryDto model)
        {
            LogToken();
            try
            {
                var parameters = new DynamicParameters();
                parameters.Add("p_doc_id", documentId);
                parameters.Add("p_new_expiry_date", model.newExpiryDate);
                parameters.Add("p_updated_by", model.updated_by);

                var result = await _service.RenewDocumentExpiry(parameters);
                return Ok(new { message = "Document expiry updated successfully", result });
            }
            catch (Exception ex)
            {
                Serilog.Log.Error(ex, "Error renewing document expiry for DocumentId: {DocumentId}", documentId);
                return StatusCode(500, new { error = "Error renewing document expiry.", details = ex.Message });
            }
        }

    }
}
