using TMCC.Models.DTO;

namespace TMCC.Services.IServices
{
    public interface IEmployeePaymentHistoryService
    {
        Task<IEnumerable<EmployeePaymentHistoryDto>> GetEmployeePaymentsAsync(Guid empId);
        Task<int> AddEmployeePaymentAsync(EmployeePaymentHistoryDto payment);
        Task<int> UpdateEmployeePaymentAsync(EmployeePaymentHistoryDto payment);
        Task<int> DeleteEmployeePaymentAsync(Guid paymentId);
        Task<IEnumerable<EmployeePaymentHistoryDto>> GetLatestEmployeePaymentsAsync();

    }
}
