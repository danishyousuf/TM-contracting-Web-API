using TMCC.Models;

namespace TMCC.Repository.IRepository
{
    public interface IClientPaymentHistoryRepository
    {
        Task<int> AddClientPaymentAsync(ClientPaymentHistory payment);
        Task<IEnumerable<ClientPaymentHistory>> GetLatestPaymentsAsync();
        Task<IEnumerable<ClientPaymentHistory>> GetClientPaymentHistoryAsync(string clientId);
        Task<int> DeleteClientPaymentAsync(string paymentId, string deletedBy);
    }
}
