using Microsoft.EntityFrameworkCore;
using Pangolivia.API.Data;
using Pangolivia.API.Models;

namespace Pangolivia.API.Repositories;

public class UserRepository : IUserRepository
{
    private readonly PangoliviaDbContext _context; // Changed to readonly for best practice

    public UserRepository(PangoliviaDbContext context)
    {
        _context = context;
    }

    public async Task<List<UserModel>> getAllUserModels()
    {
        return await _context
            .Users.Include(u => u.CreatedQuizzes)
            .ThenInclude(q => q.Questions)
            .Include(u => u.PlayerGameRecords)
            .ThenInclude(pgr => pgr.GameRecord)
            .Include(u => u.HostedGameRecords)
            .ThenInclude(gr => gr.Quiz)
            .ToListAsync();
    }

    public async Task<UserModel?> getUserModelById(int id)
    {
        return await _context
            .Users.Include(u => u.CreatedQuizzes)
            .Include(u => u.HostedGameRecords)
            .Include(u => u.PlayerGameRecords)
            .FirstOrDefaultAsync(user => user.Id == id);
    }

    public async Task<UserModel?> getUserModelByUsername(string username)
    {
        return await _context
            .Users.Include(u => u.CreatedQuizzes)
            .Include(u => u.HostedGameRecords)
            .Include(u => u.PlayerGameRecords)
            .FirstOrDefaultAsync(u => u.Username == username);
    }

    public async Task<UserModel> createUserModel(UserModel newUser)
    {
        _context.Users.Add(newUser);
        await _context.SaveChangesAsync();
        return newUser;
    }

    public async Task removeUserModel(int id)
    {
        UserModel? user = await _context
            .Users.Include(u => u.CreatedQuizzes)
            .Include(u => u.PlayerGameRecords)
            .Include(u => u.HostedGameRecords)
            .FirstOrDefaultAsync(u => u.Id == id);

        if (user == null)
        {
            throw new KeyNotFoundException($"UserModel with id {id} not found.");
        }

        // EF Core with cascade delete should handle this, but being explicit can be safer depending on configuration.
        // The existing logic is fine.
        _context.Quizzes.RemoveRange(user.CreatedQuizzes);
        _context.PlayerGameRecords.RemoveRange(user.PlayerGameRecords);
        _context.GameRecords.RemoveRange(user.HostedGameRecords);

        _context.Users.Remove(user);
        await _context.SaveChangesAsync();
    }
}
