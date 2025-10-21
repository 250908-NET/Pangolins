using Pangolivia.API.DTOs;
using Pangolivia.API.Models;

namespace Pangolivia.API.Services;

public interface IQuizService
{
    Task<QuizDetailDto> CreateQuizAsync(CreateQuizRequestDto requestDto, int creatorUserId);
    Task<QuizDetailDto> UpdateQuizAsync(
        int quizId,
        UpdateQuizRequestDto requestDto,
        int currentUserId
    );
    Task DeleteQuizAsync(int id, int currentUserId);
    Task<List<QuizSummaryDto>> GetAllQuizzesAsync();
    Task<QuizDetailDto?> GetQuizByIdAsync(int quizId);
    Task<List<QuizSummaryDto>> GetQuizzesByUserIdAsync(int userId);
    Task<List<QuizSummaryDto>> FindQuizzesByNameAsync(string query);
}
