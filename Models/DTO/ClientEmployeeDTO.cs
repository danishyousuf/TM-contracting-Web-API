namespace TMCC.Models.DTO
{
    public class ClientEmployeeDTO
    {
        public Guid EmpId { get; set; }
        public string FullName { get; set; }
        public string Mobile { get; set; }
        public string Email { get; set; }
        public string ClientName { get; set; }
        public DateTime StartDate { get; set; }
        public DateTime? EndDate { get; set; }
    }
}
