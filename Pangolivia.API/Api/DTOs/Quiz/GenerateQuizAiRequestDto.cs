using System.Collections.Generic;

namespace Pangolivia.API.DTOs;

public class GenerateQuizAiRequestDto
{
    public string Topic { get; set; } = string.Empty;
    public int NumberOfQuestions { get; set; } = 5;
    public string Difficulty { get; set; } = "medium";
}
