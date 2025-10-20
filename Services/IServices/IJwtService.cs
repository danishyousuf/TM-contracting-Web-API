namespace TMCC.Services.IServices
{
    public interface IJwtService
    {
        (string tokenString, object tokenInfo) GenerateToken(string userId, string email, string firstName, string lastName);
    }
}
