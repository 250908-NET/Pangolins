using Microsoft.EntityFrameworkCore;
using Pangolivia.API.Data;
using Pangolivia.API.DTOs;
using Pangolivia.API.Models;
namespace Pangolivia.API.Repositories;

public class UserRepository : IUserRepository
{
    private PangoliviaDbContext _context;
    public UserRepository(PangoliviaDbContext context)
    {
        _context = context;
    }
    public async Task<List<UserModel>> getAllUserModels()
    {
        return await _context.Users
        // .Include(u => u.Username)
        .ToListAsync();
        // throw new NotImplementedException();
    }

    public async Task<UserModel> getUserModelById(int id)
    {
        var user = await _context.Users.Include(u => u.Username)
                    .FirstOrDefaultAsync(user => user.Id == id);
        if (user != null)
        {
            return user;
        }
        throw new KeyNotFoundException($"UserModel with id {id} not found.");

    }



    public async Task<UserModel?> getUserModelByUsername(string username)
    {
        var user = await _context.Users
        .Include(u => u.PlayerGameRecords)
        .Include(u => u.HostedGameRecords)
        .Include(u => u.CreatedQuizzes)
        .FirstOrDefaultAsync(u => u.Username == username);

        return user;
    }

    public async Task<UserModel> createUserModel(UserModel newUser)
    {
        // UserModel newUser = new UserModel
        // {
        //     AuthUuid = userDto.authUuid,
        //     Username = userDto.username
        // };

        _context.Users.Add(newUser);
        await _context.SaveChangesAsync();
        return newUser;
    }
    // Update methods*********************************
    public async Task<UserModel> updateUserModelPlayerGameRecord(int id, PlayerGameRecordDto pgrDto)
    {
        var user = await _context.Users
    .Include(u => u.PlayerGameRecords)
    .FirstOrDefaultAsync(u => u.Id == id);

        if (user == null)
        {
            throw new KeyNotFoundException($"UserModel with id {id} not found. PlayerGameRecord NOT Updated");
        }

        var playerGameRecord = new PlayerGameRecordModel
        {
            GameRecordId = pgrDto.GameRecordId,
            UserId = pgrDto.UserId,
            score = pgrDto.score
        };

        user.PlayerGameRecords.Add(playerGameRecord);

        await _context.SaveChangesAsync();

        return user;
        // throw new NotImplementedException();
    }

    public async Task<UserModel> updateUserModelHostedGameRecord(int id, GameRecordModel GRM)
    {
        var user = await _context.Users
        .Include(u => u.Id)
        .Include(u => u.HostedGameRecords)
        .FirstOrDefaultAsync(u => u.Id == id);

        if (user == null)
        {
            throw new KeyNotFoundException($"UserModel with id {id} not found. HostedGameRecord NOT Updated");
        }
        // Assign the foreign key 
        GRM.HostUserId = user.Id;

        user.HostedGameRecords.Add(GRM);

        await _context.SaveChangesAsync();

        return user;
        // throw new NotImplementedException();
    }
    public async Task<UserModel> updateUserModelCreatedQuizzes(int id, QuizModel quiz)
    {
        var user = await _context.Users
        .Include(u => u.Id)
        .Include(u => u.CreatedQuizzes)
        .FirstOrDefaultAsync(u => u.Id == id);

        if (user == null)
        {
            throw new KeyNotFoundException($"UserModel with id {id} not found. CreatedQuizzes NOT Updated");
        }
        // link quize to user
        quiz.CreatedByUserId = user.Id;

        user.CreatedQuizzes.Add(quiz);

        await _context.SaveChangesAsync();

        return user;
        // throw new NotImplementedException();
    }
    // **********************************************
    public async Task removeUserModel(int id)
    {
        var user = await _context.Users.FirstOrDefaultAsync(u => u.Id == id);

        if (user == null)
            throw new KeyNotFoundException($"UserModel with id {id} not found.");

        _context.Users.Remove(user);
        await _context.SaveChangesAsync();

        // throw new NotImplementedException();
    }
}