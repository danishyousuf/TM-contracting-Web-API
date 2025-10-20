using System.ComponentModel.DataAnnotations.Schema;

namespace TMCC.Models.DTO
{
    public class EmployeeDto
    {
        public Guid EmpId { get; set; }        // if emp_id is INT
        public string EmpCode { get; set; }
        public string FullName { get; set; }
        public string Profession { get; set; }
        public string Nationality { get; set; }
        public string Mobile { get; set; }
        public string Email { get; set; }
        public bool IsBusy { get; set; }      // if is_busy is TINYINT(1)
    }


}