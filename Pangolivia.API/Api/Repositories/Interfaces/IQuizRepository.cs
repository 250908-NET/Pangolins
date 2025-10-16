using Pangolivia.API.Models;

namespace Pangolivia.API.Repositories;

public interface IQuizRepository
{
    Task<QuizModel> AddAsync(QuizModel quiz);
    void UpdateAsync(QuizModel quiz);
    void DeleteAsync(QuizModel quiz);
    Task<QuizModel?> GetByIdWithDetailsAsync(int quizId);
    Task<QuizModel?> GetByIdAsync(int quizId);
    Task<List<QuizModel>> GetAllAsync();
    Task<List<QuizModel>> FindByNameAsync(string name);
    Task<List<QuizModel>> GetByUserIdAsync(int userId);
    Task<bool> SaveChangesAsync();
}
