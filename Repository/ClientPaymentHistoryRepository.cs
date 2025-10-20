using Dapper;
using TMCC.Db_Helper;
using TMCC.Models;
using TMCC.Repository.IRepository;

namespace TMCC.Repository
{
    public class ClientPaymentHistoryRepository : IClientPaymentHistoryRepository
    {
        private readonly DapperHelper _dapper;

        public ClientPaymentHistoryRepository(DapperHelper dapper)
        {
            _dapper = dapper;
        }

        public async Task<int> AddClientPaymentAsync(ClientPaymentHistory payment)
        {
            var parameters = new DynamicParameters();
            parameters.Add("p_client_id", payment.ClientId);
            parameters.Add("p_client_name", payment.ClientName);
            parameters.Add("p_amount", payment.Amount);
            parameters.Add("p_payment_mode", payment.PaymentMode);
            parameters.Add("p_remarks", payment.Remarks);
            parameters.Add("p_created_by", payment.CreatedBy);

            return await _dapper.ExecuteNonQueryAsync("sp_AddClientPayment", parameters);
        }

        public async Task<IEnumerable<ClientPaymentHistory>> GetLatestPaymentsAsync()
        {
            var result = await _dapper.QueryAsync<ClientPaymentHistory>(
                "sp_GetLatestClientPayments",
                commandType: System.Data.CommandType.StoredProcedure
            );
            return result;
        }
        public async Task<IEnumerable<ClientPaymentHistory>> GetClientPaymentHistoryAsync(string clientId)
        {
            var parameters = new DynamicParameters();
            parameters.Add("p_client_id", clientId);

            var result = await _dapper.QueryAsync<ClientPaymentHistory>(
                "sp_GetClientPaymentHistory",
                parameters,
                commandType: System.Data.CommandType.StoredProcedure
            );

            return result;
        }


        public async Task<int> DeleteClientPaymentAsync(string paymentId, string deletedBy)
        {
            var parameters = new DynamicParameters();
            parameters.Add("p_payment_id", paymentId);
            parameters.Add("p_deleted_by", deletedBy);

            return await _dapper.ExecuteNonQueryAsync("sp_DeleteClientPayment", parameters);
        }
    }
}
