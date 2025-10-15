using Pangolivia.Models;

namespace Pangolivia.Services;

public interface IQuizService
{
    Task<List<Quiz>> GetAllAsync();
    Task<Quiz?> GetByIdAsync(int id);
    Task<Quiz> CreateAsync(Quiz quiz);
    Task<bool> UpdateAsync(int id, Quiz updatedQuiz);
    Task<bool> DeleteAsync(int id);
}