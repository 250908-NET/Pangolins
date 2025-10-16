using Pangolivia.API.Models;

namespace Pangolivia.API.Repositories;

public interface IQuestionRepository
{
    Task AddAsync(QuestionModel question);
    Task UpdateAsync(QuestionModel question);
    Task DeleteAsync(QuestionModel question);
    Task<QuestionModel?> GetByIdAsync(int id);
    Task SaveChangesAsync();
}
