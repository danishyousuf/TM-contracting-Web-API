namespace TMCC.Models.DTO
{
    public class DocumentUploadDto
    {
        public string doc_id { get; set; }
        public string company_id { get; set; }
        public string doc_name { get; set; }
        public IFormFile doc_content { get; set; } 
        public string doc_expiry { get; set; }
        public string uploaded_by { get; set; }
    }
}
