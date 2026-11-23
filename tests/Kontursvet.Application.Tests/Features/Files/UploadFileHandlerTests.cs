using Kontursvet.Application.Abstractions;
using Kontursvet.Application.Features.Files;
using Moq;

namespace Kontursvet.Application.Tests.Features.Files;

/// <summary>
/// Тесты загрузки файлов в локальное хранилище: пустой файл, превышение размера,
/// недопустимый MIME-тип, пустое имя, успешный путь.
/// </summary>
public class UploadFileHandlerTests
{
    private const long MaxSize = 10 * 1024 * 1024; // как в обработчике

    private static Stream Content() => new MemoryStream([1, 2, 3]);

    private static UploadFileCommand Cmd(
        long size = 1024,
        string type = "image/jpeg",
        string name = "photo.jpg") =>
        new(Content(), name, type, size);

    [Theory]
    [InlineData(0)]
    [InlineData(-1)]
    public async Task Handle_EmptyFile_ReturnsFileEmpty(long size)
    {
        var storage = new Mock<IFileStorage>();
        var handler = new UploadFileHandler(storage.Object);

        var result = await handler.HandleAsync(Cmd(size: size), CancellationToken.None);

        Assert.False(result.IsSuccess);
        Assert.Equal("FILE_EMPTY", result.ErrorCode);
        storage.Verify(s => s.SaveAsync(
            It.IsAny<Stream>(), It.IsAny<string>(), It.IsAny<string>(), It.IsAny<CancellationToken>()),
            Times.Never);
    }

    [Fact]
    public async Task Handle_TooLargeFile_ReturnsFileTooLarge()
    {
        var handler = new UploadFileHandler(new Mock<IFileStorage>().Object);

        var result = await handler.HandleAsync(Cmd(size: MaxSize + 1), CancellationToken.None);

        Assert.False(result.IsSuccess);
        Assert.Equal("FILE_TOO_LARGE", result.ErrorCode);
        Assert.Contains("10 МБ", result.Error);
    }

    [Theory]
    [InlineData("application/pdf")]
    [InlineData("image/tiff")]
    [InlineData("text/plain")]
    [InlineData("")]
    public async Task Handle_NotAllowedType_ReturnsFileTypeNotAllowed(string contentType)
    {
        var handler = new UploadFileHandler(new Mock<IFileStorage>().Object);

        var result = await handler.HandleAsync(Cmd(type: contentType), CancellationToken.None);

        Assert.False(result.IsSuccess);
        Assert.Equal("FILE_TYPE_NOT_ALLOWED", result.ErrorCode);
    }

    [Theory]
    [InlineData("image/jpeg")]
    [InlineData("image/png")]
    [InlineData("image/webp")]
    [InlineData("image/gif")]
    [InlineData("image/svg+xml")]
    [InlineData("image/avif")]
    [InlineData("IMAGE/JPEG")] // регистр не важен (OrdinalIgnoreCase)
    public async Task Handle_AllowedTypes_PassValidation(string contentType)
    {
        var storage = new Mock<IFileStorage>();
        storage
            .Setup(s => s.SaveAsync(It.IsAny<Stream>(), It.IsAny<string>(), It.IsAny<string>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(new StoredFile("/uploads/2026/11/a.jpg", "a.jpg", 1024, contentType));

        var handler = new UploadFileHandler(storage.Object);
        var result = await handler.HandleAsync(Cmd(type: contentType), CancellationToken.None);

        Assert.True(result.IsSuccess);
    }

    [Theory]
    [InlineData("")]
    [InlineData("   ")]
    [InlineData(null)]
    public async Task Handle_EmptyFilename_ReturnsFileNameRequired(string? filename)
    {
        var storage = new Mock<IFileStorage>();
        var handler = new UploadFileHandler(storage.Object);

        var result = await handler.HandleAsync(
            Cmd(name: filename!), CancellationToken.None);

        Assert.False(result.IsSuccess);
        Assert.Equal("FILE_NAME_REQUIRED", result.ErrorCode);
        storage.Verify(s => s.SaveAsync(
            It.IsAny<Stream>(), It.IsAny<string>(), It.IsAny<string>(), It.IsAny<CancellationToken>()),
            Times.Never);
    }

    [Fact]
    public async Task Handle_Valid_ForwardsArgumentsToStorage()
    {
        var stored = new StoredFile("/uploads/2026/11/guid.jpg", "cat.jpg", 2048, "image/jpeg");
        var storage = new Mock<IFileStorage>();
        Stream? passedStream = null;
        storage
            .Setup(s => s.SaveAsync(It.IsAny<Stream>(), "cat.jpg", "image/png", It.IsAny<CancellationToken>()))
            .Callback<Stream, string, string, CancellationToken>((st, _, _, _) => passedStream = st)
            .ReturnsAsync(stored);

        var handler = new UploadFileHandler(storage.Object);
        var result = await handler.HandleAsync(
            new UploadFileCommand(Content(), "cat.jpg", "image/png", 2048), CancellationToken.None);

        Assert.True(result.IsSuccess);
        Assert.Equal(stored, result.Value);
        Assert.Same(result.Value!.Url, stored.Url);
        Assert.NotNull(passedStream);
    }

    [Fact]
    public async Task Handle_MaxSizeBoundary_Succeeds()
    {
        var storage = new Mock<IFileStorage>();
        storage
            .Setup(s => s.SaveAsync(It.IsAny<Stream>(), It.IsAny<string>(), It.IsAny<string>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(new StoredFile("/uploads/x.jpg", "x.jpg", MaxSize, "image/jpeg"));

        var handler = new UploadFileHandler(storage.Object);
        var result = await handler.HandleAsync(Cmd(size: MaxSize), CancellationToken.None);

        Assert.True(result.IsSuccess);
    }
}
