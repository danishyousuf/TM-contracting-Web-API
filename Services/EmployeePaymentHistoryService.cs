using TMCC.Models.DTO;
using TMCC.Repository;
using TMCC.Repository.IRepository;
using TMCC.Services.IServices;

namespace TMCC.Services
{
    public class EmployeePaymentHistoryService : IEmployeePaymentHistoryService
    {
        private readonly IEmployeePaymentHistoryRepository _repository;

        public EmployeePaymentHistoryService(IEmployeePaymentHistoryRepository repository)
        {
            _repository = repository;
        }

        public Task<IEnumerable<EmployeePaymentHistoryDto>> GetEmployeePaymentsAsync(Guid empId)
            => _repository.GetEmployeePaymentsAsync(empId);

        public Task<int> AddEmployeePaymentAsync(EmployeePaymentHistoryDto payment)
            => _repository.AddEmployeePaymentAsync(payment);

        public Task<int> UpdateEmployeePaymentAsync(EmployeePaymentHistoryDto payment)
            => _repository.UpdateEmployeePaymentAsync(payment);

        public Task<int> DeleteEmployeePaymentAsync(Guid paymentId)
            => _repository.DeleteEmployeePaymentAsync(paymentId);
        public async Task<IEnumerable<EmployeePaymentHistoryDto>> GetLatestEmployeePaymentsAsync()
        {
            return await _repository.GetLatestEmployeePaymentsAsync();
        }

    }
}
