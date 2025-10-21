using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using Microsoft.IdentityModel.Tokens;
using Pangolivia.API.DTOs;
using Pangolivia.API.Models;
using Pangolivia.API.Repositories;

namespace Pangolivia.API.Services
{
    public class AuthService : IAuthService
    {
        private readonly IUserRepository _userRepository;
        private readonly IConfiguration _configuration;

        public AuthService(IUserRepository userRepository, IConfiguration configuration)
        {
            _userRepository = userRepository;
            _configuration = configuration;
        }

        public async Task<UserModel> RegisterAsync(UserRegisterDto userRegisterDto)
        {
            var existingUser = await _userRepository.getUserModelByUsername(
                userRegisterDto.Username
            );

            if (existingUser != null)
            {
                throw new ArgumentException("Username already exists.");
            }

            var user = new UserModel
            {
                Username = userRegisterDto.Username,
                // We hash the password before saving
                PasswordHash = BCrypt.Net.BCrypt.HashPassword(userRegisterDto.Password),
                AuthUuid = Guid.NewGuid().ToString(), // For local users, we can generate a UUID
            };

            // This now passes the complete user object with the hashed password
            await _userRepository.createUserModel(user);

            return user;
        }

        public async Task<LoginResponseDto?> LoginAsync(UserLoginDto userLoginDto)
        {
            var user = await _userRepository.getUserModelByUsername(userLoginDto.Username);

            if (user == null || !BCrypt.Net.BCrypt.Verify(userLoginDto.Password, user.PasswordHash))
            {
                return null; // Invalid credentials
            }

            var token = GenerateJwtToken(user);

            return new LoginResponseDto
            {
                UserId = user.Id,
                Username = user.Username,
                Token = token,
            };
        }

        private string GenerateJwtToken(UserModel user)
        {
            var securityKey = new SymmetricSecurityKey(
                Encoding.UTF8.GetBytes(_configuration["Jwt:Key"]!)
            );
            var credentials = new SigningCredentials(securityKey, SecurityAlgorithms.HmacSha256);

            var claims = new[]
            {
                new Claim(JwtRegisteredClaimNames.Sub, user.Id.ToString()),
                new Claim(ClaimTypes.NameIdentifier, user.Id.ToString()), // Standard claim for user ID
                new Claim(ClaimTypes.Name, user.Username), // Use ClaimTypes.Name for compatibility
                new Claim(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString()),
            };

            var token = new JwtSecurityToken(
                issuer: _configuration["Jwt:Issuer"],
                audience: _configuration["Jwt:Audience"],
                claims: claims,
                expires: DateTime.Now.AddMinutes(30),
                signingCredentials: credentials
            );

            return new JwtSecurityTokenHandler().WriteToken(token);
        }
    }
}
