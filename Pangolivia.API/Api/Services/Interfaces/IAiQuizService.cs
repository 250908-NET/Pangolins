using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using Pangolivia.API.DTOs;

namespace Pangolivia.API.Services;

public interface IAiQuizService
{
    Task<List<QuestionDto>> GenerateQuestionsAsync(string topic, int numberOfQuestions, string difficulty, CancellationToken cancellationToken = default);
}
