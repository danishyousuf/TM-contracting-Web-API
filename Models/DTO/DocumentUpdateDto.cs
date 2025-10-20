namespace TMCC.Models.DTO
{
    public class DocumentUpdateDto
    {
        public string doc_id { get; set; }
        public string company_id { get; set; }
        public string doc_name { get; set; }
        public IFormFile doc_content { get; set; }  
        public string doc_expiry { get; set; }
        public string updated_by { get; set; }
    }
}
