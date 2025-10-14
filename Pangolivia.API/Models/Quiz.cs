using System.Reflection.Metadata;

namespace Pangolivia.API.Models;

public class Quiz
{
    public int Id { get; set; }
    public string Name { get; set; }
    public User Creator { get; set; }
    public List<Question> Questions { get; set; }
}