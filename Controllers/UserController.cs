using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Serilog;
using TMCC.Models.DTO;
using TMCC.Services.IServices;

namespace TMCC.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class UserController : ControllerBase
    {
        private readonly IUserService _userService;
        private readonly IJwtService _jwtService;

        public UserController(IUserService userService, IJwtService jwtService)
        {
            _userService = userService;
            _jwtService = jwtService;
        }

        [AllowAnonymous]
        [HttpPost("register")]
        public async Task<IActionResult> Register([FromBody] RegisterUserDto user)
        {
            try
            {
                await _userService.RegisterAsync(user);
                return Ok(new { message = "User registered successfully" });
            }
            catch (Exception ex)
            {
                Serilog.Log.Error(ex, "Error during user registration for Email: {Email}", user.Email);
                return BadRequest(new { message = ex.Message });
            }
        }

        [AllowAnonymous]
        [HttpPost("login")]
        public async Task<IActionResult> Login([FromBody] LoginUserDto login)
        {
            var user = await _userService.LoginAsync(login);
            if (user == null)
            {
                Serilog.Log.Warning("Login failed for Email: {Email}", login.Email);
                return Unauthorized(new { message = "Invalid email or password" });
            }

            // Generate token and token info
            var (tokenString, tokenInfo) = _jwtService.GenerateToken(
                user.UserId.ToString(),
                user.Email,
                user.FirstName,
                user.LastName
            );

            // Log the generated token (for debugging only; in production consider masking)
            Serilog.Log.Information("Generated JWT token for UserId: {UserId}, Token: {Token}", user.UserId, tokenString);

            return Ok(new
            {
                message = "Login successful",
                token = new
                {
                    tokenString,
                    tokenInfo
                },
                user
            });
        }

        [Authorize]
        [HttpPut("update")]
        public async Task<IActionResult> Update([FromBody] UpdateUserDto user)
        {
            try
            {
                await _userService.UpdateUserAsync(user);
                return Ok(new { message = "User updated successfully" });
            }
            catch (Exception ex)
            {
                Serilog.Log.Error(ex, "Error updating user with Email: {Email}", user.Email);
                return BadRequest(new { message = ex.Message });
            }
        }

        [Authorize]
        [HttpGet("getByEmail/{email}")]
        public async Task<IActionResult> GetByEmail(string email)
        {
            try
            {
                var user = await _userService.GetUserByEmailAsync(email);
                if (user == null)
                {
                    Serilog.Log.Warning("GetByEmail failed. User not found for Email: {Email}", email);
                    return NotFound(new { message = "User not found" });
                }

                return Ok(user);
            }
            catch (Exception ex)
            {
                Serilog.Log.Error(ex, "Error fetching user by Email: {Email}", email);
                return BadRequest(new { message = ex.Message });
            }
        }

        [Authorize]
        [HttpGet("profile")]
        public async Task<IActionResult> GetProfile()
        {
            try
            {
                var userId = User.FindFirst(System.Security.Claims.ClaimTypes.NameIdentifier)?.Value;

                if (string.IsNullOrEmpty(userId))
                {
                    Serilog.Log.Warning("Profile request failed. Invalid token.");
                    return Unauthorized(new { message = "Invalid token" });
                }

                var email = User.FindFirst(System.Security.Claims.ClaimTypes.Email)?.Value;
                var user = await _userService.GetUserByEmailAsync(email);

                if (user == null)
                {
                    Serilog.Log.Warning("Profile request failed. User not found for Email: {Email}", email);
                    return NotFound(new { message = "User not found" });
                }

                Serilog.Log.Information("Profile retrieved successfully for UserId: {UserId}", userId);
                return Ok(user);
            }
            catch (Exception ex)
            {
                Serilog.Log.Error(ex, "Error fetching profile for token.");
                return BadRequest(new { message = ex.Message });
            }
        }
    }
}
