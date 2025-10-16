using Pangolivia.API.DTOs;
using Pangolivia.API.Models;

public interface IUserRepository
{
    public Task<List<UserModel>> getAllUserModels();
    public Task<UserModel> getUserModelById(int id);
    public Task<UserModel> getUserModelByUsername(string username);
    public Task<UserModel> createUserModel(CreateUserDTO userDTO);
    public Task<UserModel> updateUserModelPlayerGameRecord(int id, PlayerGameRecordDto PGR);
    public Task<UserModel> updateUserModelHostedGameRecord(int id, GameRecordModel GRM);
    public Task<UserModel> updateUserModelCreatedQuizzes(int id, QuizModel quiz);
    public Task removeUserModel(int id);
}