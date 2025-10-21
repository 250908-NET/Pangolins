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
            dateTimeCompleted = DateTime.UtcNow
        };
        _context.GameRecords.Add(gameRecord);
        _context.SaveChanges();

        return (user.Id, quiz.Id, gameRecord.Id);
    }

    [Fact]
    public async Task GetAllAsync_WhenNone_ReturnsEmptyList() // Renamed to match repo
    {
        //Act
        var result = await _repo.GetAllAsync(); // Call actual repo method

        //Assert
        result.Should().NotBeNull();
        result.Should().BeEmpty();
    }

    [Fact]
    public async Task GetAllAsync_WhenExists_ReturnsListWithIncludes() // Renamed to match repo
    {
        var (userId, _, gameRecordId) = SeedUserQuizAndGame();
        var pgr = new PlayerGameRecordModel
        {
            UserId = userId,
            GameRecordId = gameRecordId,
            Score = 10.5
        };
        _context.PlayerGameRecords.Add(pgr);
        _context.SaveChanges();

        //Act
        var result = await _repo.GetAllAsync(); // Call actual repo method

        //Assert
        result.Should().HaveCount(1);
        var saved = result.Single();
        saved.User.Should().NotBeNull();
        saved.GameRecord.Should().NotBeNull();
        saved.Score.Should().Be(10.5); // Assuming 'score' property name is correct
    }

    [Fact]
    public async Task GetByUserIdAsync_WhenMissing_ReturnsEmptyList() // Renamed and adjusted expectation
    {
        //Act
        var result = await _repo.GetByUserIdAsync(999); // Call actual repo method

        //Assert
        result.Should().NotBeNull(); // It will return an empty enumerable, not null
        result.Should().BeEmpty();
    }

    [Fact]
    public async Task GetByUserIdAsync_WhenExists_ReturnsRecord() // Renamed and adjusted expectation
    {
        //Arrange
        var (userId, _, gameRecordId) = SeedUserQuizAndGame();
        var pgr = new PlayerGameRecordModel
        {
            UserId = userId,
            GameRecordId = gameRecordId,
            Score = 7
        };
        _context.PlayerGameRecords.Add(pgr);
        _context.SaveChanges();

        //Act
        var result = await _repo.GetByUserIdAsync(userId); // Call actual repo method

        //Assert
        result.Should().NotBeNull();
        result.Should().HaveCount(1);
        var saved = result.Single();
        saved.UserId.Should().Be(userId);
        saved.GameRecord.Should().NotBeNull();
    }

    [Fact]
    public async Task AddAsync_CreatesAndPersists() // Renamed to match repo
    {
        var (userId, _, gameRecordId) = SeedUserQuizAndGame();

        // DTO to Model mapping logic would typically be in a service layer,
        // or you instantiate the model directly for repository tests.
        var newRecord = new PlayerGameRecordModel
        {
            UserId = userId,
            GameRecordId = gameRecordId,
            Score = 42.3 // Assuming 'score' property name is correct
        };

        //Act
        var created = await _repo.AddAsync(newRecord); // Call actual repo method

        //Assert
        created.Id.Should().BeGreaterThan(0);
        created.UserId.Should().Be(userId);
        created.GameRecordId.Should().Be(gameRecordId);
        created.Score.Should().Be(42.3);

        var fromDb = await _context.PlayerGameRecords.FindAsync(created.Id);
        fromDb.Should().NotBeNull();
        fromDb!.Score.Should().Be(42.3); // Verify from DB
    }

    [Fact]
    public async Task DeleteAsync_WhenNotFound_ReturnsFalse() // Renamed and adjusted expectation
    {
        //Act
        var result = await _repo.DeleteAsync(12345); // Call actual repo method

        //Assert
        result.Should().BeFalse();
    }

    [Fact]
    public async Task DeleteAsync_WhenExists_Removes() // Renamed to match repo
    {
        //Arrange
        var (userId, _, gameRecordId) = SeedUserQuizAndGame();
        var pgr = new PlayerGameRecordModel
        {
            UserId = userId,
            GameRecordId = gameRecordId,
            Score = 1
        };
        _context.PlayerGameRecords.Add(pgr);
        _context.SaveChanges();

        //Act
        var result = await _repo.DeleteAsync(pgr.Id); // Call actual repo method

        //Assert
        result.Should().BeTrue(); // Verify deletion was successful
        var all = _context.PlayerGameRecords.ToList();
        all.Should().BeEmpty();
    }

    // Test for GetByIdAsync when not found
    [Fact]
    public async Task GetByIdAsync_WhenMissing_ReturnsNull()
    {
        var result = await _repo.GetByIdAsync(999);
        result.Should().BeNull();
    }

    // Test for GetByIdAsync when exists
    [Fact]
    public async Task GetByIdAsync_WhenExists_ReturnsRecordWithIncludes()
    {
        var (userId, _, gameRecordId) = SeedUserQuizAndGame();
        var pgr = new PlayerGameRecordModel
        {
            UserId = userId,
            GameRecordId = gameRecordId,
            Score = 25
        };
        _context.PlayerGameRecords.Add(pgr);
        _context.SaveChanges();

        var result = await _repo.GetByIdAsync(pgr.Id);

        result.Should().NotBeNull();
        result!.Id.Should().Be(pgr.Id);
        result.User.Should().NotBeNull();
        result.GameRecord.Should().NotBeNull();
        result.Score.Should().Be(25);
    }



    [Fact]
    public async Task UpdateAsync_WhenExists_UpdatesRecord() // Renamed to match repo, assuming it updates a whole model
    {
        //Arrange
        var (userId, _, gameRecordId) = SeedUserQuizAndGame();
        var pgr = new PlayerGameRecordModel
        {
            UserId = userId,
            GameRecordId = gameRecordId,
            Score = 5
        };
        _context.PlayerGameRecords.Add(pgr);
        _context.SaveChanges();

        // Modify the existing model
        pgr.Score = 99;
        // pgr.SomeOtherProperty = "New Value"; // If you update other properties

        //Act
        var updatedRecord = await _repo.UpdateAsync(pgr); // Pass the modified model

        //Assert
        updatedRecord.Should().NotBeNull();
        updatedRecord.Id.Should().Be(pgr.Id);
        updatedRecord.Score.Should().Be(99);

        var fromDb = await _context.PlayerGameRecords.FirstOrDefaultAsync(x => x.Id == pgr.Id);
        fromDb.Should().NotBeNull();
        fromDb!.Score.Should().Be(99);
    }
}
