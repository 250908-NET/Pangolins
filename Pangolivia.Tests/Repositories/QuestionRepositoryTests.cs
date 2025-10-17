 namespace Pangolivia.Tests.Repositories;

 public class QuestionRepositoryTests : RepositoryTestBase
 {
     private readonly QuestionRepository _repo;

     public QuestionRepositoryTests()
     {
         _repo = new QuestionRepository(_context);
     }

     //Helper method to seed a user and quiz
     //Arrange
     private (int userId, int quizId) SeedUserAndQuiz()
     {
         var user = new UserModel
         {
             AuthUuid = Guid.NewGuid().ToString(),
             Username = $"user_{Guid.NewGuid():N}".Substring(0, 12)
         };
         _context.Users.Add(user);
         _context.SaveChanges();

         var quiz = new QuizModel
         {
             QuizName = "Sample Quiz",
             CreatedByUserId = user.Id
         };
         _context.Quizzes.Add(quiz);
         _context.SaveChanges();

         return (user.Id, quiz.Id);
     }

     [Fact]
     public async Task AddAsync_ThenSave_Persists()
     {
         //Arrange
         var (_, quizId) = SeedUserAndQuiz();
         var q = new QuestionModel
         {
             QuizId = quizId,
             QuestionText = "What is 2+2?",
             CorrectAnswer = "4",
             Answer2 = "3",
             Answer3 = "5",
             Answer4 = "22"
         };

         //Act
         await _repo.AddAsync(q);
         await _repo.SaveChangesAsync();

         //Assert
         q.Id.Should().BeGreaterThan(0);
         var fromDb = await _repo.GetByIdAsync(q.Id);
         fromDb.Should().NotBeNull();
         fromDb!.QuestionText.Should().Be("What is 2+2?");
     }

     [Fact]
     public async Task UpdateAsync_ThenSave_PersistsChange()
     {
         //Arrange
         var (_, quizId) = SeedUserAndQuiz();
         var q = new QuestionModel
         {
             QuizId = quizId,
             QuestionText = "Old",
             CorrectAnswer = "A",
             Answer2 = "B",
             Answer3 = "C",
             Answer4 = "D"
         };
         await _repo.AddAsync(q);
         await _repo.SaveChangesAsync();

         q.QuestionText = "New";

         //Act
         await _repo.UpdateAsync(q);
         await _repo.SaveChangesAsync();

         //Assert
         var fromDb = await _repo.GetByIdAsync(q.Id);
         fromDb!.QuestionText.Should().Be("New");
     }

     [Fact]
     public async Task DeleteAsync_ThenSave_Removes()
     {
         //Arrange
         var (_, quizId) = SeedUserAndQuiz();
         var q = new QuestionModel
         {
             QuizId = quizId,
             QuestionText = "To be deleted",
             CorrectAnswer = "A",
             Answer2 = "B",
             Answer3 = "C",
             Answer4 = "D"
         };
         await _repo.AddAsync(q);
         await _repo.SaveChangesAsync();

         //Act
         await _repo.DeleteAsync(q);
         await _repo.SaveChangesAsync();

         //Assert
         var fromDb = await _repo.GetByIdAsync(q.Id);
         fromDb.Should().BeNull();
     }

     [Fact]
     public async Task GetByIdAsync_WhenNotFound_ReturnsNull()
     {
         //Act
         var result = await _repo.GetByIdAsync(9999);

         //Assert
         result.Should().BeNull();
     }

     [Fact]
     public async Task GetByIdAsync_WhenFound_ReturnsQuestion()
     {
         //Arrange
         var (_, quizId) = SeedUserAndQuiz();
         var q = new QuestionModel
         {
             QuizId = quizId,
             QuestionText = "What is 2+10?",
             CorrectAnswer = "12",
             Answer2 = "3",
             Answer3 = "5",
             Answer4 = "52"
         };
         await _repo.AddAsync(q);
         await _repo.SaveChangesAsync();

         //Act
         var result = await _repo.GetByIdAsync(q.Id);

         //Assert
         result.Should().NotBeNull();
         result!.QuestionText.Should().Be("What is 2+10?");
     }
 }
