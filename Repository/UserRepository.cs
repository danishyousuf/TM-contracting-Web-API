using Dapper;
using System.Data;
using TMCC.Db_Helper;
using TMCC.Models.DTO;
using TMCC.Repository.IRepository;

namespace TMCC.Repository
{
    public class UserRepository : IUserRepository
    {
        private readonly DapperHelper _dapperHelper;

        public UserRepository(DapperHelper dapperHelper)
        {
            _dapperHelper = dapperHelper;
        }

        public async Task<int> RegisterUserAsync(RegisterUserDto user)
        {
            var parameters = new DynamicParameters();
            parameters.Add("p_first_name", user.FirstName);
            parameters.Add("p_last_name", user.LastName);
            parameters.Add("p_email", user.Email);
            parameters.Add("p_phone", user.Phone);
            parameters.Add("p_password_hash", user.Password);
            parameters.Add("p_gender", user.Gender);
            parameters.Add("p_dob", user.Dob);

            return await _dapperHelper.ExecuteNonQueryAsync("sp_RegisterUser", parameters);
        }

        public async Task<UserResponseDto> LoginAsync(string email, string passwordHash)
        {
            var parameters = new DynamicParameters();
            parameters.Add("p_email", email);
            parameters.Add("p_password_hash", passwordHash);

            var result = await _dapperHelper.QueryFirstOrDefaultAsync<dynamic>(
                "sp_UserLogin",
                parameters,
                CommandType.StoredProcedure
            );

            if (result == null)
                return null;

            var userDto = new UserResponseDto
            {
                UserId = Guid.Parse(result.UserId.ToString()),
                FirstName = result.FirstName,
                LastName = result.LastName,
                Email = result.Email,
                Phone = result.Phone,
                Gender = result.Gender,
                Dob = result.DateOfBirth,
                Status = result.Status
            };
            return userDto;
        }
        public async Task<int> UpdateUserAsync(UpdateUserDto user)
        {
            var parameters = new DynamicParameters();
            parameters.Add("p_user_id", user.UserId);
            parameters.Add("p_first_name", user.FirstName);
            parameters.Add("p_last_name", user.LastName);
            parameters.Add("p_email", user.Email);
            parameters.Add("p_phone", user.Phone);
            parameters.Add("p_gender", user.Gender);
            parameters.Add("p_dob", user.Dob);

            return await _dapperHelper.ExecuteNonQueryAsync("sp_UpdateUser", parameters);
        }
        public async Task<UserResponseDto?> GetUserByEmailAsync(string email)
        {
            var parameters = new DynamicParameters();
            parameters.Add("p_email", email);

            var result = await _dapperHelper.QueryFirstOrDefaultAsync<dynamic>(
                "sp_GetUserByEmail",
                parameters,
                CommandType.StoredProcedure
            );

            if (result == null) return null;

            return new UserResponseDto
            {
                UserId = Guid.Parse(result.user_id.ToString()),
                FirstName = result.first_name,
                LastName = result.last_name,
                Email = result.email,
                Phone = result.phone,
                Gender = result.gender,
                Dob = result.dob,
                Status = "Active"
            };
        }

    }
}
