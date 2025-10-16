 namespace Pangolivia.Tests.Repositories;

 public class QuizRepositoryTests : RepositoryTestBase
 {
     private readonly QuizRepository _repo;

     public QuizRepositoryTests()
     {
         _repo = new QuizRepository(_context, null!);
     }

     //Helper method to seed user
     //Arrange
     private int SeedUser()
     {
         var user = new UserModel
         {
             AuthUuid = Guid.NewGuid().ToString(),
             Username = $"user_{Guid.NewGuid():N}".Substring(0, 12)
         };
         _context.Users.Add(user);
         _context.SaveChanges();
         return user.Id;
     }

     [Fact]
     public async Task AddAsync_CreatesAndPersists()
     {
         //Arrange
         var userId = SeedUser();
         var quiz = new QuizModel { QuizName = "Sample Quiz", CreatedByUserId = userId };

         //Act
         var created = await _repo.AddAsync(quiz);

         created.Id.Should().BeGreaterThan(0);
         created.CreatedByUserId.Should().Be(userId);

         //Assert
         var fromDb = await _repo.GetByIdAsync(created.Id);
         fromDb.Should().NotBeNull();
     }

     [Fact]
     public async Task UpdateAsync_ThenSave_PersistsChange()
     {
         //Arrange
         var userId = SeedUser();
         var quiz = new QuizModel { QuizName = "Old Name", CreatedByUserId = userId };
         await _repo.AddAsync(quiz);

         //Act
         quiz.QuizName = "New Name";
         _repo.UpdateAsync(quiz);
         await _repo.SaveChangesAsync();

         //Assert
         var fromDb = await _repo.GetByIdAsync(quiz.Id);
         fromDb!.QuizName.Should().Be("New Name");
     }

     [Fact]
     public async Task DeleteAsync_ThenSave_Removes()
     {
         //Arrange
         var userId = SeedUser();
         var quiz = new QuizModel { QuizName = "To Delete", CreatedByUserId = userId };
         await _repo.AddAsync(quiz);

         //Act
         _repo.DeleteAsync(quiz);
         await _repo.SaveChangesAsync();

         //Assert
         var fromDb = await _repo.GetByIdAsync(quiz.Id);
         fromDb.Should().BeNull();
     }

     [Fact]
     public async Task GetByIdWithDetailsAsync_WhenNotFound_ReturnsNull()
     {
         //Act
         var result = await _repo.GetByIdWithDetailsAsync(9999);

         //Assert
         result.Should().BeNull();
     }

     [Fact]
     public async Task GetByIdWithDetailsAsync_ReturnsIncludes()
     {
         //Arrange
         var userId = SeedUser();
         var quiz = new QuizModel { QuizName = "With Details", CreatedByUserId = userId };
         await _repo.AddAsync(quiz);

         var q1 = new QuestionModel
         {
             QuizId = quiz.Id,
             QuestionText = "Q1",
             CorrectAnswer = "A",
             Answer2 = "B",
             Answer3 = "C",
             Answer4 = "D"
         };
         var q2 = new QuestionModel
         {
             QuizId = quiz.Id,
             QuestionText = "Q2",
             CorrectAnswer = "A",
             Answer2 = "B",
             Answer3 = "C",
             Answer4 = "D"
         };
         _context.Questions.AddRange(q1, q2);
         _context.SaveChanges();

         //Act
         var withDetails = await _repo.GetByIdWithDetailsAsync(quiz.Id);

         //Assert
         withDetails.Should().NotBeNull();
         withDetails!.CreatedByUser.Should().NotBeNull();
         withDetails.Questions.Should().HaveCount(2);
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
     public async Task GetByIdAsync_WhenFound_ReturnsQuiz()
     {
         //Arrange
         var userId = SeedUser();
         var quiz = new QuizModel { QuizName = "To Find", CreatedByUserId = userId };
         await _repo.AddAsync(quiz);

         //Act
         var result = await _repo.GetByIdAsync(quiz.Id);

         //Assert
         result.Should().NotBeNull();
         result!.QuizName.Should().Be("To Find");
     }

     [Fact]
     public async Task GetAllAsync_ReturnsAllWithIncludes()
     {
         //Arrange
         var userId = SeedUser();
         var quiz1 = new QuizModel { QuizName = "Quiz 1", CreatedByUserId = userId };
         var quiz2 = new QuizModel { QuizName = "Quiz 2", CreatedByUserId = userId };
         await _repo.AddAsync(quiz1);
         await _repo.AddAsync(quiz2);

         _context.Questions.Add(new QuestionModel
         {
             QuizId = quiz1.Id,
             QuestionText = "Only Q for 1",
             CorrectAnswer = "A",
             Answer2 = "B",
             Answer3 = "C",
             Answer4 = "D"
         });
         _context.SaveChanges();

         //Act
         var all = await _repo.GetAllAsync();

         //Assert
         all.Should().HaveCount(2);
         all.Any(x => x.CreatedByUser is null).Should().BeFalse();
     }

     [Fact]
     public async Task FindByNameAsync_FiltersBySubstring()
     {
         //Arrange
         var userId = SeedUser();
         await _repo.AddAsync(new QuizModel { QuizName = "Math Basics", CreatedByUserId = userId });
         await _repo.AddAsync(new QuizModel { QuizName = "Advanced Math", CreatedByUserId = userId });
         await _repo.AddAsync(new QuizModel { QuizName = "History", CreatedByUserId = userId });

         //Act
         var found = await _repo.FindByNameAsync("Math");

         //Assert
         found.Should().HaveCount(2);
         found.All(q => q.QuizName.Contains("Math")).Should().BeTrue();
     }

     [Fact]
     public async Task GetByUserIdAsync_FiltersByCreator()
     {
         //Arrange
         var userA = SeedUser();
         var userB = SeedUser();
         await _repo.AddAsync(new QuizModel { QuizName = "A1", CreatedByUserId = userA });
         await _repo.AddAsync(new QuizModel { QuizName = "A2", CreatedByUserId = userA });
         await _repo.AddAsync(new QuizModel { QuizName = "B1", CreatedByUserId = userB });

         //Act
         var forA = await _repo.GetByUserIdAsync(userA);

         //Assert
         forA.Should().HaveCount(2);
         forA.All(q => q.CreatedByUserId == userA).Should().BeTrue();
     }
 }
