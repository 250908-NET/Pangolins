using System.Collections.Generic;
using System.Threading.Tasks;
using Pangolivia.API.Models;

namespace Pangolivia.Repositories
{
    public interface IQuestionRepository
    {
        Task<IEnumerable<Question>> GetAllAsync();
        Task<Question?> GetByIdAsync(int id);
        Task<IEnumerable<Question>> GetByQuizIdAsync(int quizId);
        Task<Question> AddAsync(Question question);
        Task<Question?> UpdateAsync(Question question);
        Task<bool> DeleteAsync(int id);
    }
}