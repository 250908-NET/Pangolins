using Microsoft.EntityFrameworkCore;
using Pangolivia.Data;
using Pangolivia.Models;
using Pangolivia.Repositories.Interfaces;

namespace Pangolivia.Repositories
{
    public class UserRepository : IUserRepository
    {
        private readonly ApplicationDbContext _context;
        private readonly ILogger<UserRepository> _logger;

        public UserRepository(ApplicationDbContext context, ILogger<UserRepository> logger)
        {
            _context = context;
            _logger = logger;
        }

        public async Task<User?> GetByIdAsync(int id)
        {
            try
            {
                return await _context.Users.FindAsync(id);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error retrieving user with ID {UserId}", id);
                throw;
            }
        }

        public async Task<User?> GetByAuthUuidAsync(string authUuid)
        {
            try
            {
                return await _context.Users
                    .FirstOrDefaultAsync(u => u.AuthUuid == authUuid);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error retrieving user with AuthUuid {AuthUuid}", authUuid);
                throw;
            }
        }

        public async Task<User?> GetByUsernameAsync(string username)
        {
            try
            {
                return await _context.Users
                    .FirstOrDefaultAsync(u => u.Username == username);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error retrieving user with username {Username}", username);
                throw;
            }
        }

        public async Task<IEnumerable<User>> GetAllAsync()
        {
            try
            {
                return await _context.Users.ToListAsync();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error retrieving all users");
                throw;
            }
        }

        public async Task<User> CreateAsync(User user)
        {
            try
            {
                user.CreatedAt = DateTime.UtcNow;
                _context.Users.Add(user);
                await _context.SaveChangesAsync();
                
                _logger.LogInformation("Created new user with ID {UserId}", user.Id);
                return user;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error creating user");
                throw;
            }
        }

        public async Task<User?> UpdateAsync(User user)
        {
            try
            {
                var existingUser = await _context.Users.FindAsync(user.Id);
                if (existingUser == null)
                {
                    _logger.LogWarning("User with ID {UserId} not found for update", user.Id);
                    return null;
                }

                existingUser.AuthUuid = user.AuthUuid;
                existingUser.Username = user.Username;
                existingUser.UpdatedAt = DateTime.UtcNow;

                await _context.SaveChangesAsync();
                
                _logger.LogInformation("Updated user with ID {UserId}", user.Id);
                return existingUser;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error updating user with ID {UserId}", user.Id);
                throw;
            }
        }

        public async Task<bool> DeleteAsync(int id)
        {
            try
            {
                var user = await _context.Users.FindAsync(id);
                if (user == null)
                {
                    _logger.LogWarning("User with ID {UserId} not found for deletion", id);
                    return false;
                }

                _context.Users.Remove(user);
                await _context.SaveChangesAsync();
                
                _logger.LogInformation("Deleted user with ID {UserId}", id);
                return true;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error deleting user with ID {UserId}", id);
                throw;
            }
        }

        public async Task<bool> ExistsByAuthUuidAsync(string authUuid)
        {
            try
            {
                return await _context.Users.AnyAsync(u => u.AuthUuid == authUuid);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error checking existence of AuthUuid {AuthUuid}", authUuid);
                throw;
            }
        }

        public async Task<bool> ExistsByUsernameAsync(string username)
        {
            try
            {
                return await _context.Users.AnyAsync(u => u.Username == username);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error checking existence of username {Username}", username);
                throw;
            }
        }
    }
}