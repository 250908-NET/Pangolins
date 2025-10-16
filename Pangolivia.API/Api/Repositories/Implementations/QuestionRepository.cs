using Microsoft.EntityFrameworkCore;
using Pangolivia.API.Data;
using Pangolivia.API.Models;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Pangolivia.Repositories
{

        public class QuestionRepository : IQuestionRepository
    {
        private readonly AppDbContext _context;

        public QuestionRepository(AppDbContext context)
        {
            _context = context;
        }

        public async Task<IEnumerable<Question>> GetAllAsync()
        {
            return await _context.Questions.ToListAsync();
        }

        public async Task<Question?> GetByIdAsync(int id)
        {
            return await _context.Questions.FindAsync(id);
        }

        public async Task<IEnumerable<Question>> GetByQuizIdAsync(int quizId)
        {
            return await _context.Questions
                .Where(q => q.QuizId == quizId)
                .ToListAsync();
        }

        public async Task<Question> AddAsync(Question question)
        {
            _context.Questions.Add(question);
            await _context.SaveChangesAsync();
            return question;
        }

        public async Task<Question?> UpdateAsync(Question question)
        {
            var existing = await _context.Questions.FindAsync(question.Id);
            if (existing == null)
                return null;

            _context.Entry(existing).CurrentValues.SetValues(question);
            await _context.SaveChangesAsync();
            return existing;
        }

        public async Task<bool> DeleteAsync(int id)
        {
            var question = await _context.Questions.FindAsync(id);
            if (question == null)
                return false;

            _context.Questions.Remove(question);
            await _context.SaveChangesAsync();
            return true;
        }
    }
}