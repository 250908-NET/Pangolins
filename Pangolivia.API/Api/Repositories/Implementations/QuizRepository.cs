using AutoMapper;
using Microsoft.EntityFrameworkCore;
using Pangolivia.API.Data;
using Pangolivia.API.Models;

namespace Pangolivia.API.Repositories;

public class QuizRepository : IQuizRepository
{
    private readonly PangoliviaDbContext _context;

    public QuizRepository(PangoliviaDbContext context)
    {
        _context = context;
    }

    // Create quiz
    public async Task<QuizModel> AddAsync(QuizModel quiz)
    {
        await _context.Quizzes.AddAsync(quiz);
        await SaveChangesAsync();
        return quiz;
    }

    // Update quiz
    public void UpdateAsync(QuizModel quiz)
    {
        _context.Quizzes.Update(quiz);
    }

    // Delete quiz
    public void DeleteAsync(QuizModel quiz)
    {
        _context.Quizzes.Remove(quiz);
    }

    // Get quiz by Id including creator and questions
    // NOTE: not working yet since Questions and Users is not defined
    public async Task<QuizModel?> GetByIdWithDetailsAsync(int quizId)
    {
        return await _context
            .Quizzes.Include(q => q.CreatedByUser)
            .Include(q => q.Questions)
            .FirstOrDefaultAsync(q => q.Id == quizId);
    }

    // Get quiz by Id
    public async Task<QuizModel?> GetByIdAsync(int quizId)
    {
        return await _context.Quizzes.FindAsync(quizId);
    }

    // Get all quizzes
    // NOTE: not working yet since Questions and Users is not defined
    public async Task<List<QuizModel>> GetAllAsync()
    {
        return await _context
            .Quizzes.Include(q => q.CreatedByUser)
            .Include(q => q.Questions)
            .ToListAsync();
    }

    // Find quizzes by name (include creator and questions)
    public async Task<List<QuizModel>> FindByNameAsync(string name)
    {
        return await _context
            .Quizzes.Include(q => q.CreatedByUser)
            .Include(q => q.Questions)
            .Where(q => q.QuizName.Contains(name))
            .ToListAsync();
    }

    // Get quizzes created by a specific userId
    public async Task<List<QuizModel>> GetByUserIdAsync(int userId)
    {
        return await _context
            .Quizzes.Include(q => q.CreatedByUser)
            .Include(q => q.Questions)
            .Where(q => q.CreatedByUserId == userId)
            .ToListAsync();
    }

    // Save changes
    public async Task<bool> SaveChangesAsync()
    {
        return await _context.SaveChangesAsync() > 0;
    }
}
