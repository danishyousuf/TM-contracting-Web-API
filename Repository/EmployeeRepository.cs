using Dapper;
using System.Data;
using TMCC.Db_Helper;
using TMCC.Models;
using TMCC.Repository.IRepository;

namespace TMCC.Repository
{
    public class EmployeeRepository : IEmployeeRepository
    {
        private readonly DapperHelper _dapperHelper;

        public EmployeeRepository(DapperHelper dapperHelper)
        {
            _dapperHelper = dapperHelper;
        }

        //  Get All Employees
        public async Task<IEnumerable<Employee>> GetEmployeesAsync()
        {
            return await _dapperHelper.QueryAsync<Employee>(
                "sp_GetEmployees",
                null,
                CommandType.StoredProcedure
            );
        }

        //  Get Employee By Id
        public async Task<Employee> GetEmployeeByIdAsync(Guid empId)
        {
            var parameters = new DynamicParameters();
            parameters.Add("p_emp_id", empId);

            return await _dapperHelper.QueryFirstOrDefaultAsync<Employee>(
                "sp_GetEmployeeById",
                parameters,
                CommandType.StoredProcedure
            );
        }

        //  Add Employee
        public async Task<int> AddEmployeeAsync(Employee employee)
        {
            var parameters = new DynamicParameters();
            parameters.Add("p_emp_code", employee.EmpCode);
            parameters.Add("p_full_name", employee.FullName);
            parameters.Add("p_iqama_no", employee.IqamaNo);
            parameters.Add("p_profession", employee.Profession);
            parameters.Add("p_nationality", employee.Nationality);
            parameters.Add("p_date_of_arrival", employee.DateOfArrival);
            parameters.Add("p_date_of_birth", employee.DateOfBirth);
            parameters.Add("p_passport_no", employee.PassportNo);
            parameters.Add("p_passport_expiry", employee.PassportExpiry);
            parameters.Add("p_iqama_expiry", employee.IqamaExpiry);
            parameters.Add("p_account_number", employee.AccountNumber);
            parameters.Add("p_bank_name", employee.BankName);
            parameters.Add("p_mobile", employee.Mobile);
            parameters.Add("p_email", employee.Email);
            parameters.Add("p_sponsor", employee.Sponsor);
            parameters.Add("p_status", employee.Status);
            parameters.Add("p_remarks", employee.Remarks);
            parameters.Add("p_created_by", employee.CreatedBy);

            return await _dapperHelper.ExecuteNonQueryAsync("sp_AddEmployee", parameters);
        }

        //  Update Employee
        public async Task<int> UpdateEmployeeAsync(Employee employee)
        {
            var parameters = new DynamicParameters();
            parameters.Add("p_emp_id", employee.EmpId);
            parameters.Add("p_emp_code", employee.EmpCode);
            parameters.Add("p_full_name", employee.FullName);
            parameters.Add("p_iqama_no", employee.IqamaNo);
            parameters.Add("p_profession", employee.Profession);
            parameters.Add("p_nationality", employee.Nationality);
            parameters.Add("p_date_of_arrival", employee.DateOfArrival);
            parameters.Add("p_date_of_birth", employee.DateOfBirth);
            parameters.Add("p_passport_no", employee.PassportNo);
            parameters.Add("p_passport_expiry", employee.PassportExpiry);
            parameters.Add("p_iqama_expiry", employee.IqamaExpiry);
            parameters.Add("p_account_number", employee.AccountNumber);
            parameters.Add("p_bank_name", employee.BankName);
            parameters.Add("p_mobile", employee.Mobile);
            parameters.Add("p_email", employee.Email);
            parameters.Add("p_sponsor", employee.Sponsor);
            parameters.Add("p_status", employee.Status);
            parameters.Add("p_remarks", employee.Remarks);
            parameters.Add("p_updated_by", employee.UpdatedBy);

            return await _dapperHelper.ExecuteNonQueryAsync("sp_UpdateEmployee", parameters);
        }

        // Delete employee
        public async Task<int> DeleteEmployeeAsync(Guid empId, string deletedBy)
        {
            var parameters = new DynamicParameters();
            parameters.Add("p_emp_id", empId);
            parameters.Add("p_deleted_by", deletedBy);

            return await _dapperHelper.ExecuteNonQueryAsync(
                "sp_DeleteEmployee",
                parameters
            );
        }

        // ============= DOCUMENT METHODS =============

        // Add document
        public async Task<int> AddDocumentAsync(EmployeeDocument document)
        {
            var parameters = new DynamicParameters();
            parameters.Add("p_emp_id", document.EmpId.ToString());
            parameters.Add("p_document_name", document.DocumentName);
            parameters.Add("p_file_name", document.FileName);
            parameters.Add("p_file_extension", document.FileExtension);
            parameters.Add("p_file_content", document.FileContent);
            parameters.Add("p_file_size", document.FileSize);
            parameters.Add("p_expiry_date", document.ExpiryDate);
            parameters.Add("p_uploaded_by", document.UploadedBy);

            return await _dapperHelper.ExecuteNonQueryAsync(
                "sp_AddEmployeeDocument",
                parameters
            );
        }

        // Get all documents by employee id (without file content)
        public async Task<IEnumerable<EmployeeDocument>> GetDocumentsByEmployeeAsync(Guid empId)
        {
            var parameters = new DynamicParameters();
            parameters.Add("p_emp_id", empId.ToString());

            return await _dapperHelper.QueryAsync<EmployeeDocument>(
                "sp_GetEmployeeDocuments",
                parameters,
                CommandType.StoredProcedure 
            );
        }

        // Get single document by id (with file content for download)
        public async Task<EmployeeDocument> GetDocumentByIdAsync(Guid documentId)
        {
            var parameters = new DynamicParameters();
            parameters.Add("p_document_id", documentId.ToString());

            return await _dapperHelper.QueryFirstOrDefaultAsync<EmployeeDocument>(
                "sp_GetDocumentById",
                parameters,
                CommandType.StoredProcedure  
            );
        }

        // Delete document
        public async Task<int> DeleteDocumentAsync(Guid documentId, string deletedBy)
        {
            var parameters = new DynamicParameters();
            parameters.Add("p_document_id", documentId.ToString());
            parameters.Add("p_deleted_by", deletedBy);

            return await _dapperHelper.ExecuteNonQueryAsync(
                "sp_DeleteEmployeeDocument",
                parameters
            );
        }
        // Get documents expiring before or on a given date
        public async Task<IEnumerable<EmployeeDocumentExpiry>> GetDocumentsExpiringBeforeDateAsync(DateTime expiryDate)
        {
            var parameters = new DynamicParameters();
            parameters.Add("p_expiry_date", expiryDate);

            return await _dapperHelper.QueryAsync<EmployeeDocumentExpiry>(
                "sp_GetDocumentsExpiringBeforeDate",
                parameters,
                CommandType.StoredProcedure
            );
        }

        // Get documents by name and expiry
        public async Task<IEnumerable<EmployeeDocumentExpiry>> GetDocumentsByNameAndExpiryAsync(string documentName, DateTime expiryDate)
        {
            var parameters = new DynamicParameters();
            parameters.Add("p_document_name", documentName);
            parameters.Add("p_expiry_date", expiryDate);

            return await _dapperHelper.QueryAsync<EmployeeDocumentExpiry>(
                "sp_GetDocumentsByNameAndExpiry",
                parameters,
                CommandType.StoredProcedure
            );
        }
        
             public async Task<int> RenewEmpDocumentExpiryAsync(Guid documentId, Guid empId, string newExpiryDate, string updatedBy)
        {
            var parameters = new DynamicParameters();
            parameters.Add("p_document_id", documentId.ToString());
            parameters.Add("p_emp_id", empId.ToString());
            parameters.Add("p_new_expiry_date", newExpiryDate);
            parameters.Add("p_updated_by", updatedBy);

            return await _dapperHelper.ExecuteNonQueryAsync(
                "sp_RenewEmployeeDocumentExpiry",
                parameters
            );
        }
    }
}