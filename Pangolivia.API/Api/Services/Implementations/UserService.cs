using Pangolivia.API.DTOs;
using Pangolivia.API.Models;
using Pangolivia.API.Repositories;
using Pangolivia.API.Services;

public class UserService : IUserService
{

    private IUserRepository _context;


    public UserService(IUserRepository context)
    {
        _context = context;
    }

    public Task<List<UserModel>> getAllUsersAsync()
    {
        return _context.getAllUserModels();
    }

    public Task<UserModel> getUserByIdAsync(int id)
    {
        return _context.getUserModelById(id);
    }
    public Task<UserModel?> findUserByUsernameAsync(string username)
    {
        return _context.getUserModelByUsername(username);
    }

    public Task<UserModel> createUserAsync(UserModel User)
    {
        return _context.createUserModel(User);
    }

    public Task<UserModel> updateUserAsync(int userId, object Obj)
    {
        switch (Obj)
        {
            case PlayerGameRecordDto pgrDTO:
                return _context.updateUserModelPlayerGameRecord(userId, pgrDTO);
            case GameRecordModel GRM:
                return _context.updateUserModelHostedGameRecord(userId, GRM);
            case QuizModel quiz:
                return _context.updateUserModelCreatedQuizzes(userId, quiz);
            default:
                throw new ArgumentException("Unknown Type: should be type:PlayerGameRecordDto, GameRecordModel, QuizModel");
        }
    }

    public async Task deleteUserAsync(int id)
    {
        await _context.removeUserModel(id);
    }


}