using TMCC.Models.DTO;

namespace TMCC.Repository.IRepository
{
    public interface IEmployeeHistoryAndStatusRepository
    {
        Task<IEnumerable<EmployeeDto>> GetAvailableEmployeesAsync();
        Task<IEnumerable<EmployeeOccupiedDto>> GetOccupiedEmployeesAsync();
        Task MarkEmployeeFreeAsync(string empId, DateTime lastWorkingDate);

        Task MarkEmployeeBusyAsync(string empId, string clientId, DateTime startDate);


        Task<IEnumerable<EmployeeHistoryDto>> GetEmployeeHistoryAsync(string empId);
        Task<IEnumerable<ClientEmployeeDTO>> GetEmployeesByClientAsync(string clientId);
    }
}
