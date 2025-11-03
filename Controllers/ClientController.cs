using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using TMCC.Models;
using TMCC.Services.IServices;

namespace TMCC.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class ClientController : ControllerBase
    {
        private readonly IClientService _clientService;

        public ClientController(IClientService clientService)
        {
            _clientService = clientService;
        }

        // ============= CLIENT ENDPOINTS =============

        [Authorize]
        [HttpGet("GetClients")]
        public async Task<IActionResult> GetClients()
        {
            try
            {
                var clients = await _clientService.GetClientsAsync();
                return Ok(clients);
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { error = "Error retrieving clients.", details = ex.Message });
            }
        }

        [Authorize]
        [HttpGet("GetClientById/{id}")]
        public async Task<IActionResult> GetClientById(Guid id)
        {
            try
            {
                var client = await _clientService.GetClientByIdAsync(id);
                if (client == null)
                    return NotFound(new { error = $"Client with Id {id} not found." });
                return Ok(client);
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { error = "Error retrieving client.", details = ex.Message });
            }
        }

        [Authorize]
        [HttpPost("AddClient")]
        public async Task<IActionResult> AddClient([FromBody] Client client)
        {
            try
            {
                var result = await _clientService.AddClientAsync(client);
                if (result != null)
                {
                    return Ok(result);
                }
                else
                {
                    return BadRequest(new { error = "Failed to add client." });
                }
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { error = "Error adding client.", details = ex.Message });
            }
        }

        [Authorize]
        [HttpPut("UpdateClient")]
        public async Task<IActionResult> UpdateClient([FromBody] Client client)
        {
            try
            {
                // Validate required fields only
                if (string.IsNullOrWhiteSpace(client.ClientName) ||
                    string.IsNullOrWhiteSpace(client.Email) ||
                    string.IsNullOrWhiteSpace(client.Mobile) ||
                    string.IsNullOrWhiteSpace(client.Address))
                {
                    return BadRequest(new { error = "Required fields are missing." });
                }

                var result = await _clientService.UpdateClientAsync(client);
                return result != null
                    ? Ok(result)
                    : BadRequest(new { error = "Failed to update client." });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { error = "Error updating client.", details = ex.Message });
            }
        }

        [Authorize]
        [HttpDelete("{clientId}")]
        public async Task<IActionResult> DeleteClient(Guid clientId, [FromQuery] string deletedBy)
        {
            try
            {
                if (string.IsNullOrWhiteSpace(deletedBy))
                    return BadRequest(new { error = "DeletedBy is required." });

                var result = await _clientService.DeleteClientAsync(clientId, deletedBy);

                if (result >= 0)
                {
                    return Ok(new { message = "Client deleted successfully" });
                }

                return NotFound(new { error = "Client not found" });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { error = "Error deleting client.", details = ex.Message });
            }
        }

        // ============= CONCERN PERSON ENDPOINTS =============

        [Authorize]
        [HttpGet("{clientId}/concerns")]
        public async Task<IActionResult> GetConcernPersons(Guid clientId)
        {
            try
            {
                var concerns = await _clientService.GetConcernPersonsAsync(clientId);
                return Ok(concerns);
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { error = "Error retrieving concern persons.", details = ex.Message });
            }
        }

        [Authorize]
        [HttpPost("{clientId}/concerns")]
        public async Task<IActionResult> AddConcernPerson(Guid clientId, [FromBody] ConcernPerson concernPerson)
        {
            try
            {
                // Only validate required fields
                if (string.IsNullOrWhiteSpace(concernPerson.Name) ||
                    string.IsNullOrWhiteSpace(concernPerson.Email) ||
                    string.IsNullOrWhiteSpace(concernPerson.Mobile))
                {
                    return BadRequest(new { error = "Name, Email, and Mobile are required." });
                }

                concernPerson.ClientId = clientId;
                concernPerson.CreatedBy = concernPerson.CreatedBy ?? "System";

                var result = await _clientService.AddConcernPersonAsync(concernPerson);
                return result > 0
                    ? Ok(new { message = "Concern person added successfully." })
                    : BadRequest(new { error = "Failed to add concern person." });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { error = "Error adding concern person.", details = ex.Message });
            }
        }

        [Authorize]
        [HttpDelete("concern/{concernId}")]
        public async Task<IActionResult> DeleteConcernPerson(Guid concernId, [FromQuery] string deletedBy)
        {
            try
            {
                if (string.IsNullOrWhiteSpace(deletedBy))
                    return BadRequest(new { error = "DeletedBy is required." });

                var result = await _clientService.DeleteConcernPersonAsync(concernId, deletedBy);
                return result > 0
                    ? Ok(new { message = "Concern person deleted successfully" })
                    : NotFound(new { error = "Concern person not found" });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { error = "Error deleting concern person.", details = ex.Message });
            }
        }

        // ============= DOCUMENT ENDPOINTS =============

        [Authorize]
        [HttpPost("upload/{clientId}")]
        public async Task<IActionResult> UploadDocuments(
            Guid clientId,
            [FromForm] IFormFileCollection files,
            [FromForm] string[] documentNames,
            [FromForm] string[] expiryDates,
            [FromForm] string uploadedBy)
        {
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

                    var document = new ClientDocument
                    {
                        ClientId = clientId,
                        DocumentName = documentName,
                        FileName = file.FileName,
                        FileExtension = Path.GetExtension(file.FileName),
                        FileContent = fileContent,
                        FileSize = file.Length,
                        ExpiryDate = expiryDate,
                        UploadedBy = uploadedBy
                    };

                    await _clientService.AddDocumentAsync(document);
                }

                return Ok(new { message = $"{files.Count} document(s) uploaded successfully." });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { error = "Error uploading documents.", details = ex.Message });
            }
        }

        [Authorize]
        [HttpGet("client/{clientId}/documents")]
        public async Task<IActionResult> GetDocuments(Guid clientId)
        {
            try
            {
                var documents = await _clientService.GetDocumentsByClientAsync(clientId);
                return Ok(documents);
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { error = "Error retrieving documents.", details = ex.Message });
            }
        }

        [Authorize]
        [HttpGet("download/{documentId}")]
        public async Task<IActionResult> DownloadDocument(Guid documentId)
        {
            try
            {
                var document = await _clientService.GetDocumentByIdAsync(documentId);

                if (document == null || document.FileContent == null)
                    return NotFound(new { error = "Document not found." });

                return File(document.FileContent, "application/octet-stream", document.FileName);
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { error = "Error downloading document.", details = ex.Message });
            }
        }

        [Authorize]
        [HttpDelete("document/{documentId}")]
        public async Task<IActionResult> DeleteDocument(Guid documentId, [FromQuery] string deletedBy)
        {
            try
            {
                if (string.IsNullOrWhiteSpace(deletedBy))
                    return BadRequest(new { error = "DeletedBy is required." });

                var result = await _clientService.DeleteDocumentAsync(documentId, deletedBy);
                return result > 0
                    ? Ok(new { message = "Document deleted successfully" })
                    : NotFound(new { error = "Document not found" });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { error = "Error deleting document.", details = ex.Message });
            }
        }

        [Authorize]
        [HttpGet("documents/expiring")]
        public async Task<IActionResult> GetDocumentsExpiring([FromQuery] int days)
        {
            if (days <= 0)
                return BadRequest(new { error = "Days must be greater than zero." });

            try
            {
                var documents = await _clientService.GetDocumentsExpiringInDaysAsync(days);
                return Ok(documents);
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { error = "Error retrieving expiring documents.", details = ex.Message });
            }
        }

        [Authorize]
        [HttpGet("documents/expiring-by-name")]
        public async Task<IActionResult> GetDocumentsExpiringByName([FromQuery] string documentName, [FromQuery] int days)
        {
            if (string.IsNullOrWhiteSpace(documentName))
                return BadRequest(new { error = "DocumentName is required." });

            if (days <= 0)
                return BadRequest(new { error = "Days must be greater than zero." });

            try
            {
                var documents = await _clientService.GetDocumentsByNameAndExpiryAsync(documentName, days);
                return Ok(documents);
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { error = "Error retrieving expiring documents by name.", details = ex.Message });
            }
        }
        [Authorize]
        [HttpPut("{clientId}/documents/{documentId}/renew")]
        public async Task<IActionResult> RenewClientDocumentExpiry(Guid clientId, Guid documentId, [FromBody] RenewClientDocumentExpiryDto model)
        {
            try
            {
                if (model == null || string.IsNullOrWhiteSpace(model.NewExpiryDate) || string.IsNullOrWhiteSpace(model.UpdatedBy))
                    return BadRequest(new { error = "NewExpiryDate and UpdatedBy are required." });

                var result = await _clientService.RenewClientDocumentExpiryAsync(documentId, clientId, model.NewExpiryDate, model.UpdatedBy);

                return result > 0
                    ? Ok(new { message = "Client document expiry renewed successfully." })
                    : NotFound(new { error = "Document not found or could not be updated." });
            }
            catch (Exception ex)
            {
                Serilog.Log.Error(ex, "Error renewing client document expiry for DocumentId: {DocumentId}", documentId);
                return StatusCode(500, new { error = "Error renewing client document expiry.", details = ex.Message });
            }
        }


    }
}