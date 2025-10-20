namespace TMCC.Models.DTO
{
    public class EmployeeHistoryDto
    {
        public Guid EmpId { get; set; }
        public string FullName { get; set; }
        public Guid ClientId { get; set; }
        public string ClientName { get; set; }
        public DateTime StartDate { get; set; }
        public DateTime? EndDate { get; set; }
    }
}
