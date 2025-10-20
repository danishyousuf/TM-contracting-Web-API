namespace TMCC.Models
{
    public class ConcernPerson
    {
        public Guid ConcernId { get; set; }
        public Guid ClientId { get; set; }
        public string Name { get; set; }
        public string Email { get; set; }
        public string Mobile { get; set; }
        public string? CreatedBy { get; set; }  
        public DateTime? CreatedOnDate { get; set; }
        public bool IsDeleted { get; set; }
        public DateTime? DeletedOnDate { get; set; }
        public string? DeletedBy { get; set; }  
    }
}
