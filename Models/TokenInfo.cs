namespace TMCC.Models
{
    public class TokenInfo
    {
        public string Issuer { get; set; }
        public string Audience { get; set; }
        public DateTime ValidFrom { get; set; }
        public DateTime ValidTo { get; set; }
        public bool IsExpired { get; set; }
    }

}
