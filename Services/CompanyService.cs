using Dapper;
using TMCC.Models.DTO;
using TMCC.Repository.IRepository;
using TMCC.Services.IServices;

namespace TMCC.Services
{
    public class CompanyService : ICompanyService
    {
        private readonly ICompanyRepository _repo;

        public CompanyService(ICompanyRepository repo)
        {
            _repo = repo;
        }

        public async Task<dynamic> GetCompanyDetails()
        {
            return await _repo.GetCompanyDetails();
        }

        public async Task<int> UpdateCompanyDetails(DynamicParameters parameters)
        {
            return await _repo.UpdateCompanyDetails(parameters);
        }

        public async Task<int> UploadCompanyDocument(DynamicParameters parameters)
        {
            return await _repo.UploadCompanyDocument(parameters);
        }

        public async Task<int> DeleteCompanyDocument(DynamicParameters parameters)
        {
            return await _repo.DeleteCompanyDocument(parameters);
        }

        public async Task<IEnumerable<dynamic>> GetCompanyDocuments(string companyId)
        {
            return await _repo.GetCompanyDocuments(companyId);
        }
        public async Task<IEnumerable<CompanyDocumentExpiryDto>> GetCompanyDocumentsExpiringInDays(int days)
        {
            return await _repo.GetCompanyDocumentsExpiringInDays(days);
        }
        public async Task<int> RenewDocumentExpiry(DynamicParameters parameters)
        {
            return await _repo.RenewDocumentExpiry(parameters);
        }

    }
}
