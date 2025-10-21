using Pangolivia.API.Models;
namespace Pangolivia.API.Repositories;

public interface IUserRepository
{
    Task<List<UserModel>> getAllUserModels(); // Changed from Task<List<UserDto>>
    Task<UserModel?> getUserModelById(int id); 
    public Task<UserModel?> getUserModelByUsername(string username); 
    Task<UserModel> createUserModel(UserModel user);
    Task removeUserModel(int id);
}