namespace TMCC.Models
{
  
        public class Employee
        {
            public Guid EmpId { get; set; }  // changed from int to Guid
            public string EmpCode { get; set; }
            public string FullName { get; set; }
            public string IqamaNo { get; set; }
            public string Profession { get; set; }
            public string Nationality { get; set; }
            public DateTime? DateOfArrival { get; set; }
            public DateTime? DateOfBirth { get; set; }
            public string PassportNo { get; set; }
            public DateTime? PassportExpiry { get; set; }
            public DateTime? IqamaExpiry { get; set; }
            public string AccountNumber { get; set; }
            public string BankName { get; set; }
            public string Mobile { get; set; }
            public string Email { get; set; }
            public string Sponsor { get; set; }
            public string Status { get; set; }
            public string Remarks { get; set; }
            public string? CreatedBy { get; set; }
            public DateTime? CreatedOnDate { get; set; }
            public string? UpdatedBy { get; set; }
            public DateTime? UpdatedOnDate { get; set; }
            public bool IsDeleted { get; set; }
            public DateTime? DeletedOnDate { get; set; }
            public string? DeletedBy { get; set; }
        }
    }



