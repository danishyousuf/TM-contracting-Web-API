namespace TMCC.Models
{
    public class Client
    {
        public Guid ClientId { get; set; }
        public string ClientName { get; set; }
        public string Email { get; set; }
        public string Mobile { get; set; }
        public string Address { get; set; }
        public string Status { get; set; }
        public string? Remarks { get; set; }  
        public string? CreatedBy { get; set; }  
        public DateTime? CreatedOnDate { get; set; }
        public string? UpdatedBy { get; set; }  
        public DateTime? UpdatedOnDate { get; set; }
        public bool IsDeleted { get; set; }
        public DateTime? DeletedOnDate { get; set; }
        public string? DeletedBy { get; set; } 
    }
}