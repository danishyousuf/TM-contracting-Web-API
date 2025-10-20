namespace TMCC.Models.DTO
{
    public class CompanyUpdateDto
    {
        public string company_id { get; set; }
        public string company_name { get; set; }
        public string address { get; set; }
        public string mobile_phone { get; set; }
        public string landline_phone { get; set; }
        public string primary_email { get; set; }
        public string secondary_email { get; set; }
        public string updated_by { get; set; }
    }
}
