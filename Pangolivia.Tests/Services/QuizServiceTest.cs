using Pangolivia.API.Services;
using Pangolivia.API.DTOs;

namespace Pangolivia.Tests.Services;

public class QuizServiceTest
{
    private readonly Mock<IQuizRepository> _quizRepoMock;
    private readonly Mock<IQuestionRepository> _questionRepoMock;
    private readonly Mock<AutoMapper.IMapper> _mapperMock;
    private readonly QuizService _service;

    public QuizServiceTest()
    {
        _quizRepoMock = new Mock<IQuizRepository>();
        _questionRepoMock = new Mock<IQuestionRepository>();
        _mapperMock = new Mock<AutoMapper.IMapper>();
        _service = new QuizService(_quizRepoMock.Object, _questionRepoMock.Object, _mapperMock.Object);
    }

    [Fact]
    public async Task CreateQuizAsync_AddsQuizAndQuestions_AndReturnsMappedDto()
    {
        // Arrange
        var request = new CreateQuizRequestDto
        {
            QuizName = "Sample",
            Questions = new List<QuestionDto>
            {
                new QuestionDto { QuestionText = "Q1", CorrectAnswer = "A", Answer2 = "B", Answer3 = "C", Answer4 = "D" }
            }
        };

        // Capture the quiz passed to AddAsync and assign Id
        _quizRepoMock
            .Setup(r => r.AddAsync(It.IsAny<QuizModel>()))
            .ReturnsAsync((QuizModel q) => { q.Id = 10; return q; });

        _questionRepoMock.Setup(r => r.AddAsync(It.IsAny<QuestionModel>())).Returns(Task.CompletedTask);
        _quizRepoMock.Setup(r => r.SaveChangesAsync()).ReturnsAsync(true);

        var detailedModel = new QuizModel
        {
            Id = 10,
            QuizName = request.QuizName,
            CreatedByUserId = 5,
            CreatedByUser = new UserModel { Id = 5, Username = "u" },
            Questions = new List<QuestionModel>
            {
                new QuestionModel { Id = 1, QuestionText = "Q1", QuizId = 10, CorrectAnswer = "A", Answer2 = "B", Answer3 = "C", Answer4 = "D" }
            }
        };

        _quizRepoMock.Setup(r => r.GetByIdWithDetailsAsync(10)).ReturnsAsync(detailedModel);

        var mappedDto = new QuizDetailDto { Id = 10, QuizName = "Sample", CreatedByUserId = 5, CreatorUsername = "u" };
        _mapperMock.Setup(m => m.Map<QuizDetailDto>(It.IsAny<QuizModel>())).Returns(mappedDto);

        // Act
        var result = await _service.CreateQuizAsync(request, creatorUserId: 5);

        // Assert
        result.Should().BeEquivalentTo(mappedDto);
        _quizRepoMock.Verify(r => r.AddAsync(It.IsAny<QuizModel>()), Times.Once);
        _questionRepoMock.Verify(r => r.AddAsync(It.IsAny<QuestionModel>()), Times.Exactly(request.Questions.Count));
        _quizRepoMock.Verify(r => r.SaveChangesAsync(), Times.Once);
        _quizRepoMock.Verify(r => r.GetByIdWithDetailsAsync(10), Times.Once);
    }

    [Fact]
    public async Task UpdateQuizAsync_WhenQuizMissing_ThrowsKeyNotFoundException()
    {
        _quizRepoMock.Setup(r => r.GetByIdWithDetailsAsync(42)).ReturnsAsync((QuizModel?)null);

        var dto = new UpdateQuizRequestDto { QuizName = "X", Questions = new List<QuestionDto>() };

        await FluentActions.Invoking(() => _service.UpdateQuizAsync(42, dto, currentUserId: 1))
            .Should().ThrowAsync<KeyNotFoundException>();
    }

    [Fact]
    public async Task UpdateQuizAsync_WhenNotCreator_ThrowsUnauthorizedAccessException()
    {
        var existing = new QuizModel { Id = 2, QuizName = "Old", CreatedByUserId = 9, Questions = new List<QuestionModel>() };
        _quizRepoMock.Setup(r => r.GetByIdWithDetailsAsync(2)).ReturnsAsync(existing);

        var dto = new UpdateQuizRequestDto { QuizName = "New", Questions = new List<QuestionDto>() };

        await FluentActions.Invoking(() => _service.UpdateQuizAsync(2, dto, currentUserId: 1))
            .Should().ThrowAsync<UnauthorizedAccessException>();
    }

    [Fact]
    public async Task UpdateQuizAsync_UpdatesQuestions_AddsUpdatesAndDeletes_AsExpected()
    {
        // Arrange existing quiz with two questions
        var existing = new QuizModel
        {
            Id = 3,
            QuizName = "Old",
            CreatedByUserId = 1,
            Questions = new List<QuestionModel>
            {
                new QuestionModel { Id = 100, QuestionText = "Qold1", QuizId = 3, CorrectAnswer = "A", Answer2="B", Answer3="C", Answer4="D" },
                new QuestionModel { Id = 200, QuestionText = "Qold2", QuizId = 3, CorrectAnswer = "A2", Answer2="B2", Answer3="C2", Answer4="D2" }
            }
        };

        _quizRepoMock.Setup(r => r.GetByIdWithDetailsAsync(3)).ReturnsAsync(existing);
        _quizRepoMock.Setup(r => r.SaveChangesAsync()).ReturnsAsync(true);

        // Capture arguments passed to question repo methods so assertions are reliable
        var capturedAdds = new List<QuestionModel>();
        var capturedUpdates = new List<QuestionModel>();
        var capturedDeletes = new List<QuestionModel>();

        _questionRepoMock
            .Setup(r => r.AddAsync(It.IsAny<QuestionModel>()))
            .Callback<QuestionModel>(q => capturedAdds.Add(q))
            .Returns(Task.CompletedTask);

        _questionRepoMock
            .Setup(r => r.UpdateAsync(It.IsAny<QuestionModel>()))
            .Callback<QuestionModel>(q => capturedUpdates.Add(q))
            .Returns(Task.CompletedTask);

        _questionRepoMock
            .Setup(r => r.DeleteAsync(It.IsAny<QuestionModel>()))
            .Callback<QuestionModel>(q => capturedDeletes.Add(q))
            .Returns(Task.CompletedTask);

        // DTO: keep question 100 (update text), remove 200, add new (Id = 0)
        var dto = new UpdateQuizRequestDto
        {
            QuizName = "New",
            Questions = new List<QuestionDto>
            {
                    new QuestionDto { Id = 100, QuestionText = "Qold1-updated", CorrectAnswer = "X", Answer2 = "Y", Answer3 = "Z", Answer4 = "W" },
                new QuestionDto { Id = 0, QuestionText = "NewQ", CorrectAnswer = "N3", Answer2 = "N1", Answer3 = "N2", Answer4 = "N4" }
            }
        };

        var mapped = new QuizDetailDto { Id = 3, QuizName = "New", CreatedByUserId = 1, CreatorUsername = "u" };
        _mapperMock.Setup(m => m.Map<QuizDetailDto>(It.IsAny<QuizModel>())).Returns(mapped);

        // Act
        var result = await _service.UpdateQuizAsync(3, dto, currentUserId: 1);

        // Assert result mapping
        result.Should().BeEquivalentTo(mapped);

        // Assert repo interactions via captured arguments
        capturedDeletes.Should().HaveCount(1);
        capturedDeletes.Single().Id.Should().Be(200);

        capturedUpdates.Should().HaveCount(1);
        capturedUpdates.Single().Id.Should().Be(100);
        capturedUpdates.Single().QuestionText.Should().Be("Qold1-updated");

        capturedAdds.Should().HaveCount(1);
        capturedAdds.Single().QuestionText.Should().Be("NewQ");

        _quizRepoMock.Verify(r => r.SaveChangesAsync(), Times.Once);

        // initial GetByIdWithDetailsAsync + final GetByIdWithDetailsAsync
        _quizRepoMock.Verify(r => r.GetByIdWithDetailsAsync(3), Times.Exactly(2));
    }

    [Fact]
    public async Task DeleteQuizAsync_WhenMissing_ThrowsKeyNotFoundException()
    {
        _quizRepoMock.Setup(r => r.GetByIdWithDetailsAsync(99)).ReturnsAsync((QuizModel?)null);

        await FluentActions.Invoking(() => _service.DeleteQuizAsync(99, currentUserId: 1))
            .Should().ThrowAsync<KeyNotFoundException>();
    }

    [Fact]
    public async Task DeleteQuizAsync_WhenNotCreator_ThrowsUnauthorizedAccessException()
    {
        var q = new QuizModel { Id = 5, CreatedByUserId = 10 };
        _quizRepoMock.Setup(r => r.GetByIdWithDetailsAsync(5)).ReturnsAsync(q);

        await FluentActions.Invoking(() => _service.DeleteQuizAsync(5, currentUserId: 1))
            .Should().ThrowAsync<UnauthorizedAccessException>();
    }

    [Fact]
    public async Task DeleteQuizAsync_DeletesAndSaves_WhenAuthorized()
    {
        var q = new QuizModel { Id = 6, CreatedByUserId = 2 };
        _quizRepoMock.Setup(r => r.GetByIdWithDetailsAsync(6)).ReturnsAsync(q);
        _quizRepoMock.Setup(r => r.SaveChangesAsync()).ReturnsAsync(true);

        _quizRepoMock.Setup(r => r.DeleteAsync(q));

        await _service.DeleteQuizAsync(6, currentUserId: 2);

        _quizRepoMock.Verify(r => r.DeleteAsync(q), Times.Once);
        _quizRepoMock.Verify(r => r.SaveChangesAsync(), Times.Once);
    }

    [Fact]
    public async Task GetAllQuizzesAsync_MapsAndReturnsSummaries()
    {
        var list = new List<QuizModel>
        {
            new QuizModel { Id = 1, QuizName = "A", CreatedByUserId = 1, CreatedByUser = new UserModel{Id=1, Username="u"} },
            new QuizModel { Id = 2, QuizName = "B", CreatedByUserId = 1, CreatedByUser = new UserModel{Id=1, Username="u"} }
        };

        _quizRepoMock.Setup(r => r.GetAllAsync()).ReturnsAsync(list);

        var mapped = new List<QuizSummaryDto>
        {
            new QuizSummaryDto { Id = 1, QuizName = "A", CreatorUsername = "u", QuestionCount = 0 },
            new QuizSummaryDto { Id = 2, QuizName = "B", CreatorUsername = "u", QuestionCount = 0 },
        };

        _mapperMock.Setup(m => m.Map<List<QuizSummaryDto>>(It.IsAny<List<QuizModel>>())).Returns(mapped);

        var result = await _service.GetAllQuizzesAsync();

        result.Should().BeEquivalentTo(mapped);
        _quizRepoMock.Verify(r => r.GetAllAsync(), Times.Once);
    }

    [Fact]
    public async Task GetQuizByIdAsync_ReturnsNullWhenNotFound_OrMappedWhenFound()
    {
        _quizRepoMock.Setup(r => r.GetByIdWithDetailsAsync(500)).ReturnsAsync((QuizModel?)null);
        var nullResult = await _service.GetQuizByIdAsync(500);
        nullResult.Should().BeNull();

        var model = new QuizModel { Id = 501, QuizName = "Found", CreatedByUserId = 1, CreatedByUser = new UserModel { Id = 1, Username = "u" } };
        _quizRepoMock.Setup(r => r.GetByIdWithDetailsAsync(501)).ReturnsAsync(model);
        var dto = new QuizDetailDto { Id = 501, QuizName = "Found", CreatedByUserId = 1, CreatorUsername = "u" };
        _mapperMock.Setup(m => m.Map<QuizDetailDto>(model)).Returns(dto);

        var found = await _service.GetQuizByIdAsync(501);
        found.Should().BeEquivalentTo(dto);
    }

    [Fact]
    public async Task GetQuizzesByUserIdAsync_And_FindQuizzesByNameAsync_Behavior()
    {
        var byUser = new List<QuizModel> { new QuizModel { Id = 1, QuizName = "UQ", CreatedByUserId = 7, CreatedByUser = new UserModel { Id = 7, Username = "u7" } } };
        _quizRepoMock.Setup(r => r.GetByUserIdAsync(7)).ReturnsAsync(byUser);
        var mappedUser = new List<QuizSummaryDto> { new QuizSummaryDto { Id = 1, QuizName = "UQ", CreatorUsername = "u7", QuestionCount = 0 } };
        _mapperMock.Setup(m => m.Map<List<QuizSummaryDto>>(It.IsAny<List<QuizModel>>())).Returns(mappedUser);

        var res = await _service.GetQuizzesByUserIdAsync(7);
        res.Should().BeEquivalentTo(mappedUser);

        // Find by empty query -> should call GetAllAsync
        var all = new List<QuizModel>();
        _quizRepoMock.Setup(r => r.GetAllAsync()).ReturnsAsync(all);
        var mappedAll = new List<QuizSummaryDto>();
        _mapperMock.Setup(m => m.Map<List<QuizSummaryDto>>(all)).Returns(mappedAll);

        var foundEmpty = await _service.FindQuizzesByNameAsync("   ");
        foundEmpty.Should().BeEquivalentTo(mappedAll);

        // Find by query -> should call FindByNameAsync
        var foundList = new List<QuizModel> { new QuizModel { Id = 9, QuizName = "SearchMatch", CreatedByUser = new UserModel { Id = 1, Username = "u" } } };
        _quizRepoMock.Setup(r => r.FindByNameAsync("term")).ReturnsAsync(foundList);
        var mappedFound = new List<QuizSummaryDto> { new QuizSummaryDto { Id = 9, QuizName = "SearchMatch", CreatorUsername = "u", QuestionCount = 0 } };
        _mapperMock.Setup(m => m.Map<List<QuizSummaryDto>>(foundList)).Returns(mappedFound);

        var found = await _service.FindQuizzesByNameAsync("term");
        found.Should().BeEquivalentTo(mappedFound);
    }
}
