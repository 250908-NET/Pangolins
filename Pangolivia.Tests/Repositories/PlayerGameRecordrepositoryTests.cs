 namespace Pangolivia.Tests.Repositories;

 public class PlayerGameRecordRepositoryTests : RepositoryTestBase
 {
     private readonly PlayerGameRecordRepository _repo;

     public PlayerGameRecordRepositoryTests()
     {
         _repo = new PlayerGameRecordRepository(_context);
     }

    //Helper method to seed user, quiz, and game record
    //Arrange
     private (int userId, int quizId, int gameRecordId) SeedUserQuizAndGame()
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

         var gameRecord = new GameRecordModel
         {
             HostUserId = user.Id,
             QuizId = quiz.Id,
             datetimeCompleted = DateTime.UtcNow
         };
         _context.GameRecords.Add(gameRecord);
         _context.SaveChanges();

         return (user.Id, quiz.Id, gameRecord.Id);
     }

     [Fact]
     public async Task GetAllPlayerGameRecords_WhenNone_ReturnsEmptyList()
     {
         //Act
         var result = await _repo.getAllPlayerGameRecords();

         //Assert
         result.Should().NotBeNull();
         result.Should().BeEmpty();
     }

     [Fact]
     public async Task GetAllPlayerGameRecords_WhenExists_ReturnsListWithIncludes()
     {
             var (userId, _, gameRecordId) = SeedUserQuizAndGame();
         var pgr = new PlayerGameRecordModel
         {
             UserId = userId,
             GameRecordId = gameRecordId,
             score = 10.5
         };
         _context.PlayerGameRecords.Add(pgr);
         _context.SaveChanges();

         //Act
         var result = await _repo.getAllPlayerGameRecords();

         //Assert
         result.Should().HaveCount(1);
         var saved = result.Single();
         saved.User.Should().NotBeNull();
         saved.GameRecord.Should().NotBeNull();
         saved.score.Should().Be(10.5);
     }

     [Fact]
     public async Task GetPlayerGameRecordModelByUserId_WhenMissing_ReturnsNull()
     {
         //Act
         var result = await _repo.getPlayerGameRecordModelByUserId(999);

         //Assert
         result.Should().BeNull();
     }

     [Fact]
     public async Task GetPlayerGameRecordModelByUserId_WhenExists_ReturnsRecord()
     {
         //Arrange
         var (userId, _, gameRecordId) = SeedUserQuizAndGame();
         var pgr = new PlayerGameRecordModel
         {
             UserId = userId,
             GameRecordId = gameRecordId,
             score = 7
         };
         _context.PlayerGameRecords.Add(pgr);
         _context.SaveChanges();

         //Act
         var result = await _repo.getPlayerGameRecordModelByUserId(userId);

         //Assert
         result.Should().NotBeNull();
         result!.UserId.Should().Be(userId);
         result.GameRecord.Should().NotBeNull();
     }

     [Fact]
     public async Task AddPlayerGameRecordModel_CreatesAndPersists()
     {
             var (userId, _, gameRecordId) = SeedUserQuizAndGame();

         var dto = new PlayerGameRecordDto
         {
             UserId = userId,
             GameRecordId = gameRecordId,
             Score = 42.3
         };

         //Act
         var created = await _repo.AddPlayerGameRecordModel(dto);

         //Assert
         created.Id.Should().BeGreaterThan(0);
         created.UserId.Should().Be(userId);
         created.GameRecordId.Should().Be(gameRecordId);
         created.score.Should().Be(42.3);

         var fromDb = await _context.PlayerGameRecords.FindAsync(created.Id);
         fromDb.Should().NotBeNull();
     }

     [Fact]
     public async Task RemovePlayerGameRecordModel_WhenNotFound_Throws()
     {
         //Act
         Func<Task> act = async () => await _repo.RemovePlayerGameRecordModel(12345);

         //Assert
         await act.Should().ThrowAsync<KeyNotFoundException>();
     }

     [Fact]
     public async Task RemovePlayerGameRecordModel_WhenExists_Removes()
     {
         //Arrange
         var (userId, _, gameRecordId) = SeedUserQuizAndGame();
         var pgr = new PlayerGameRecordModel
         {
             UserId = userId,
             GameRecordId = gameRecordId,
             score = 1
         };
         _context.PlayerGameRecords.Add(pgr);
         _context.SaveChanges();

         //Act
         await _repo.RemovePlayerGameRecordModel(pgr.Id);

         //Assert
         var all = _context.PlayerGameRecords.ToList();
         all.Should().BeEmpty();
     }

     [Fact]
     public async Task UpdatePlayerGameRecordModel_WhenNotFound_Throws()
     {
         //Act
         Func<Task> act = async () => await _repo.UpdatePlayerGameRecordModel(99999, 77);

         //Assert
         await act.Should().ThrowAsync<KeyNotFoundException>();
     }

     [Fact]
     public async Task UpdatePlayerGameRecordModel_WhenExists_UpdatesScore()
     {
         //Arrange
         var (userId, _, gameRecordId) = SeedUserQuizAndGame();
         var pgr = new PlayerGameRecordModel
         {
             UserId = userId,
             GameRecordId = gameRecordId,
             score = 5
         };
         _context.PlayerGameRecords.Add(pgr);
         _context.SaveChanges();

         //Act
         await _repo.UpdatePlayerGameRecordModel(pgr.Id, 99);

         //Assert
         var updated = await _context.PlayerGameRecords.FindAsync(pgr.Id);
         updated!.score.Should().Be(99);
     }
 }
