using Kontursvet.Application.Abstractions;
using Kontursvet.Application.Dtos;
using Kontursvet.Application.Features.Portfolio.Card;
using Kontursvet.Domain.Entities;
using Moq;

namespace Kontursvet.Application.Tests.Features.Portfolio.Card;

/// <summary>
/// Тесты CRUD-обработчиков карточек портфолио: валидация, 404, нормализация пагинации,
/// перенос значений DTO → домен и обратно.
/// </summary>
public class PortfolioCardHandlerTests
{
    private static PortfolioCardDto ValidDto(long id = 0) => new()
    {
        Id = id,
        Link = "project-one",
        Title = "Освещение гостиной",
        SubTitle = "ЖК «Контур»",
        Description = "Точечный свет + подсветка ниши",
        Img = new ImgDto { Src = "/uploads/2026/10/img.jpg", Alt = "Гостиная" }
    };

    // ==================== Create ====================

    [Fact]
    public async Task Create_EmptyLink_ReturnsLinkRequired()
    {
        var repo = new Mock<IPortfolioRepository>();
        var dto = ValidDto();
        dto.Link = "";

        var handler = new CreatePortfolioCardHandler(repo.Object);
        var result = await handler.HandleAsync(new CreatePortfolioCardCommand(dto), CancellationToken.None);

        Assert.False(result.IsSuccess);
        Assert.Equal("CARD_LINK_REQUIRED", result.ErrorCode);
        repo.Verify(r => r.CreateCardAsync(It.IsAny<PortfolioCard>(), It.IsAny<CancellationToken>()), Times.Never);
    }

    [Fact]
    public async Task Create_EmptyTitle_ReturnsTitleRequired()
    {
        var dto = ValidDto();
        dto.Title = "   ";

        var handler = new CreatePortfolioCardHandler(new Mock<IPortfolioRepository>().Object);
        var result = await handler.HandleAsync(new CreatePortfolioCardCommand(dto), CancellationToken.None);

        Assert.False(result.IsSuccess);
        Assert.Equal("CARD_TITLE_REQUIRED", result.ErrorCode);
    }

    [Fact]
    public async Task Create_Valid_ReturnsNewId_AndPersistsMappedCard()
    {
        PortfolioCard? saved = null;
        var repo = new Mock<IPortfolioRepository>();
        repo.Setup(r => r.CreateCardAsync(It.IsAny<PortfolioCard>(), It.IsAny<CancellationToken>()))
            .Callback<PortfolioCard, CancellationToken>((c, _) => saved = c)
            .ReturnsAsync(77L);

        var dto = ValidDto();
        var handler = new CreatePortfolioCardHandler(repo.Object);
        var result = await handler.HandleAsync(new CreatePortfolioCardCommand(dto), CancellationToken.None);

        Assert.True(result.IsSuccess);
        Assert.Equal(77L, result.Value);
        Assert.NotNull(saved);
        Assert.Equal(dto.Link, saved.Link);
        Assert.Equal(dto.Title, saved.Title);
        Assert.Equal(dto.Img.Src, saved.ImgSrc);
        Assert.Equal(dto.Img.Alt, saved.ImgAlt);
    }

    // ==================== Update ====================

    [Fact]
    public async Task Update_EmptyLink_ReturnsNameRequired()
    {
        var dto = ValidDto();
        dto.Link = "";

        var handler = new UpdatePortfolioCardHandler(new Mock<IPortfolioRepository>().Object);
        var result = await handler.HandleAsync(new UpdatePortfolioCardCommand(1, dto), CancellationToken.None);

        Assert.False(result.IsSuccess);
        // В обработчике это намеренно код CARD_NAME_REQUIRED (исторически)
        Assert.Equal("CARD_NAME_REQUIRED", result.ErrorCode);
    }

    [Fact]
    public async Task Update_EmptyTitle_ReturnsTitleRequired()
    {
        var dto = ValidDto();
        dto.Title = "";

        var handler = new UpdatePortfolioCardHandler(new Mock<IPortfolioRepository>().Object);
        var result = await handler.HandleAsync(new UpdatePortfolioCardCommand(1, dto), CancellationToken.None);

        Assert.False(result.IsSuccess);
        Assert.Equal("CARD_TITLE_REQUIRED", result.ErrorCode);
    }

    [Fact]
    public async Task Update_CardNotFound_ReturnsNotFound()
    {
        var repo = new Mock<IPortfolioRepository>();
        repo.Setup(r => r.GetCardByIdAsync(5, It.IsAny<CancellationToken>()))
            .ReturnsAsync((PortfolioCard?)null);

        var handler = new UpdatePortfolioCardHandler(repo.Object);
        var result = await handler.HandleAsync(new UpdatePortfolioCardCommand(5, ValidDto()), CancellationToken.None);

        Assert.False(result.IsSuccess);
        Assert.Equal("CARD_NOT_FOUND", result.ErrorCode);
        repo.Verify(r => r.UpdateCardAsync(It.IsAny<PortfolioCard>(), It.IsAny<CancellationToken>()), Times.Never);
    }

    [Fact]
    public async Task Update_Existing_ReturnsSuccess_AndUsesRouteId()
    {
        var repo = new Mock<IPortfolioRepository>();
        repo.Setup(r => r.GetCardByIdAsync(3, It.IsAny<CancellationToken>()))
            .ReturnsAsync(new PortfolioCard { Id = 3, Link = "old", Title = "old" });
        repo.Setup(r => r.UpdateCardAsync(It.IsAny<PortfolioCard>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(true);

        var dto = ValidDto(id: 999); // id в теле игнорируется, берётся из маршрута
        var handler = new UpdatePortfolioCardHandler(repo.Object);
        var result = await handler.HandleAsync(new UpdatePortfolioCardCommand(3, dto), CancellationToken.None);

        Assert.True(result.IsSuccess);
        Assert.Equal(3L, dto.Id);
        repo.Verify(r => r.UpdateCardAsync(It.Is<PortfolioCard>(c => c.Id == 3), It.IsAny<CancellationToken>()), Times.Once);
    }

    [Fact]
    public async Task Update_RepoReturnsFalse_ReturnsUpdateFailed()
    {
        var repo = new Mock<IPortfolioRepository>();
        repo.Setup(r => r.GetCardByIdAsync(It.IsAny<long>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(new PortfolioCard { Id = 1 });
        repo.Setup(r => r.UpdateCardAsync(It.IsAny<PortfolioCard>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(false);

        var handler = new UpdatePortfolioCardHandler(repo.Object);
        var result = await handler.HandleAsync(new UpdatePortfolioCardCommand(1, ValidDto()), CancellationToken.None);

        Assert.False(result.IsSuccess);
        Assert.Equal("CARD_UPDATE_FAILED", result.ErrorCode);
    }

    // ==================== GetById ====================

    [Fact]
    public async Task GetById_Exists_ReturnsMappedDto()
    {
        var card = new PortfolioCard
        {
            Id = 10,
            Link = "link",
            Title = "title",
            SubTitle = "subtitle",
            Description = "desc",
            ImgSrc = "/uploads/a.jpg",
            ImgAlt = "alt",
            UpdatedAt = new DateTime(2026, 10, 5, 12, 0, 0, DateTimeKind.Utc)
        };

        var repo = new Mock<IPortfolioRepository>();
        repo.Setup(r => r.GetCardByIdAsync(10, It.IsAny<CancellationToken>())).ReturnsAsync(card);

        var handler = new GetPortfolioCardHandler(repo.Object);
        var result = await handler.HandleAsync(new GetPortfolioCardQuery(10), CancellationToken.None);

        Assert.True(result.IsSuccess);
        Assert.Equal(card.Id, result.Value!.Id);
        Assert.Equal(card.Link, result.Value!.Link);
        Assert.Equal(card.ImgSrc, result.Value!.Img.Src);
        Assert.Equal(card.ImgAlt, result.Value!.Img.Alt);
        Assert.Equal(card.UpdatedAt, result.Value!.UpdatedAt);
    }

    [Fact]
    public async Task GetById_NotFound_ReturnsNotFound()
    {
        var repo = new Mock<IPortfolioRepository>();
        repo.Setup(r => r.GetCardByIdAsync(404, It.IsAny<CancellationToken>()))
            .ReturnsAsync((PortfolioCard?)null);

        var handler = new GetPortfolioCardHandler(repo.Object);
        var result = await handler.HandleAsync(new GetPortfolioCardQuery(404), CancellationToken.None);

        Assert.False(result.IsSuccess);
        Assert.Equal("CARD_NOT_FOUND", result.ErrorCode);
    }

    // ==================== GetAll (пагинация) ====================

    [Theory]
    [InlineData(-10, 0, 0, 1)]    // отрицательный skip → 0, take<1 → 1
    [InlineData(0, 500, 0, 200)]  // take сверху ограничен MaxTake=200
    [InlineData(20, 10, 20, 10)]  // корректные значения проходят как есть
    public async Task GetAll_NormalizesSkipAndTake(int skip, int take, int expSkip, int expTake)
    {
        var repo = new Mock<IPortfolioRepository>();
        repo.Setup(r => r.GetAllCardsAsync(It.IsAny<int>(), It.IsAny<int>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(new List<PortfolioCard>());

        var handler = new GetPortfolioCardsHandler(repo.Object);
        await handler.HandleAsync(new GetPortfolioCardsQuery(skip, take), CancellationToken.None);

        repo.Verify(
            r => r.GetAllCardsAsync(expSkip, expTake, It.IsAny<CancellationToken>()),
            Times.Once);
    }

    [Fact]
    public async Task GetAll_MapsAllCardsToDtos()
    {
        var cards = new List<PortfolioCard>
        {
            new() { Id = 1, Link = "a", Title = "A" },
            new() { Id = 2, Link = "b", Title = "B" }
        };

        var repo = new Mock<IPortfolioRepository>();
        repo.Setup(r => r.GetAllCardsAsync(0, 10, It.IsAny<CancellationToken>()))
            .ReturnsAsync(cards);

        var handler = new GetPortfolioCardsHandler(repo.Object);
        var result = await handler.HandleAsync(new GetPortfolioCardsQuery(0, 10), CancellationToken.None);

        Assert.True(result.IsSuccess);
        Assert.Equal(2, result.Value!.Count);
        Assert.Equal(new[] { 1L, 2L }, result.Value!.Select(d => d.Id));
    }

    // ==================== Delete ====================

    [Fact]
    public async Task Delete_NotFound_ReturnsNotFound()
    {
        var repo = new Mock<IPortfolioRepository>();
        repo.Setup(r => r.GetCardByIdAsync(1, It.IsAny<CancellationToken>()))
            .ReturnsAsync((PortfolioCard?)null);

        var handler = new DeletePortfolioCardHandler(repo.Object);
        var result = await handler.HandleAsync(new DeletePortfolioCardCommand(1), CancellationToken.None);

        Assert.False(result.IsSuccess);
        Assert.Equal("CARD_NOT_FOUND", result.ErrorCode);
        repo.Verify(r => r.DeleteCardAsync(It.IsAny<long>(), It.IsAny<CancellationToken>()), Times.Never);
    }

    [Fact]
    public async Task Delete_Existing_ReturnsSuccess()
    {
        var repo = new Mock<IPortfolioRepository>();
        repo.Setup(r => r.GetCardByIdAsync(1, It.IsAny<CancellationToken>()))
            .ReturnsAsync(new PortfolioCard { Id = 1 });
        repo.Setup(r => r.DeleteCardAsync(1, It.IsAny<CancellationToken>())).ReturnsAsync(true);

        var handler = new DeletePortfolioCardHandler(repo.Object);
        var result = await handler.HandleAsync(new DeletePortfolioCardCommand(1), CancellationToken.None);

        Assert.True(result.IsSuccess);
    }

    [Fact]
    public async Task Delete_RepoReturnsFalse_ReturnsDeleteFailed()
    {
        var repo = new Mock<IPortfolioRepository>();
        repo.Setup(r => r.GetCardByIdAsync(It.IsAny<long>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(new PortfolioCard { Id = 1 });
        repo.Setup(r => r.DeleteCardAsync(It.IsAny<long>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(false);

        var handler = new DeletePortfolioCardHandler(repo.Object);
        var result = await handler.HandleAsync(new DeletePortfolioCardCommand(1), CancellationToken.None);

        Assert.False(result.IsSuccess);
        Assert.Equal("CARD_DELETE_FAILED", result.ErrorCode);
    }

    // ==================== Counts ====================

    [Fact]
    public async Task CardsCount_NullFromRepo_ReturnsError()
    {
        var repo = new Mock<IPortfolioRepository>();
        repo.Setup(r => r.GetCardsCountAsync(It.IsAny<CancellationToken>()))
            .ReturnsAsync((PortfolioCardCount?)null);

        var handler = new GetPortfolioCardsCountHandler(repo.Object);
        var result = await handler.HandleAsync(new GetPortfolioCardsCount(), CancellationToken.None);

        Assert.False(result.IsSuccess);
        Assert.Equal("ERROR_GET_CARDS_COUNT", result.ErrorCode);
    }

    [Fact]
    public async Task CardsCount_Exists_ReturnsDto()
    {
        var repo = new Mock<IPortfolioRepository>();
        repo.Setup(r => r.GetCardsCountAsync(It.IsAny<CancellationToken>()))
            .ReturnsAsync(new PortfolioCardCount { Quantity = 42, Last = 7 });

        var handler = new GetPortfolioCardsCountHandler(repo.Object);
        var result = await handler.HandleAsync(new GetPortfolioCardsCount(), CancellationToken.None);

        Assert.True(result.IsSuccess);
        Assert.Equal(42L, result.Value!.Quantity);
        Assert.Equal(7L, result.Value!.Last);
    }

    [Fact]
    public async Task PhotosCount_NullFromRepo_ReturnsError()
    {
        var repo = new Mock<IPortfolioRepository>();
        repo.Setup(r => r.GetPhotosCountAsync(It.IsAny<CancellationToken>()))
            .ReturnsAsync((long?)null);

        var handler = new GetPortfolioPhotosCountHandler(repo.Object);
        var result = await handler.HandleAsync(new GetPortfolioPhotosCount(), CancellationToken.None);

        Assert.False(result.IsSuccess);
        Assert.Equal("ERROR_GET_PHOTOS_COUNT", result.ErrorCode);
    }

    [Fact]
    public async Task PhotosCount_Exists_ReturnsDto()
    {
        var repo = new Mock<IPortfolioRepository>();
        repo.Setup(r => r.GetPhotosCountAsync(It.IsAny<CancellationToken>())).ReturnsAsync(128L);

        var handler = new GetPortfolioPhotosCountHandler(repo.Object);
        var result = await handler.HandleAsync(new GetPortfolioPhotosCount(), CancellationToken.None);

        Assert.True(result.IsSuccess);
        Assert.Equal(128L, result.Value!.Photos);
    }
}
