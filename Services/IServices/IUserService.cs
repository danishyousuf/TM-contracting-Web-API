using TMCC.Models.DTO;

namespace TMCC.Services.IServices
{
    public interface IUserService
    {
        Task RegisterAsync(RegisterUserDto user);
        Task<UserResponseDto> LoginAsync(LoginUserDto login);
        Task UpdateUserAsync(UpdateUserDto user);
        Task<UserResponseDto?> GetUserByEmailAsync(string email);
    }


}
