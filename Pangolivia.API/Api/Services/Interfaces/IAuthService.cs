using Pangolivia.API.DTOs;
using Pangolivia.API.Models;

namespace Pangolivia.API.Services
{
    public interface IAuthService
    {
        Task<UserModel> RegisterAsync(UserRegisterDto userRegisterDto);
        Task<LoginResponseDto?> LoginAsync(UserLoginDto userLoginDto);
    }
}
