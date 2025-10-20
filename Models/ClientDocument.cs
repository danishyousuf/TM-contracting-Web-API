namespace TMCC.Models
{
    public class ClientDocument
    {
        public Guid DocumentId { get; set; }
        public Guid ClientId { get; set; }
        public string DocumentName { get; set; }
        public string FileName { get; set; }
        public string? FileExtension { get; set; }  
        public byte[] FileContent { get; set; }
        public long FileSize { get; set; }
        public DateTime? ExpiryDate { get; set; }
        public string? UploadedBy { get; set; }  
        public DateTime? UploadedDate { get; set; }
        public string? DeletedBy { get; set; }  
        public DateTime? DeletedOnDate { get; set; }
        public bool IsDeleted { get; set; }
    }
}
