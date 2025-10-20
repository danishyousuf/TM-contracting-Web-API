using Dapper;
using TMCC.Models.DTO;

namespace TMCC.Services.IServices
{
    public interface ICompanyService
    {
        Task<dynamic> GetCompanyDetails();
        Task<int> UpdateCompanyDetails(DynamicParameters parameters);
        Task<int> UploadCompanyDocument(DynamicParameters parameters);
        Task<int> DeleteCompanyDocument(DynamicParameters parameters);
        Task<IEnumerable<dynamic>> GetCompanyDocuments(string companyId);
        Task<IEnumerable<CompanyDocumentExpiryDto>> GetCompanyDocumentsExpiringInDays(int days);

    }
}
