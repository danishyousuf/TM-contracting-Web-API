using TMCC.Models;
using TMCC.Repository;
using TMCC.Repository.IRepository;
using TMCC.Services.IServices;

namespace TMCC.Services
{
    public class EmployeeService : IEmployeeService
    {
        private readonly IEmployeeRepository _employeeRepository;

        public EmployeeService(IEmployeeRepository employeeRepository)
        {
            _employeeRepository = employeeRepository;
        }

        // ============= EMPLOYEE METHODS =============

        public async Task<IEnumerable<Employee>> GetEmployeesAsync()
            => await _employeeRepository.GetEmployeesAsync();

        public async Task<Employee> GetEmployeeByIdAsync(Guid empId)
            => await _employeeRepository.GetEmployeeByIdAsync(empId);

        public async Task<int> AddEmployeeAsync(Employee employee)
            => await _employeeRepository.AddEmployeeAsync(employee);

        public async Task<int> UpdateEmployeeAsync(Employee employee)
            => await _employeeRepository.UpdateEmployeeAsync(employee);

        public async Task<int> DeleteEmployeeAsync(Guid empId, string deletedBy)
            => await _employeeRepository.DeleteEmployeeAsync(empId, deletedBy);

        // ============= DOCUMENT METHODS =============

        public async Task<int> AddDocumentAsync(EmployeeDocument document)
            => await _employeeRepository.AddDocumentAsync(document);

        public async Task<IEnumerable<EmployeeDocument>> GetDocumentsByEmployeeAsync(Guid empId)
            => await _employeeRepository.GetDocumentsByEmployeeAsync(empId);

        public async Task<EmployeeDocument> GetDocumentByIdAsync(Guid documentId)
            => await _employeeRepository.GetDocumentByIdAsync(documentId);

        public async Task<int> DeleteDocumentAsync(Guid documentId, string deletedBy)
            => await _employeeRepository.DeleteDocumentAsync(documentId, deletedBy);
        public async Task<IEnumerable<EmployeeDocumentExpiry>> GetDocumentsExpiringBeforeDateAsync(DateTime expiryDate)
    => await _employeeRepository.GetDocumentsExpiringBeforeDateAsync(expiryDate);

        public async Task<IEnumerable<EmployeeDocumentExpiry>> GetDocumentsByNameAndExpiryAsync(string documentName, DateTime expiryDate)
            => await _employeeRepository.GetDocumentsByNameAndExpiryAsync(documentName, expiryDate);

        public async Task<int> RenewEmpDocumentExpiryAsync(Guid documentId, Guid empId, string newExpiryDate, string updatedBy)
  => await _employeeRepository.RenewEmpDocumentExpiryAsync(documentId, empId, newExpiryDate, updatedBy);
    }
}