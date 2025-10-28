using Dapper;
using TMCC.Db_Helper;
using TMCC.Models.DTO;
using TMCC.Repository.IRepository;
using System.Data;

namespace TMCC.Repository
{
    public class EmployeeHistoryAndStatusRepository : IEmployeeHistoryAndStatusRepository
    {
        private readonly DapperHelper _dapper;

        public EmployeeHistoryAndStatusRepository(DapperHelper dapper)
        {
            _dapper = dapper;
        }

        public async Task<IEnumerable<EmployeeDto>> GetAvailableEmployeesAsync()
        {
            return await _dapper.QueryAsync<EmployeeDto>(
                "sp_GetAvailableEmployees", commandType: CommandType.StoredProcedure);
        }

        public async Task<IEnumerable<EmployeeOccupiedDto>> GetOccupiedEmployeesAsync()
        {
            return await _dapper.QueryAsync<EmployeeOccupiedDto>(
                "sp_GetOccupiedEmployees", commandType: CommandType.StoredProcedure);
        }

        public async Task MarkEmployeeFreeAsync(string empId, DateTime lastWorkingDate)
        {
            var parameters = new DynamicParameters();
            parameters.Add("p_emp_id", empId);
            parameters.Add("p_last_working_date", lastWorkingDate);

            await _dapper.ExecuteNonQueryAsync("sp_MarkEmployeeFree", parameters);
        }


        public async Task MarkEmployeeBusyAsync(string empId, string clientId, DateTime startDate)
        {
            var parameters = new DynamicParameters();
            parameters.Add("p_emp_id", empId);
            parameters.Add("p_client_id", clientId);
            parameters.Add("p_start_date", startDate);
            parameters.Add("p_end_date", null);

            await _dapper.ExecuteNonQueryAsync("sp_MarkEmployeeBusy", parameters);
        }


        public async Task<IEnumerable<EmployeeHistoryDto>> GetEmployeeHistoryAsync(string empId)
        {
            var parameters = new DynamicParameters();
            parameters.Add("p_emp_id", empId);
            return await _dapper.QueryAsync<EmployeeHistoryDto>(
                "sp_GetEmployeeHistory", parameters, CommandType.StoredProcedure);
        }

        public async Task<IEnumerable<ClientEmployeeDTO>> GetEmployeesByClientAsync(string clientId)
        {
            var parameters = new DynamicParameters();
            parameters.Add("p_client_id", clientId);
            return await _dapper.QueryAsync<ClientEmployeeDTO>(
                "sp_GetEmployeesByClient", parameters, CommandType.StoredProcedure);
        }
    }
}
