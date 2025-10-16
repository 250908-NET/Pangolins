using Microsoft.VisualBasic;
using Pangolivia.API.Data;
using Pangolivia.API.DTOs;
using Pangolivia.API.Models;

public class UserRepository
{
    private PangoliviaDbContext _context;
    public UserRepository(PangoliviaDbContext context)
    {
        _context = context;
    }
    Task<List<UserModel>> getAllUserModels()
    {
        throw new NotImplementedException();
    }

    Task<UserModel> getUserModelById(int id)
    {
        throw new NotImplementedException();
    }

    Task<UserModel> getUserModelByUsername(string username)
    {
        throw new NotImplementedException();
    }

    Task<UserModel> createUserModel(CreateUserDTO userDTO)
    {
        throw new NotImplementedException();
    }
    // Update methods*********************************
    Task<UserModel> updateUserModelPlayerGameRecord(int id, PlayerGameRecordDto PGR)
    {
        throw new NotImplementedException();
    }
    Task<UserModel> updateUserModelHostedGameRecord(int id, GameRecordModel GRM)
    {
        throw new NotImplementedException();
    }
    Task<UserModel> updateUserModelCreatedQuizzes(int id, QuizModel quiz)
    {
        throw new NotImplementedException();
    }
    // **********************************************
    Task removeUserModel(int id)
    {
        throw new NotImplementedException();
    }
}