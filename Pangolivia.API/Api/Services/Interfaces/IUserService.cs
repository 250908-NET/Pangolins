using Pangolivia.API.Models;
using Pangolivia.API.DTOs;

namespace Pangolivia.API.Services;

public interface IUserService
{
    Task<List<UserDto>> getAllUsersAsync();
    Task<UserModel> getUserByIdAsync(int id);
    Task<UserDto?> findUserByUsernameAsync(string username);
    Task<UserModel> createUserAsync(UserModel userDTO);
    Task<UserModel> updateUserAsync(int userId, object Obj);
    Task deleteUserAsync(int id);

}