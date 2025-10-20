using Dapper;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
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
        
        [Authorize]
        [HttpGet("details")]
        public async Task<IActionResult> GetCompanyDetails()
        {
            var result = await _service.GetCompanyDetails();
            return Ok(result);
        }

        [Authorize]
        [HttpPut("update")]
        public async Task<IActionResult> UpdateCompany([FromBody] CompanyUpdateDto model)
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
        
        [Authorize]
        [HttpPost("document/upload")]
        public async Task<IActionResult> UploadDocument([FromForm] DocumentUploadDto model)
        {
            if (model.doc_content == null || model.doc_content.Length == 0)
                return BadRequest("No file uploaded");

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

        [Authorize]
        [HttpDelete("document/delete")]
        public async Task<IActionResult> DeleteDocument([FromBody] DocumentDeleteDto model)
        {
            var parameters = new DynamicParameters();
            parameters.Add("p_doc_id", model.doc_id);
            parameters.Add("p_deleted_by", model.deleted_by);

            var result = await _service.DeleteCompanyDocument(parameters);
            return Ok(result);
        }

        [Authorize]
        [HttpGet("documents/{companyId}")]
        public async Task<IActionResult> GetDocuments(string companyId)
        {
            var result = await _service.GetCompanyDocuments(companyId);
            return Ok(result);
        }

        [Authorize]
        [HttpGet("documents/expiring/{days}")]
        public async Task<IActionResult> GetCompanyDocumentsExpiringInDays(int days)
        {
            var result = await _service.GetCompanyDocumentsExpiringInDays(days);
            return Ok(result);
        }

    }
}
