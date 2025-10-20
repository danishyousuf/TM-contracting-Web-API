using TMCC.Db_Helper;
using TMCC.Models.DTO;
using TMCC.Repository.IRepository;
using TMCC.Services.IServices;

namespace TMCC.Services
{
    public class UserService : IUserService
    {
        private readonly IUserRepository _userRepository;

        public UserService(IUserRepository userRepository)
        {
            _userRepository = userRepository;
        }

        public async Task RegisterAsync(RegisterUserDto user)
        {
            // Hash password
            user.Password = PasswordHelper.HashPassword(user.Password);

            await _userRepository.RegisterUserAsync(user);
        }

        public async Task<UserResponseDto> LoginAsync(LoginUserDto login)
        {
            var passwordHash = PasswordHelper.HashPassword(login.Password);
            var user = await _userRepository.LoginAsync(login.Email, passwordHash);

            if (user == null)
                throw new Exception("Invalid email or password.");

            return user;
        }
        public async Task UpdateUserAsync(UpdateUserDto user)
        {
            await _userRepository.UpdateUserAsync(user);
        }
        public async Task<UserResponseDto?> GetUserByEmailAsync(string email)
        {
            return await _userRepository.GetUserByEmailAsync(email);
        }

    }
}
