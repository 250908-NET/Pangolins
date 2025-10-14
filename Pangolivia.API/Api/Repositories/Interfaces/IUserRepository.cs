using Pangolivia.Models;

namespace Pangolivia.Repositories.Interfaces
{
    public interface IUserRepository
    {
        // Get operations
        Task<User?> GetByIdAsync(int id);
        Task<User?> GetByAuthUuidAsync(string authUuid);
        Task<User?> GetByUsernameAsync(string username);
        Task<IEnumerable<User>> GetAllAsync();

        // Create operation
        Task<User> CreateAsync(User user);

        // Update operation
        Task<User?> UpdateAsync(User user);

        // Delete operation
        Task<bool> DeleteAsync(int id);

        // Existence checks
        Task<bool> ExistsByAuthUuidAsync(string authUuid);
        Task<bool> ExistsByUsernameAsync(string username);
    }
}