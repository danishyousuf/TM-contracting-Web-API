using Dapper;
using TMCC.Db_Helper;
using TMCC.Models.DTO;
using TMCC.Repository.IRepository;

namespace TMCC.Repository
{
    public class CompanyRepository : ICompanyRepository
    {
        private readonly DapperHelper _db;

        public CompanyRepository(DapperHelper db)
        {
            _db = db;
        }

        // Get single company
        public async Task<dynamic> GetCompanyDetails()
        {
            return await _db.QueryFirstOrDefaultSPAsync<dynamic>("sp_GetCompanyDetails", new DynamicParameters());
        }

        // Update company
        public async Task<int> UpdateCompanyDetails(DynamicParameters parameters)
        {
            return await _db.ExecuteNonQueryAsync("sp_UpdateCompanyDetails", parameters);
        }

        // Upload document
        public async Task<int> UploadCompanyDocument(DynamicParameters parameters)
        {
            return await _db.ExecuteNonQueryAsync("sp_UploadCompanyDocument", parameters);
        }

        // Delete document
        public async Task<int> DeleteCompanyDocument(DynamicParameters parameters)
        {
            return await _db.ExecuteNonQueryAsync("sp_DeleteCompanyDocument", parameters);
        }

        // Get company documents
        public async Task<IEnumerable<dynamic>> GetCompanyDocuments(string companyId)
        {
            var param = new DynamicParameters();
            param.Add("p_company_id", companyId);
            return await _db.QueryAsync<dynamic>("sp_GetCompanyDocuments", param, commandType: System.Data.CommandType.StoredProcedure);
        }
        public async Task<IEnumerable<CompanyDocumentExpiryDto>> GetCompanyDocumentsExpiringInDays(int days)
        {
            var param = new DynamicParameters();
            param.Add("p_days", days);
            return await _db.QueryAsync<CompanyDocumentExpiryDto>(
                "sp_GetCompanyDocumentsExpiringInDays",
                param,
                commandType: System.Data.CommandType.StoredProcedure
            );
        }

    }
}
