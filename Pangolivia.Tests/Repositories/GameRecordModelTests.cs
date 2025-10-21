namespace Pangolivia.Tests.Repositories;

public class GameRecordModelTests : RepositoryTestBase
{
    private readonly GameRecordRepository _repo;
    private readonly ILogger<GameRecordRepository> _repoLogger;

    public GameRecordModelTests()
    {
        _repoLogger = new Mock<ILogger<GameRecordRepository>>().Object;
        _repo = new GameRecordRepository(_context, _repoLogger);
    }

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
    public async Task GetAllGameRecordsAsync_WhenNoGameRecordsExist_ReturnsEmptyList()
    {
        //Arrange

        //Act
        var result = await _repo.GetAllGameRecordsAsync();

        //Assert
        result.Should().NotBeNull();
        result.Should().BeEmpty();
    }

    [Fact]
    public async Task GetAllGameRecordsAsync_WhenGameRecordsExist_ReturnsList()
    {
        //Arrange
        var (userId, quizId) = SeedUserAndQuiz();
        var gameRecord = new GameRecordModel
        {
            HostUserId = userId,
            QuizId = quizId,
            datetimeCompleted = DateTime.UtcNow
        };
        _context.GameRecords.Add(gameRecord);
        _context.SaveChanges();

        //Act
        var result = await _repo.GetAllGameRecordsAsync();

        //Assert
        result.Should().NotBeEmpty();
        result.Should().HaveCount(1);
        var saved = result.Single();
        saved.HostUserId.Should().Be(gameRecord.HostUserId);
        saved.QuizId.Should().Be(gameRecord.QuizId);
        saved.datetimeCompleted.Should().BeCloseTo(gameRecord.datetimeCompleted, TimeSpan.FromSeconds(1));
    }

    [Fact]
    public async Task GetGameRecordByIdAsync_WhenNotFound_ReturnsNull()
    {
        //Act
        var result = await _repo.GetGameRecordByIdAsync(999);

        //Assert
        result.Should().BeNull();
    }

    [Fact]
    public async Task GetGameRecordByIdAsync_WhenExists_ReturnsRecord()
    {
        //Arrange
        var (userId, quizId) = SeedUserAndQuiz();
        var gameRecord = new GameRecordModel
        {
            HostUserId = userId,
            QuizId = quizId,
            datetimeCompleted = DateTime.UtcNow
        };
        _context.GameRecords.Add(gameRecord);
        _context.SaveChanges();

        //Act
        var result = await _repo.GetGameRecordByIdAsync(gameRecord.Id);

        //Assert
        result.Should().NotBeNull();
        result!.Id.Should().Be(gameRecord.Id);
        result.HostUserId.Should().Be(gameRecord.HostUserId);
        result.QuizId.Should().Be(gameRecord.QuizId);
    }

    [Fact]
    public async Task CreateGameRecordAsync_SetsCompletedTimeAndPersists()
    {
        //Arrange
        var (userId, quizId) = SeedUserAndQuiz();
        var toCreate = new GameRecordModel
        {
            HostUserId = userId,
            QuizId = quizId,
            datetimeCompleted = DateTime.UtcNow
        };

        //Act
        var created = await _repo.CreateGameRecordAsync(toCreate);

        //Assert
        created.Id.Should().BeGreaterThan(0);
        created.HostUserId.Should().Be(userId);
        created.QuizId.Should().Be(quizId);
        created.datetimeCompleted.Should().BeOnOrAfter(DateTime.UtcNow.AddSeconds(-1));
        created.datetimeCompleted.Should().BeOnOrBefore(DateTime.UtcNow.AddSeconds(1));

        var fromDb = await _repo.GetGameRecordByIdAsync(created.Id);
        fromDb.Should().NotBeNull();
    }

    [Fact]
    public async Task DeleteGameRecordAsync_WhenExists_RemovesAndReturnsTrue()
    {
        //Arrange
        var (userId, quizId) = SeedUserAndQuiz();
        var gameRecord = new GameRecordModel
        {
            HostUserId = userId,
            QuizId = quizId,
            datetimeCompleted = DateTime.UtcNow
        };
        _context.GameRecords.Add(gameRecord);
        _context.SaveChanges();

        //Act
        var deleted = await _repo.DeleteGameRecordAsync(gameRecord.Id);

        //Assert
        deleted.Should().BeTrue();
        var all = await _repo.GetAllGameRecordsAsync();
        all.Should().BeEmpty();
    }

    [Fact]
    public async Task DeleteGameRecordAsync_WhenMissing_ReturnsFalse()
    {
        //Act
        var deleted = await _repo.DeleteGameRecordAsync(12345);

        //Assert
        deleted.Should().BeFalse();
    }
}