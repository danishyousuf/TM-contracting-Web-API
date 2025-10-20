using Dapper;
using TMCC.Db_Helper;
using TMCC.Models.DTO;
using TMCC.Repository.IRepository;

namespace TMCC.Repository
{
    public class EmployeePaymentHistoryRepository : IEmployeePaymentHistoryRepository
    {
        private readonly DapperHelper _dapperHelper;

        public EmployeePaymentHistoryRepository(DapperHelper dapperHelper)
        {
            _dapperHelper = dapperHelper;
        }

        public async Task<IEnumerable<EmployeePaymentHistoryDto>> GetEmployeePaymentsAsync(Guid empId)
        {
            var parameters = new DynamicParameters();
            parameters.Add("p_emp_id", empId);

            return await _dapperHelper.QueryAsync<EmployeePaymentHistoryDto>(
                "sp_GetEmployeePaymentHistory",
                parameters,
                System.Data.CommandType.StoredProcedure
            );
        }

        public async Task<int> AddEmployeePaymentAsync(EmployeePaymentHistoryDto payment)
        {
            var parameters = new DynamicParameters();
            parameters.Add("p_emp_id", payment.EmpId);
            parameters.Add("p_amount", payment.Amount);
            parameters.Add("p_payment_date", payment.PaymentDate);
            parameters.Add("p_payment_mode", payment.PaymentMode);
            parameters.Add("p_remarks", payment.Remarks);

            return await _dapperHelper.ExecuteNonQueryAsync("sp_AddEmployeePayment", parameters);
        }

        public async Task<int> UpdateEmployeePaymentAsync(EmployeePaymentHistoryDto payment)
        {
            var parameters = new DynamicParameters();
            parameters.Add("p_payment_id", payment.PaymentId);
            parameters.Add("p_amount", payment.Amount);
            parameters.Add("p_payment_date", payment.PaymentDate);
            parameters.Add("p_payment_mode", payment.PaymentMode);
            parameters.Add("p_remarks", payment.Remarks);

            return await _dapperHelper.ExecuteNonQueryAsync("sp_UpdateEmployeePayment", parameters);
        }

        public async Task<int> DeleteEmployeePaymentAsync(Guid paymentId)
        {
            var parameters = new DynamicParameters();
            parameters.Add("p_payment_id", paymentId);

            return await _dapperHelper.ExecuteNonQueryAsync("sp_DeleteEmployeePayment", parameters);
        }
        public async Task<IEnumerable<EmployeePaymentHistoryDto>> GetLatestEmployeePaymentsAsync()
        {
            return await _dapperHelper.QueryAsync<EmployeePaymentHistoryDto>(
                "sp_GetLatestEmployeePayments",
                null,
                System.Data.CommandType.StoredProcedure
            );
        }

    }
}
