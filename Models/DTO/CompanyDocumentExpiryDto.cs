namespace TMCC.Models.DTO
{
    public class CompanyDocumentExpiryDto
    {
        public Guid DocumentId { get; set; }
        public Guid CompanyId { get; set; }
        public string DocumentName { get; set; }
        public DateTime ExpiryDate { get; set; }
        public string UploadedBy { get; set; }
        public DateTime UploadedDate { get; set; }
    }
}
