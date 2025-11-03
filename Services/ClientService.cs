using TMCC.Models;
using TMCC.Models.DTO;
using TMCC.Repository.IRepository;
using TMCC.Services.IServices;

namespace TMCC.Services
{
    public class ClientService : IClientService
    {
        private readonly IClientRepository _clientRepository;

        public ClientService(IClientRepository clientRepository)
        {
            _clientRepository = clientRepository;
        }

        // ============= CLIENT METHODS =============

        public async Task<IEnumerable<Client>> GetClientsAsync()
            => await _clientRepository.GetClientsAsync();

        public async Task<Client> GetClientByIdAsync(Guid clientId)
            => await _clientRepository.GetClientByIdAsync(clientId);

        public async Task<Client> AddClientAsync(Client client)
            => await _clientRepository.AddClientAsync(client);

        public async Task<Client> UpdateClientAsync(Client client)
            => await _clientRepository.UpdateClientAsync(client);

        public async Task<int> DeleteClientAsync(Guid clientId, string deletedBy)
            => await _clientRepository.DeleteClientAsync(clientId, deletedBy);

        // ============= CONCERN PERSON METHODS =============

        public async Task<IEnumerable<ConcernPerson>> GetConcernPersonsAsync(Guid clientId)
            => await _clientRepository.GetConcernPersonsAsync(clientId);

        public async Task<int> AddConcernPersonAsync(ConcernPerson concernPerson)
            => await _clientRepository.AddConcernPersonAsync(concernPerson);

        public async Task<int> DeleteConcernPersonAsync(Guid concernId, string deletedBy)
            => await _clientRepository.DeleteConcernPersonAsync(concernId, deletedBy);

        // ============= DOCUMENT METHODS =============

        public async Task<int> AddDocumentAsync(ClientDocument document)
            => await _clientRepository.AddDocumentAsync(document);

        public async Task<IEnumerable<ClientDocument>> GetDocumentsByClientAsync(Guid clientId)
            => await _clientRepository.GetDocumentsByClientAsync(clientId);

        public async Task<ClientDocument> GetDocumentByIdAsync(Guid documentId)
            => await _clientRepository.GetDocumentByIdAsync(documentId);

        public async Task<int> DeleteDocumentAsync(Guid documentId, string deletedBy)
            => await _clientRepository.DeleteDocumentAsync(documentId, deletedBy);
        public async Task<IEnumerable<ClientDocumentExpiryDTO>> GetDocumentsExpiringInDaysAsync(int days)
    => await _clientRepository.GetDocumentsExpiringInDaysAsync(days);

        public async Task<IEnumerable<ClientDocumentExpiryDTO>> GetDocumentsByNameAndExpiryAsync(string documentName, int days)
            => await _clientRepository.GetDocumentsByNameAndExpiryAsync(documentName, days);

        public async Task<int> RenewClientDocumentExpiryAsync(Guid documentId, Guid clientId, string newExpiryDate, string updatedBy)
    => await _clientRepository.RenewClientDocumentExpiryAsync(documentId, clientId, newExpiryDate, updatedBy);

    }
}