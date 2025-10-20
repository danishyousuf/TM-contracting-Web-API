using TMCC.Models.DTO;

namespace TMCC.Services.IServices
{
    public interface IEmployeeHistoryAndStatusService
    {
        Task<IEnumerable<EmployeeDto>> GetAvailableEmployeesAsync();
        Task<IEnumerable<EmployeeOccupiedDto>> GetOccupiedEmployeesAsync();
        Task MarkEmployeeFreeAsync(string empId);
        Task MarkEmployeeBusyAsync(string empId, string clientId);
        Task<IEnumerable<EmployeeHistoryDto>> GetEmployeeHistoryAsync(string empId);
        Task<IEnumerable<ClientEmployeeDTO>> GetEmployeesByClientAsync(string clientId);
    }
}
