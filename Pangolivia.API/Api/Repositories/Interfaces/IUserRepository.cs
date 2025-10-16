using Pangolivia.API.DTOs;
using Pangolivia.API.Models;

public interface IUserRepository
{
    Task<List<UserModel>> getAllUserModels();
    Task<UserModel> getUserModelById(int id);
    Task<UserModel> getUserModelByUsername(string username);
    Task<UserModel> createUserModel(CreateUserDTO userDTO);
    Task<UserModel> updateUserModelPlayerGameRecord(int id, PlayerGameRecordDto PGR);
    Task<UserModel> updateUserModelHostedGameRecord(int id, GameRecordModel GRM);
    Task<UserModel> updateUserModelCreatedQuizzes(int id, QuizModel quiz);
    Task removeUserModel(int id);
}