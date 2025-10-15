using Pangolivia.Models;

namespace Pangolivia.Repositories;

public interface IQuizRepository
{
    Task<Quiz> AddAsync(Quiz quiz);
    void UpdateAsync(Quiz quiz);
    void DeleteAsync(Quiz quiz);
    Task<Quiz?> GetByIdWithDetailsAsync(int quizId);
    Task<Quiz?> GetByIdAsync(int quizId);
    Task<List<Quiz>> GetAllAsync();
    Task<List<Quiz>> FindByNameAsync(string name);
    Task<List<Quiz>> GetByUserIdAsync(int userId);
    Task<bool> SaveChangesAsync();
}

