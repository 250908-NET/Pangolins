using Microsoft.EntityFrameworkCore.Query.SqlExpressions;
using Microsoft.VisualBasic;
using Pangolivia.API.DTOs;
using Pangolivia.API.Models;
using Pangolivia.API.Repositories;

public class UserService
{

    private UserRepository _context;


    public UserService(UserRepository context)
    {
        _context = context;
    }

    public Task<List<UserModel>> getAllUsersAsync()
    {
        return _context.getAllUserModels();
    }

    public Task<UserModel> getUserByUserIdAsync(int id)
    {
        return _context.getUserModelById(id);
    }

    public Task<UserModel> createUserAsync(CreateUserDTO userDTO)
    {
        return _context.createUserModel(userDTO);
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