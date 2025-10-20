namespace TMCC.Models.DTO
{
    public class ClientDocumentExpiryDTO
    {
            public Guid DocumentId { get; set; }
            public Guid ClientId { get; set; }
            public string DocumentName { get; set; }
            public string ClientName { get; set; }
            public DateTime? ExpiryDate { get; set; }
            public string? UploadedBy { get; set; }
            public DateTime? UploadedDate { get; set; }
         
        
    }
}
