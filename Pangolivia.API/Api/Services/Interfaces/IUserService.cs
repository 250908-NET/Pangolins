using System.Collections.Generic;
using System.Threading.Tasks;
using Pangolivia.API.DTOs;
using Pangolivia.API.Models;

namespace Pangolivia.API.Services;

public interface IUserService
{
    Task<IEnumerable<UserSummaryDto>> getAllUsersAsync();
    Task<UserDetailDto?> getUserByIdAsync(int id);
    Task<UserDetailDto?> findUserByUsernameAsync(string username);
    Task deleteUserAsync(int id);
}
