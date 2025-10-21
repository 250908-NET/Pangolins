using Pangolivia.API.DTOs;
using Pangolivia.API.Models;

namespace Pangolivia.API.Services;

public interface IUserService
{
    Task<List<UserModel>> getAllUsersAsync();
    Task<UserModel> getUserByIdAsync(int id);
    Task<UserModel?> findUserByUsernameAsync(string username);

    // Task<UserModel> createUserAsync(CreateUserDTO userDTO);
    Task<UserModel> updateUserAsync(int userId, object Obj);
    Task deleteUserAsync(int id);
}
