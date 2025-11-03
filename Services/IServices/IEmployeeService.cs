using TMCC.Models;

namespace TMCC.Services.IServices
{
    public interface IEmployeeService
    {
        // Employee methods
        Task<IEnumerable<Employee>> GetEmployeesAsync();
        Task<Employee> GetEmployeeByIdAsync(Guid empId);
        Task<int> AddEmployeeAsync(Employee employee);
        Task<int> UpdateEmployeeAsync(Employee employee);
        Task<int> DeleteEmployeeAsync(Guid empId, string deletedBy);

        // Document methods
        Task<int> AddDocumentAsync(EmployeeDocument document);
        Task<IEnumerable<EmployeeDocument>> GetDocumentsByEmployeeAsync(Guid empId);
        Task<EmployeeDocument> GetDocumentByIdAsync(Guid documentId);
        Task<int> DeleteDocumentAsync(Guid documentId, string deletedBy);
        Task<IEnumerable<EmployeeDocumentExpiry>> GetDocumentsExpiringBeforeDateAsync(DateTime expiryDate);
        Task<IEnumerable<EmployeeDocumentExpiry>> GetDocumentsByNameAndExpiryAsync(string documentName, DateTime expiryDate);
        Task<int> RenewEmpDocumentExpiryAsync(Guid documentId, Guid empId, string newExpiryDate, string updatedBy);


    }
}