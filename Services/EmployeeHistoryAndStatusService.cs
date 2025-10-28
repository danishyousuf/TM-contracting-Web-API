using TMCC.Models.DTO;
using TMCC.Repository.IRepository;
using TMCC.Services.IServices;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace TMCC.Services
{
    public class EmployeeHistoryAndStatusService : IEmployeeHistoryAndStatusService
    {
        private readonly IEmployeeHistoryAndStatusRepository _repo;

        public EmployeeHistoryAndStatusService(IEmployeeHistoryAndStatusRepository repo)
        {
            _repo = repo;
        }

        public Task<IEnumerable<EmployeeDto>> GetAvailableEmployeesAsync()
            => _repo.GetAvailableEmployeesAsync();

        public Task<IEnumerable<EmployeeOccupiedDto>> GetOccupiedEmployeesAsync()
            => _repo.GetOccupiedEmployeesAsync();

        public Task MarkEmployeeFreeAsync(string empId, DateTime lastWorkingDate)
           => _repo.MarkEmployeeFreeAsync(empId, lastWorkingDate);


        public Task MarkEmployeeBusyAsync(string empId, string clientId, DateTime startDate)
           => _repo.MarkEmployeeBusyAsync(empId, clientId, startDate);



        public Task<IEnumerable<EmployeeHistoryDto>> GetEmployeeHistoryAsync(string empId)
            => _repo.GetEmployeeHistoryAsync(empId);

        public Task<IEnumerable<ClientEmployeeDTO>> GetEmployeesByClientAsync(string clientId)
            => _repo.GetEmployeesByClientAsync(clientId);
    }
}
