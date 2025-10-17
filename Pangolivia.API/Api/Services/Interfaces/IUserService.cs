using Pangolivia.API.Models;
using Pangolivia.API.DTOs;
using Microsoft.VisualBasic;

namespace Pangolivia.API.Services;

public interface IUserService
{
    Task<List<UserModel>> GetAllUsersAsync();

    Task<UserModel?> GetUserIdAsync(int id);
    Task<UserModel> CreateUserAsync(CreateUserDTO userDTO);


    Task<UserModel> UpdateUserAsync(int userId, object Obj);
    Task DeleteUserAsync(int id, int currentUserId);
    Task<List<UserModel>> GetUserByUserIdAsync(int userId);
    Task<List<UserModel>> FindUserByUsernameAsync(string query);
}