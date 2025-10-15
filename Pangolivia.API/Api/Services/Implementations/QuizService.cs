// using Pangolivia.Models;
// using Pangolivia.Repositories;

// namespace Pangolivia.Services;

// public class QuizService : IQuizService
// {
//     private readonly IQuizRepository _repo;

//     public QuizService(IQuizRepository repo)
//     {
//         _repo = repo;
//     }

//     public async Task<List<Quiz>> GetAllAsync() => await _repo.GetAllAsync();

//     public async Task<Quiz?> GetByIdAsync(int id) => await _repo.GetByIdAsync(id);

//     public async Task<Quiz> CreateAsync(Quiz quiz)
//     {
//         await _repo.AddAsync(quiz);
//         await _repo.SaveChangesAsync();
//         return quiz;
//     }

//     public async Task<bool> UpdateAsync(int id, Quiz updatedQuiz)
//     {
//         var existing = await _repo.GetByIdAsync(id);
        
//         if (existing == null)
//         {
//             return false;
//         }

//         existing.QuizName = updatedQuiz.QuizName;
//         existing.CreatedByUserId = updatedQuiz.CreatedByUserId;

//         await _repo.UpdateAsync(existing);
//         await _repo.SaveChangesAsync();
//         return true;
//     }

//     public async Task<bool> DeleteAsync(int id)
//     {
//         var existing = await _repo.GetByIdAsync(id);
//         if (existing == null)
//         {
//             return false;
//         }

//         await _repo.DeleteAsync(id);
//         await _repo.SaveChangesAsync();
//         return true;
//     }
// }