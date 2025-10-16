using Microsoft.EntityFrameworkCore;
using AutoMapper;
using Pangolivia.Data;
using Pangolivia.Models;

namespace Pangolivia.Repositories;

public class QuizRepository : IQuizRepository
{
    private readonly PangoliviaDbContext _context;
    private readonly IMapper _mapper;

    public QuizRepository(PangoliviaDbContext context, IMapper mapper)
    {
        _context = context;
        _mapper = mapper;
    }

    // Create quiz
    public async Task<Quiz> AddAsync(Quiz quiz)
    {
        await _context.Quizzes.AddAsync(quiz);
        await SaveChangesAsync();
        return quiz;
    }

    // Update quiz
    public void UpdateAsync(Quiz quiz)
    {
        _context.Quizzes.Update(quiz);
    }

    // Delete quiz
    public void DeleteAsync(Quiz quiz)
    {
        _context.Quizzes.Remove(quiz);
    }

    // Get quiz by Id including creator and questions
    // NOTE: not working yet since Questions and Users is not defined
    public async Task<Quiz?> GetByIdWithDetailsAsync(int quizId)
    {
        return await _context.Quizzes
            .Include(q => q.CreatedByUser)
            .Include(q => q.Questions)
            .FirstOrDefaultAsync(q => q.Id == quizId);
    }

    // Get quiz by Id
    public async Task<Quiz?> GetByIdAsync(int quizId)
    {
        return await _context.Quizzes.FindAsync(quizId);
    }

    // Get all quizzes
    public async Task<List<Quiz>> GetAllAsync()
    {
        return await _context.Quizzes
            .Include(q => q.CreatedByUser)
            .Include(q => q.Questions)
            .ToListAsync();
    }

    // Find quizzes by name (include creator and questions)
    public async Task<List<Quiz>> FindByNameAsync(string name)
    {
        return await _context.Quizzes
            .Include(q => q.CreatedByUser)
            .Include(q => q.Questions)
            .Where(q => q.QuizName.Contains(name))
            .ToListAsync();
    }

    // Get quizzes created by a specific userId
    public async Task<List<Quiz>> GetByUserIdAsync(int userId)
    {
        return await _context.Quizzes
            .Include(q => q.CreatedByUser)
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