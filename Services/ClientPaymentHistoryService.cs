using TMCC.Models;
using TMCC.Repository.IRepository;
using TMCC.Services.IServices;

namespace TMCC.Services
{
    public class ClientPaymentHistoryService : IClientPaymentHistoryService
    {
        private readonly IClientPaymentHistoryRepository _repository;

        public ClientPaymentHistoryService(IClientPaymentHistoryRepository repository)
        {
            _repository = repository;
        }

        public Task<int> AddClientPaymentAsync(ClientPaymentHistory payment)
            => _repository.AddClientPaymentAsync(payment);

        public Task<IEnumerable<ClientPaymentHistory>> GetLatestPaymentsAsync()
            => _repository.GetLatestPaymentsAsync();

        public Task<IEnumerable<ClientPaymentHistory>> GetClientPaymentHistoryAsync(string clientId)
            => _repository.GetClientPaymentHistoryAsync(clientId);

        public Task<int> DeleteClientPaymentAsync(string paymentId, string deletedBy)
            => _repository.DeleteClientPaymentAsync(paymentId, deletedBy);
    }
}
