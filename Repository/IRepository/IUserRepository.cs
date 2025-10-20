using TMCC.Models.DTO;
using System.Threading.Tasks;

namespace TMCC.Repository.IRepository
{
    public interface IUserRepository
    {
        Task<int> RegisterUserAsync(RegisterUserDto user);
        Task<UserResponseDto> LoginAsync(string email, string passwordHash);
        Task<int> UpdateUserAsync(UpdateUserDto user);
        Task<UserResponseDto?> GetUserByEmailAsync(string email);
    }


}
