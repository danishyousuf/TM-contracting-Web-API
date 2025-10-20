namespace TMCC.Models.DTO
{
    public class EmployeePaymentHistoryDto
    {
        public Guid PaymentId { get; set; }
        public Guid EmpId { get; set; }
        public string FullName { get; set; }     
        public decimal Amount { get; set; }
        public DateTime PaymentDate { get; set; }
        public string PaymentMode { get; set; }  
        public string Remarks { get; set; }
        public string CreatedBy { get; set; }
        public DateTime CreatedAt { get; set; }
    }
}
