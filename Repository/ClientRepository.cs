using Dapper;
using System.Data;
using TMCC.Db_Helper;
using TMCC.Models;
using TMCC.Models.DTO;
using TMCC.Repository.IRepository;

namespace TMCC.Repository
{
    public class ClientRepository : IClientRepository
    {
        private readonly DapperHelper _dapperHelper;

        public ClientRepository(DapperHelper dapperHelper)
        {
            _dapperHelper = dapperHelper;
        }

        // ============= CLIENT METHODS =============

        public async Task<IEnumerable<Client>> GetClientsAsync()
        {
            return await _dapperHelper.QueryAsync<Client>(
                "sp_GetClients",
                null,
                CommandType.StoredProcedure
            );
        }

        public async Task<Client> GetClientByIdAsync(Guid clientId)
        {
            var parameters = new DynamicParameters();
            parameters.Add("p_ClientId", clientId.ToString());

            return await _dapperHelper.QueryFirstOrDefaultAsync<Client>(
                "sp_GetClientById",
                parameters,
                CommandType.StoredProcedure
            );
        }

        public async Task<Client> AddClientAsync(Client client)
        {
            var parameters = new DynamicParameters();
            parameters.Add("p_client_name", client.ClientName);
            parameters.Add("p_email", client.Email);
            parameters.Add("p_mobile", client.Mobile);
            parameters.Add("p_address", client.Address);
            parameters.Add("p_status", client.Status);
            parameters.Add("p_remarks", client.Remarks);
            parameters.Add("p_created_by", client.CreatedBy);

            return await _dapperHelper.QueryFirstOrDefaultAsync<Client>(
                "sp_AddClient",
                parameters,
                CommandType.StoredProcedure
            );
        }

        public async Task<Client> UpdateClientAsync(Client client)
        {
            var parameters = new DynamicParameters();
            parameters.Add("p_client_id", client.ClientId.ToString());
            parameters.Add("p_client_name", client.ClientName);
            parameters.Add("p_email", client.Email);
            parameters.Add("p_mobile", client.Mobile);
            parameters.Add("p_address", client.Address);
            parameters.Add("p_status", client.Status);
            parameters.Add("p_remarks", client.Remarks);
            parameters.Add("p_updated_by", client.UpdatedBy);

            return await _dapperHelper.QueryFirstOrDefaultAsync<Client>(
                "sp_UpdateClient",
                parameters,
                CommandType.StoredProcedure
            );
        }

        public async Task<int> DeleteClientAsync(Guid clientId, string deletedBy)
        {
            var parameters = new DynamicParameters();
            parameters.Add("p_client_id", clientId.ToString());
            parameters.Add("p_deleted_by", deletedBy);

            return await _dapperHelper.ExecuteNonQueryAsync(
                "sp_DeleteClient",
                parameters
            );
        }

        // ============= CONCERN PERSON METHODS =============

        public async Task<IEnumerable<ConcernPerson>> GetConcernPersonsAsync(Guid clientId)
        {
            var parameters = new DynamicParameters();
            parameters.Add("p_client_id", clientId.ToString());

            return await _dapperHelper.QueryAsync<ConcernPerson>(
                "sp_GetConcernPersons",
                parameters,
                CommandType.StoredProcedure
            );
        }

        public async Task<int> AddConcernPersonAsync(ConcernPerson concernPerson)
        {
            var parameters = new DynamicParameters();
            parameters.Add("p_client_id", concernPerson.ClientId.ToString());
            parameters.Add("p_name", concernPerson.Name);
            parameters.Add("p_email", concernPerson.Email);
            parameters.Add("p_mobile", concernPerson.Mobile);
            parameters.Add("p_created_by", concernPerson.CreatedBy);

            return await _dapperHelper.ExecuteNonQueryAsync(
                "sp_AddConcernPerson",
                parameters
            );
        }

        public async Task<int> DeleteConcernPersonAsync(Guid concernId, string deletedBy)
        {
            var parameters = new DynamicParameters();
            parameters.Add("p_concern_id", concernId.ToString());
            parameters.Add("p_deleted_by", deletedBy);

            return await _dapperHelper.ExecuteNonQueryAsync(
                "sp_DeleteConcernPerson",
                parameters
            );
        }

        // ============= DOCUMENT METHODS =============

        public async Task<int> AddDocumentAsync(ClientDocument document)
        {
            var parameters = new DynamicParameters();
            parameters.Add("p_client_id", document.ClientId.ToString());
            parameters.Add("p_document_name", document.DocumentName);
            parameters.Add("p_file_name", document.FileName);
            parameters.Add("p_file_extension", document.FileExtension);
            parameters.Add("p_file_content", document.FileContent);
            parameters.Add("p_file_size", document.FileSize);
            parameters.Add("p_expiry_date", document.ExpiryDate);
            parameters.Add("p_uploaded_by", document.UploadedBy);

            return await _dapperHelper.ExecuteNonQueryAsync(
                "sp_AddClientDocument",
                parameters
            );
        }

        public async Task<IEnumerable<ClientDocument>> GetDocumentsByClientAsync(Guid clientId)
        {
            var parameters = new DynamicParameters();
            parameters.Add("p_client_id", clientId.ToString());

            return await _dapperHelper.QueryAsync<ClientDocument>(
                "sp_GetClientDocuments",
                parameters,
                CommandType.StoredProcedure
            );
        }

        public async Task<ClientDocument> GetDocumentByIdAsync(Guid documentId)
        {
            var parameters = new DynamicParameters();
            parameters.Add("p_document_id", documentId.ToString());

            return await _dapperHelper.QueryFirstOrDefaultAsync<ClientDocument>(
                "sp_GetClientDocumentById",
                parameters,
                CommandType.StoredProcedure
            );
        }

        public async Task<int> DeleteDocumentAsync(Guid documentId, string deletedBy)
        {
            var parameters = new DynamicParameters();
            parameters.Add("p_document_id", documentId.ToString());
            parameters.Add("p_deleted_by", deletedBy);

            return await _dapperHelper.ExecuteNonQueryAsync(
                "sp_DeleteClientDocument",
                parameters
            );
        }
        public async Task<IEnumerable<ClientDocumentExpiryDTO>> GetDocumentsExpiringInDaysAsync(int days)
        {
            var parameters = new DynamicParameters();
            parameters.Add("p_days", days);

            return await _dapperHelper.QueryAsync<ClientDocumentExpiryDTO>(
                "sp_GetDocumentsExpiringInDays",
                parameters,
                CommandType.StoredProcedure
            );
        }

        public async Task<IEnumerable<ClientDocumentExpiryDTO>> GetDocumentsByNameAndExpiryAsync(string documentName, int days)
        {
            var parameters = new DynamicParameters();
            parameters.Add("p_docName", documentName);
            parameters.Add("p_days", days);

            return await _dapperHelper.QueryAsync<ClientDocumentExpiryDTO>(
                "sp_GetDocumentsByNameAndExpiry",
                parameters,
                CommandType.StoredProcedure
            );
        }
        public async Task<int> RenewClientDocumentExpiryAsync(Guid documentId, Guid clientId, string newExpiryDate, string updatedBy)
        {
            var parameters = new DynamicParameters();
            parameters.Add("p_document_id", documentId.ToString());
            parameters.Add("p_client_id", clientId.ToString());
            parameters.Add("p_new_expiry_date", newExpiryDate);
            parameters.Add("p_updated_by", updatedBy);

            return await _dapperHelper.ExecuteNonQueryAsync(
                "sp_RenewClientDocumentExpiry",
                parameters
            );
        }


    }
}