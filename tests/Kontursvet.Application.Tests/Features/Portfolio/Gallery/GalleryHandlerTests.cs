using Kontursvet.Application.Abstractions;
using Kontursvet.Application.Dtos;
using Kontursvet.Application.Features.Portfolio.Gallery;
using Kontursvet.Domain.Entities;
using Moq;

namespace Kontursvet.Application.Tests.Features.Portfolio.Gallery;

/// <summary>
/// Тесты галереи представления: валидация Key/Src, 404 по элементу и по представлению,
/// защита от дубликата Key, совпадение Key в path и body.
/// </summary>
public class GalleryHandlerTests
{
    private static PortfolioCardView ViewWith(params GalleryItem[] items) => new()
    {
        Id = 1,
        Name = "N",
        Part = "P",
        Title = "T",
        Gallery = items.ToList()
    };

    private static GalleryItemDto ItemDto(int key, string src = "/uploads/x.jpg") => new()
    {
        Key = key,
        Src = src,
        Alt = "alt",
        Figcaption = "caption"
    };

    // ==================== Add ====================

    [Fact]
    public async Task Add_NegativeKey_ReturnsKeyInvalid()
    {
        var handler = new AddGalleryItemHandler(new Mock<IPortfolioRepository>().Object);
        var result = await handler.HandleAsync(
            new AddGalleryItemCommand(1, ItemDto(-1)), CancellationToken.None);

        Assert.False(result.IsSuccess);
        Assert.Equal("GALLERY_KEY_INVALID", result.ErrorCode);
    }

    [Fact]
    public async Task Add_EmptySrc_ReturnsSrcRequired()
    {
        var handler = new AddGalleryItemHandler(new Mock<IPortfolioRepository>().Object);
        var result = await handler.HandleAsync(
            new AddGalleryItemCommand(1, ItemDto(0, "")), CancellationToken.None);

        Assert.False(result.IsSuccess);
        Assert.Equal("GALLERY_SRC_REQUIRED", result.ErrorCode);
    }

    [Fact]
    public async Task Add_ViewNotFound_ReturnsViewNotFound()
    {
        var repo = new Mock<IPortfolioRepository>();
        repo.Setup(r => r.GetViewByIdAsync(1, It.IsAny<CancellationToken>()))
            .ReturnsAsync((PortfolioCardView?)null);

        var handler = new AddGalleryItemHandler(repo.Object);
        var result = await handler.HandleAsync(
            new AddGalleryItemCommand(1, ItemDto(0)), CancellationToken.None);

        Assert.False(result.IsSuccess);
        Assert.Equal("VIEW_NOT_FOUND", result.ErrorCode);
    }

    [Fact]
    public async Task Add_DuplicateKey_ReturnsKeyDuplicate()
    {
        var repo = new Mock<IPortfolioRepository>();
        repo.Setup(r => r.GetViewByIdAsync(1, It.IsAny<CancellationToken>()))
            .ReturnsAsync(ViewWith(new GalleryItem(0, "/uploads/0.jpg", "", "")));

        var handler = new AddGalleryItemHandler(repo.Object);
        var result = await handler.HandleAsync(
            new AddGalleryItemCommand(1, ItemDto(0)), CancellationToken.None);

        Assert.False(result.IsSuccess);
        Assert.Equal("GALLERY_KEY_DUPLICATE", result.ErrorCode);
        repo.Verify(r => r.AddGalleryItemAsync(It.IsAny<long>(), It.IsAny<GalleryItem>(), It.IsAny<CancellationToken>()), Times.Never);
    }

    [Fact]
    public async Task Add_Valid_ReturnsSuccess_AndPassesMappedItem()
    {
        GalleryItem? added = null;
        var repo = new Mock<IPortfolioRepository>();
        repo.Setup(r => r.GetViewByIdAsync(1, It.IsAny<CancellationToken>()))
            .ReturnsAsync(ViewWith(new GalleryItem(0, "/uploads/0.jpg", "", "")));
        repo.Setup(r => r.AddGalleryItemAsync(1, It.IsAny<GalleryItem>(), It.IsAny<CancellationToken>()))
            .Callback<long, GalleryItem, CancellationToken>((_, i, _) => added = i)
            .ReturnsAsync(true);

        var handler = new AddGalleryItemHandler(repo.Object);
        var result = await handler.HandleAsync(
            new AddGalleryItemCommand(1, ItemDto(5, "/uploads/5.jpg")), CancellationToken.None);

        Assert.True(result.IsSuccess);
        Assert.NotNull(added);
        Assert.Equal(5, added.Key);
        Assert.Equal("/uploads/5.jpg", added.Src);
    }

    [Fact]
    public async Task Add_RepoReturnsFalse_ReturnsAddFailed()
    {
        var repo = new Mock<IPortfolioRepository>();
        repo.Setup(r => r.GetViewByIdAsync(It.IsAny<long>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(ViewWith());
        repo.Setup(r => r.AddGalleryItemAsync(It.IsAny<long>(), It.IsAny<GalleryItem>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(false);

        var handler = new AddGalleryItemHandler(repo.Object);
        var result = await handler.HandleAsync(
            new AddGalleryItemCommand(1, ItemDto(0)), CancellationToken.None);

        Assert.False(result.IsSuccess);
        Assert.Equal("GALLERY_ADD_FAILED", result.ErrorCode);
    }

    // ==================== Update ====================

    [Fact]
    public async Task Update_EmptySrc_ReturnsSrcRequired()
    {
        var handler = new UpdateGalleryItemHandler(new Mock<IPortfolioRepository>().Object);
        var result = await handler.HandleAsync(
            new UpdateGalleryItemCommand(1, 0, ItemDto(0, "")), CancellationToken.None);

        Assert.False(result.IsSuccess);
        Assert.Equal("GALLERY_SRC_REQUIRED", result.ErrorCode);
    }

    [Fact]
    public async Task Update_KeyMismatch_ReturnsKeyMismatch()
    {
        var handler = new UpdateGalleryItemHandler(new Mock<IPortfolioRepository>().Object);
        var result = await handler.HandleAsync(
            new UpdateGalleryItemCommand(1, 3, ItemDto(4)), CancellationToken.None);

        Assert.False(result.IsSuccess);
        Assert.Equal("GALLERY_KEY_MISMATCH", result.ErrorCode);
    }

    [Fact]
    public async Task Update_ViewNotFound_ReturnsViewNotFound()
    {
        var repo = new Mock<IPortfolioRepository>();
        repo.Setup(r => r.GetViewByIdAsync(It.IsAny<long>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync((PortfolioCardView?)null);

        var handler = new UpdateGalleryItemHandler(repo.Object);
        var result = await handler.HandleAsync(
            new UpdateGalleryItemCommand(1, 0, ItemDto(0)), CancellationToken.None);

        Assert.False(result.IsSuccess);
        Assert.Equal("VIEW_NOT_FOUND", result.ErrorCode);
    }

    [Fact]
    public async Task Update_ItemNotFound_ReturnsItemNotFound()
    {
        var repo = new Mock<IPortfolioRepository>();
        repo.Setup(r => r.GetViewByIdAsync(1, It.IsAny<CancellationToken>()))
            .ReturnsAsync(ViewWith(new GalleryItem(0, "/uploads/0.jpg", "", "")));

        var handler = new UpdateGalleryItemHandler(repo.Object);
        var result = await handler.HandleAsync(
            new UpdateGalleryItemCommand(1, 9, ItemDto(9)), CancellationToken.None);

        Assert.False(result.IsSuccess);
        Assert.Equal("GALLERY_ITEM_NOT_FOUND", result.ErrorCode);
    }

    [Fact]
    public async Task Update_Valid_ReturnsSuccess()
    {
        var repo = new Mock<IPortfolioRepository>();
        repo.Setup(r => r.GetViewByIdAsync(1, It.IsAny<CancellationToken>()))
            .ReturnsAsync(ViewWith(new GalleryItem(0, "/uploads/0.jpg", "", "")));
        repo.Setup(r => r.UpdateGalleryItemAsync(1, 0, It.IsAny<GalleryItem>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(true);

        var handler = new UpdateGalleryItemHandler(repo.Object);
        var result = await handler.HandleAsync(
            new UpdateGalleryItemCommand(1, 0, ItemDto(0, "/uploads/new.jpg")), CancellationToken.None);

        Assert.True(result.IsSuccess);
        repo.Verify(r => r.UpdateGalleryItemAsync(1, 0, It.IsAny<GalleryItem>(), It.IsAny<CancellationToken>()), Times.Once);
    }

    [Fact]
    public async Task Update_RepoReturnsFalse_ReturnsUpdateFailed()
    {
        var repo = new Mock<IPortfolioRepository>();
        repo.Setup(r => r.GetViewByIdAsync(It.IsAny<long>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(ViewWith(new GalleryItem(0, "/uploads/0.jpg", "", "")));
        repo.Setup(r => r.UpdateGalleryItemAsync(It.IsAny<long>(), It.IsAny<int>(), It.IsAny<GalleryItem>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(false);

        var handler = new UpdateGalleryItemHandler(repo.Object);
        var result = await handler.HandleAsync(
            new UpdateGalleryItemCommand(1, 0, ItemDto(0)), CancellationToken.None);

        Assert.False(result.IsSuccess);
        Assert.Equal("GALLERY_UPDATE_FAILED", result.ErrorCode);
    }

    // ==================== Delete ====================

    [Fact]
    public async Task Delete_ViewNotFound_ReturnsViewNotFound()
    {
        var repo = new Mock<IPortfolioRepository>();
        repo.Setup(r => r.GetViewByIdAsync(It.IsAny<long>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync((PortfolioCardView?)null);

        var handler = new DeleteGalleryItemHandler(repo.Object);
        var result = await handler.HandleAsync(
            new DeleteGalleryItemCommand(1, 0), CancellationToken.None);

        Assert.False(result.IsSuccess);
        Assert.Equal("VIEW_NOT_FOUND", result.ErrorCode);
    }

    [Fact]
    public async Task Delete_ItemNotFound_ReturnsItemNotFound()
    {
        var repo = new Mock<IPortfolioRepository>();
        repo.Setup(r => r.GetViewByIdAsync(1, It.IsAny<CancellationToken>()))
            .ReturnsAsync(ViewWith(new GalleryItem(0, "/uploads/0.jpg", "", "")));

        var handler = new DeleteGalleryItemHandler(repo.Object);
        var result = await handler.HandleAsync(
            new DeleteGalleryItemCommand(1, 42), CancellationToken.None);

        Assert.False(result.IsSuccess);
        Assert.Equal("GALLERY_ITEM_NOT_FOUND", result.ErrorCode);
    }

    [Fact]
    public async Task Delete_Valid_ReturnsSuccess()
    {
        var repo = new Mock<IPortfolioRepository>();
        repo.Setup(r => r.GetViewByIdAsync(1, It.IsAny<CancellationToken>()))
            .ReturnsAsync(ViewWith(new GalleryItem(3, "/uploads/3.jpg", "", "")));
        repo.Setup(r => r.DeleteGalleryItemAsync(1, 3, It.IsAny<CancellationToken>())).ReturnsAsync(true);

        var handler = new DeleteGalleryItemHandler(repo.Object);
        var result = await handler.HandleAsync(
            new DeleteGalleryItemCommand(1, 3), CancellationToken.None);

        Assert.True(result.IsSuccess);
    }

    [Fact]
    public async Task Delete_RepoReturnsFalse_ReturnsDeleteFailed()
    {
        var repo = new Mock<IPortfolioRepository>();
        repo.Setup(r => r.GetViewByIdAsync(It.IsAny<long>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(ViewWith(new GalleryItem(3, "/uploads/3.jpg", "", "")));
        repo.Setup(r => r.DeleteGalleryItemAsync(It.IsAny<long>(), It.IsAny<int>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(false);

        var handler = new DeleteGalleryItemHandler(repo.Object);
        var result = await handler.HandleAsync(
            new DeleteGalleryItemCommand(1, 3), CancellationToken.None);

        Assert.False(result.IsSuccess);
        Assert.Equal("GALLERY_DELETE_FAILED", result.ErrorCode);
    }
}
