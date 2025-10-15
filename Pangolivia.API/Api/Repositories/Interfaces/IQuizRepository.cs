using Pangolivia.API.Models;

namespace Pangolivia.Repositories;

public interface IQuizRepository
{
    Task<List<QuizModel>> GetAllAsync();
    Task<QuizModel?> GetByIdAsync(int id);
    Task AddAsync(QuizModel quiz);
    Task UpdateAsync(QuizModel quiz);
    Task DeleteAsync(int id);
    Task SaveChangesAsync();
}
