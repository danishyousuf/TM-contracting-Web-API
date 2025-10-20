using TMCC.Models;
using TMCC.Models.DTO;

namespace TMCC.Services.IServices
{
    public interface IClientService
    {
        // Client methods
        Task<IEnumerable<Client>> GetClientsAsync();
        Task<Client> GetClientByIdAsync(Guid clientId);
        Task<Client> AddClientAsync(Client client);
        Task<Client> UpdateClientAsync(Client client);
        Task<int> DeleteClientAsync(Guid clientId, string deletedBy);

        // Concern Person methods
        Task<IEnumerable<ConcernPerson>> GetConcernPersonsAsync(Guid clientId);
        Task<int> AddConcernPersonAsync(ConcernPerson concernPerson);
        Task<int> DeleteConcernPersonAsync(Guid concernId, string deletedBy);

        // Document methods
        Task<int> AddDocumentAsync(ClientDocument document);
        Task<IEnumerable<ClientDocument>> GetDocumentsByClientAsync(Guid clientId);
        Task<ClientDocument> GetDocumentByIdAsync(Guid documentId);
        Task<int> DeleteDocumentAsync(Guid documentId, string deletedBy);
        // Expiring Document methods
        Task<IEnumerable<ClientDocumentExpiryDTO>> GetDocumentsExpiringInDaysAsync(int days);
        Task<IEnumerable<ClientDocumentExpiryDTO>> GetDocumentsByNameAndExpiryAsync(string documentName, int days);

    }
}