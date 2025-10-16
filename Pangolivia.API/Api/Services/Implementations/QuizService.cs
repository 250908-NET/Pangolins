using AutoMapper;
using Microsoft.EntityFrameworkCore;
using Pangolivia.API.DTOs;
using Pangolivia.API.Models;
using Pangolivia.API.Repositories;
using Pangolivia.API.Services;

namespace Pangolivia.Services.Implementations;

public class QuizService : IQuizService
{
    private readonly IQuizRepository _quizRepository;
    private readonly IQuestionRepository _questionRepository;
    private readonly IMapper _mapper;

    public QuizService(IQuizRepository quizRepository, IQuestionRepository questionRepository, IMapper mapper)
    {
        _quizRepository = quizRepository;
        _questionRepository = questionRepository;
        _mapper = mapper;
    }

    // Create Quiz
    public async Task<QuizDetailDto> CreateQuizAsync(CreateQuizRequestDto requestDto, int creatorUserId)
    {
        var quiz = new QuizModel
        {
            QuizName = requestDto.QuizName,
            CreatedByUserId = creatorUserId
        };

        await _quizRepository.AddAsync(quiz);

        foreach (var questionDto in requestDto.Questions)
        {
            var question = new QuestionModel
            {
                QuestionText = questionDto.QuestionText,
                QuizId = quiz.Id
            };
            await _questionRepository.AddAsync(question);
        }

        await _quizRepository.SaveChangesAsync();

        var detailedQuiz = await _quizRepository.GetByIdWithDetailsAsync(quiz.Id);
        return _mapper.Map<QuizDetailDto>(detailedQuiz);
    }

    // UPDATE QUIZ
    public async Task<QuizDetailDto> UpdateQuizAsync(int quizId, UpdateQuizRequestDto requestDto, int currentUserId)
    {
        var existingQuiz = await _quizRepository.GetByIdWithDetailsAsync(quizId);
        if (existingQuiz == null)
            throw new KeyNotFoundException("Quiz not found");

        if (existingQuiz.CreatedByUserId != currentUserId)
            throw new UnauthorizedAccessException("You are not authorized to update this quiz");

        // Update quiz fields
        existingQuiz.QuizName = requestDto.QuizName;
        _quizRepository.UpdateAsync(existingQuiz);

        // Handle question updates
        var existingQuestionIds = existingQuiz.Questions.Select(q => q.Id).ToList();

        // Remove questions that are no longer in DTO
        foreach (var question in existingQuiz.Questions.ToList())
        {
            if (!requestDto.Questions.Any(q => q.Id == question.Id))
                await _questionRepository.DeleteAsync(question);
        }

        // Add or update questions
        foreach (var dto in requestDto.Questions)
        {
            if (dto.Id == 0)
            {
                var newQuestion = new QuestionModel
                {
                    QuestionText = dto.QuestionText,
                    QuizId = quizId
                };
                await _questionRepository.AddAsync(newQuestion);
            }
            else
            {
                var existingQuestion = existingQuiz.Questions.FirstOrDefault(q => q.Id == dto.Id);
                if (existingQuestion != null)
                {
                    existingQuestion.QuestionText = dto.QuestionText;
                    await _questionRepository.UpdateAsync(existingQuestion);
                }
            }
        }

        await _quizRepository.SaveChangesAsync();

        var updated = await _quizRepository.GetByIdWithDetailsAsync(quizId);
        return _mapper.Map<QuizDetailDto>(updated);
    }

    // DELETE QUIZ
    public async Task DeleteQuizAsync(int id, int currentUserId)
    {
        var quiz = await _quizRepository.GetByIdWithDetailsAsync(id);
        if (quiz == null)
            throw new KeyNotFoundException("Quiz not found");

        if (quiz.CreatedByUserId != currentUserId)
            throw new UnauthorizedAccessException("You are not authorized to delete this quiz");

        _quizRepository.DeleteAsync(quiz);
        await _quizRepository.SaveChangesAsync();
    }

    // GET ALL QUIZZES
    public async Task<List<QuizSummaryDto>> GetAllQuizzesAsync()
    {
        var quizzes = await _quizRepository.GetAllAsync();
        return _mapper.Map<List<QuizSummaryDto>>(quizzes);
    }

    // GET QUIZ BY QUIZ ID
    public async Task<QuizDetailDto?> GetQuizByIdAsync(int quizId)
    {
        var quiz = await _quizRepository.GetByIdWithDetailsAsync(quizId);
        return quiz == null ? null : _mapper.Map<QuizDetailDto>(quiz);
    }

    // GET QUIZZES BY USER ID
    public async Task<List<QuizSummaryDto>> GetQuizzesByUserIdAsync(int userId)
    {
        var quizzes = await _quizRepository.GetByUserIdAsync(userId);
        return _mapper.Map<List<QuizSummaryDto>>(quizzes);
    }

    // FIND QUIZZES BY NAME
    public async Task<List<QuizSummaryDto>> FindQuizzesByNameAsync(string query)
    {
        var quizzes = string.IsNullOrWhiteSpace(query) ? await _quizRepository.GetAllAsync() : await _quizRepository.FindByNameAsync(query);

        return _mapper.Map<List<QuizSummaryDto>>(quizzes);
    }
}

