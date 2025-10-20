namespace TMCC.Models
{
    public class EmployeeDocument
    {
        public Guid DocumentId { get; set; }
        public Guid EmpId { get; set; }
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