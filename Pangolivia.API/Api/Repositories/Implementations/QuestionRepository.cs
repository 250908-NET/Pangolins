// NOTE: to fix later, needed a basic QuestionRepo so the Quiz can run.

using Microsoft.EntityFrameworkCore;
using Pangolivia.API.Data;
using Pangolivia.API.Models;

namespace Pangolivia.API.Repositories;

public class QuestionRepository : IQuestionRepository
{
    private readonly PangoliviaDbContext _context;

    public QuestionRepository(PangoliviaDbContext context)
    {
        _context = context;
    }

    public async Task AddAsync(QuestionModel question)
    {
        await _context.Questions.AddAsync(question);
    }

    public async Task UpdateAsync(QuestionModel question)
    {
        _context.Questions.Update(question);
        await Task.CompletedTask;
    }

    public async Task DeleteAsync(QuestionModel question)
    {
        _context.Questions.Remove(question);
        await Task.CompletedTask;
    }

    public async Task<QuestionModel?> GetByIdAsync(int id)
    {
        return await _context.Questions
            .FirstOrDefaultAsync(q => q.Id == id);
    }

    public async Task SaveChangesAsync()
    {
        await _context.SaveChangesAsync();
    }
}
