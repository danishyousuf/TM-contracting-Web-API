namespace TMCC.Models
{
    public class EmployeeDocumentExpiry
    {
        public Guid DocumentId { get; set; }
        public Guid EmployeeId { get; set; }
        public string EmployeeName { get; set; }
        public string DocumentName { get; set; }
        public string FileName { get; set; }  // Original file name
        public string FileExtension { get; set; }  // .pdf, .jpg, etc.
        public byte[] FileContent { get; set; }  // Binary file data
        public long FileSize { get; set; }  // Size in bytes
        public DateTime? ExpiryDate { get; set; }
        public string UploadedBy { get; set; }
        public DateTime? UploadedDate { get; set; }
        public string DeletedBy { get; set; }
        public DateTime? DeletedOnDate { get; set; }
        public bool IsDeleted { get; set; }
    }
}
