namespace TMCC.Models.DTO
{
    public class UserResponseDto
    {
        public Guid UserId { get; set; }
        public string FirstName { get; set; }   
        public string LastName { get; set; }   
        public string Email { get; set; }
        public string Phone { get; set; }
        public string Gender { get; set; }      
        public DateTime Dob { get; set; }      
        public string Status { get; set; }
    }
}
