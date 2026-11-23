using Kontursvet.Application.Abstractions;
using Kontursvet.Application.Dtos;
using Kontursvet.Application.Features.Portfolio.CardView;
using Kontursvet.Domain.Entities;
using Moq;

namespace Kontursvet.Application.Tests.Features.Portfolio.CardView;

/// <summary>
/// Тесты обработчиков представлений карточек (CardView): валидация полей, upsert через
/// проверку существования карточки, 404, удаление.
/// </summary>
public class PortfolioCardViewHandlerTests
{
    private static PortfolioCardViewDto ValidDto() => new()
    {
        Name = "Вилла Северная",
        Part = "Фасад",
        Title = "Архитектурная подсветка",
        Description = "Описание проекта",
        Task = "Подчеркнуть контур здания",
        Works = ["проектирование", "монтаж"],
        Location = "Выборг",
        Term = "6 недель",
        Team = "3 человека",
        Period = "2026",
        Features = "DMX-управление",
        Meta = ["фасад", "экстерьер"],
        Gallery = [new GalleryItemDto { Key = 0, Src = "/uploads/0.jpg", Alt = "Ракурс 1" }]
    };

    // ==================== GetById ====================

    [Fact]
    public async Task GetById_Exists_ReturnsMappedDto()
    {
        var view = new PortfolioCardView
        {
            Id = 1,
            Name = "N",
            Part = "P",
            Title = "T",
            Works = ["w1", "w2"],
            Meta = ["m1"],
            Gallery = [new GalleryItem(0, "/uploads/0.jpg", "alt", "подпись")],
            UpdatedAt = new DateTime(2026, 1, 2, 3, 4, 5, DateTimeKind.Utc)
        };

        var repo = new Mock<IPortfolioRepository>();
        repo.Setup(r => r.GetViewByIdAsync(1, It.IsAny<CancellationToken>())).ReturnsAsync(view);

        var handler = new GetPortfolioCardViewHandler(repo.Object);
        var result = await handler.HandleAsync(new GetPortfolioCardViewQuery(1), CancellationToken.None);

        Assert.True(result.IsSuccess);
        Assert.Equal(view.Name, result.Value!.Name);
        Assert.Equal(view.Works, result.Value!.Works);
        Assert.Equal(view.Meta, result.Value!.Meta);
        Assert.Single(result.Value!.Gallery);
        Assert.Equal("подпись", result.Value!.Gallery[0].Figcaption);
        Assert.Equal(view.UpdatedAt, result.Value!.UpdatedAt);
    }

    [Fact]
    public async Task GetById_NotFound_ReturnsNotFound()
    {
        var repo = new Mock<IPortfolioRepository>();
        repo.Setup(r => r.GetViewByIdAsync(It.IsAny<long>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync((PortfolioCardView?)null);

        var handler = new GetPortfolioCardViewHandler(repo.Object);
        var result = await handler.HandleAsync(new GetPortfolioCardViewQuery(99), CancellationToken.None);

        Assert.False(result.IsSuccess);
        Assert.Equal("CARD_NOT_FOUND", result.ErrorCode);
    }

    // ==================== GetAll ====================

    [Theory]
    [InlineData(-5, 0, 0, 1)]
    [InlineData(0, 1000, 0, 200)]
    [InlineData(10, 20, 10, 20)]
    public async Task GetAll_NormalizesSkipAndTake(int skip, int take, int expSkip, int expTake)
    {
        var repo = new Mock<IPortfolioRepository>();
        repo.Setup(r => r.GetAllViewsAsync(It.IsAny<int>(), It.IsAny<int>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(new List<PortfolioCardView>());

        var handler = new GetPortfolioCardViewsHandler(repo.Object);
        await handler.HandleAsync(new GetPortfolioCardViewsQuery(skip, take), CancellationToken.None);

        repo.Verify(r => r.GetAllViewsAsync(expSkip, expTake, It.IsAny<CancellationToken>()), Times.Once);
    }

    // ==================== Update (upsert) ====================

    [Fact]
    public async Task Update_EmptyName_ReturnsNameRequired()
    {
        var dto = ValidDto();
        dto.Name = "";

        var handler = new UpdatePortfolioCardViewHandler(new Mock<IPortfolioRepository>().Object);
        var result = await handler.HandleAsync(new UpdatePortfolioCardViewCommand(1, dto), CancellationToken.None);

        Assert.False(result.IsSuccess);
        Assert.Equal("CARD_NAME_REQUIRED", result.ErrorCode);
    }

    [Fact]
    public async Task Update_EmptyPart_ReturnsPartRequired()
    {
        var dto = ValidDto();
        dto.Part = "  ";

        var handler = new UpdatePortfolioCardViewHandler(new Mock<IPortfolioRepository>().Object);
        var result = await handler.HandleAsync(new UpdatePortfolioCardViewCommand(1, dto), CancellationToken.None);

        Assert.False(result.IsSuccess);
        Assert.Equal("CARD_PART_REQUIRED", result.ErrorCode);
    }

    [Fact]
    public async Task Update_EmptyTitle_ReturnsTitleRequired()
    {
        var dto = ValidDto();
        dto.Title = "";

        var handler = new UpdatePortfolioCardViewHandler(new Mock<IPortfolioRepository>().Object);
        var result = await handler.HandleAsync(new UpdatePortfolioCardViewCommand(1, dto), CancellationToken.None);

        Assert.False(result.IsSuccess);
        Assert.Equal("CARD_TITLE_REQUIRED", result.ErrorCode);
    }

    [Fact]
    public async Task Update_CardNotFound_ReturnsNotFound()
    {
        // Для upsert представления обязательно существование КАРТОЧКИ с этим id
        var repo = new Mock<IPortfolioRepository>();
        repo.Setup(r => r.GetCardByIdAsync(7, It.IsAny<CancellationToken>()))
            .ReturnsAsync((PortfolioCard?)null);

        var handler = new UpdatePortfolioCardViewHandler(repo.Object);
        var result = await handler.HandleAsync(new UpdatePortfolioCardViewCommand(7, ValidDto()), CancellationToken.None);

        Assert.False(result.IsSuccess);
        Assert.Equal("CARD_NOT_FOUND", result.ErrorCode);
        repo.Verify(r => r.UpsertViewAsync(It.IsAny<PortfolioCardView>(), It.IsAny<CancellationToken>()), Times.Never);
    }

    [Fact]
    public async Task Update_CardExists_ReturnsSuccess_AndUsesRouteId()
    {
        var repo = new Mock<IPortfolioRepository>();
        repo.Setup(r => r.GetCardByIdAsync(7, It.IsAny<CancellationToken>()))
            .ReturnsAsync(new PortfolioCard { Id = 7 });

        PortfolioCardView? upserted = null;
        repo.Setup(r => r.UpsertViewAsync(It.IsAny<PortfolioCardView>(), It.IsAny<CancellationToken>()))
            .Callback<PortfolioCardView, CancellationToken>((v, _) => upserted = v)
            .ReturnsAsync(true);

        var dto = ValidDto();
        dto.Id = 111; // id в теле перезаписывается id из маршрута

        var handler = new UpdatePortfolioCardViewHandler(repo.Object);
        var result = await handler.HandleAsync(new UpdatePortfolioCardViewCommand(7, dto), CancellationToken.None);

        Assert.True(result.IsSuccess);
        Assert.NotNull(upserted);
        Assert.Equal(7L, upserted.Id);
        Assert.Equal(dto.Name, upserted.Name);
        Assert.Single(upserted.Gallery);
    }

    [Fact]
    public async Task Update_UpsertReturnsFalse_ReturnsUpdateFailed()
    {
        var repo = new Mock<IPortfolioRepository>();
        repo.Setup(r => r.GetCardByIdAsync(It.IsAny<long>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(new PortfolioCard { Id = 1 });
        repo.Setup(r => r.UpsertViewAsync(It.IsAny<PortfolioCardView>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(false);

        var handler = new UpdatePortfolioCardViewHandler(repo.Object);
        var result = await handler.HandleAsync(new UpdatePortfolioCardViewCommand(1, ValidDto()), CancellationToken.None);

        Assert.False(result.IsSuccess);
        Assert.Equal("CARD_UPDATE_FAILED", result.ErrorCode);
    }

    // ==================== Delete ====================

    [Fact]
    public async Task Delete_NotFound_ReturnsNotFound()
    {
        var repo = new Mock<IPortfolioRepository>();
        repo.Setup(r => r.GetViewByIdAsync(1, It.IsAny<CancellationToken>()))
            .ReturnsAsync((PortfolioCardView?)null);

        var handler = new DeletePortfolioCardViewHandler(repo.Object);
        var result = await handler.HandleAsync(new DeletePortfolioCardViewCommand(1), CancellationToken.None);

        Assert.False(result.IsSuccess);
        Assert.Equal("CARD_NOT_FOUND", result.ErrorCode);
    }

    [Fact]
    public async Task Delete_Existing_ReturnsSuccess()
    {
        var repo = new Mock<IPortfolioRepository>();
        repo.Setup(r => r.GetViewByIdAsync(1, It.IsAny<CancellationToken>()))
            .ReturnsAsync(new PortfolioCardView { Id = 1 });
        repo.Setup(r => r.DeleteViewAsync(1, It.IsAny<CancellationToken>())).ReturnsAsync(true);

        var handler = new DeletePortfolioCardViewHandler(repo.Object);
        var result = await handler.HandleAsync(new DeletePortfolioCardViewCommand(1), CancellationToken.None);

        Assert.True(result.IsSuccess);
    }

    [Fact]
    public async Task Delete_RepoReturnsFalse_ReturnsDeleteFailed()
    {
        var repo = new Mock<IPortfolioRepository>();
        repo.Setup(r => r.GetViewByIdAsync(It.IsAny<long>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(new PortfolioCardView { Id = 1 });
        repo.Setup(r => r.DeleteViewAsync(It.IsAny<long>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(false);

        var handler = new DeletePortfolioCardViewHandler(repo.Object);
        var result = await handler.HandleAsync(new DeletePortfolioCardViewCommand(1), CancellationToken.None);

        Assert.False(result.IsSuccess);
        Assert.Equal("CARD_DELETE_FAILED", result.ErrorCode);
    }
}
