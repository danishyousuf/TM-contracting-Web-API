using Microsoft.Extensions.Configuration;
using Microsoft.IdentityModel.Tokens;
using Serilog;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using TMCC.Services.IServices;

namespace TMCC.Services
{
    public class JwtService : IJwtService
    {
        private readonly IConfiguration _configuration;

        public JwtService(IConfiguration configuration)
        {
            _configuration = configuration;
        }

        public (string tokenString, object tokenInfo) GenerateToken(string userId, string email, string firstName, string lastName)
        {
            try
            {
                var jwtSettings = _configuration.GetSection("Jwt");
                var key = Encoding.UTF8.GetBytes(jwtSettings["Key"]);
                var issuer = jwtSettings["Issuer"];
                var audience = jwtSettings["Audience"];
                var expiryHours = Convert.ToDouble(jwtSettings["ExpiryInHours"] ?? "24");

                Serilog.Log.Information("Generating JWT token for UserId: {UserId}, Email: {Email}", userId, email);

                var claims = new List<Claim>
                {
                    new Claim(JwtRegisteredClaimNames.Sub, userId),
                    new Claim(JwtRegisteredClaimNames.Email, email),
                    new Claim("given_name", firstName),
                    new Claim("family_name", lastName),
                    new Claim(ClaimTypes.NameIdentifier, userId),
                    new Claim(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString())
                };

                var creds = new SigningCredentials(new SymmetricSecurityKey(key), SecurityAlgorithms.HmacSha256);

                var token = new JwtSecurityToken(
                    issuer: issuer,
                    audience: audience,
                    claims: claims,
                    expires: DateTime.UtcNow.AddHours(expiryHours),
                    signingCredentials: creds
                );

                var tokenString = new JwtSecurityTokenHandler().WriteToken(token);

                var tokenInfo = new
                {
                    Issuer = issuer,
                    Audience = audience,
                    ExpiresAt = token.ValidTo,
                    IssuedAt = token.ValidFrom
                };

                Serilog.Log.Information("Token generated successfully for UserId: {UserId}, ExpiresAt: {Expiry}", userId, token.ValidTo);

                return (tokenString, tokenInfo);
            }
            catch (Exception ex)
            {
                Serilog.Log.Error(ex, "Error generating JWT token for UserId: {UserId}", userId);
                throw;
            }
        }
    }
}
