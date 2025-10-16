using Pangolivia.API.Models;

namespace Pangolivia.API.Repositories;

public interface IUserRepository
{
    Task<UserModel?> GetByIdAsync(int userId);
}