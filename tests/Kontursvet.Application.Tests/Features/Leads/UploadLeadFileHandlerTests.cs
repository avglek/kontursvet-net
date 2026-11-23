using Kontursvet.Application.Abstractions;
using Kontursvet.Application.Features.Leads;
using Moq;
using UploadLeadFileCommand = Kontursvet.Application.Features.Leads.UploadFileCommand;

namespace Kontursvet.Application.Tests.Features.Leads;

/// <summary>
/// Тесты загрузки файла лида в MAX (транзит): пустой файл, размер, MIME-типы
/// (включая heic/heif, которых нет в обычной загрузке), имя файла, успешный путь.
/// </summary>
public class UploadLeadFileHandlerTests
{
    private const long MaxSize = 10 * 1024 * 1024; // как в обработчике

    private static UploadLeadFileCommand Cmd(
        long size = 1024,
        string type = "image/jpeg",
        string name = "photo.jpg") =>
        new(new MemoryStream([1, 2, 3]), name, type, size);

    [Theory]
    [InlineData(0)]
    [InlineData(-5)]
    public async Task Handle_EmptyFile_ReturnsFileEmpty(long size)
    {
        var upload = new Mock<IUploadLeadFile>();
        var handler = new UploadLeadFileHandler(upload.Object);

        var result = await handler.HandleAsync(Cmd(size: size), CancellationToken.None);

        Assert.False(result.IsSuccess);
        Assert.Equal("FILE_EMPTY", result.ErrorCode);
        upload.Verify(u => u.UploadAsync(
            It.IsAny<Stream>(), It.IsAny<string>(), It.IsAny<string>(), It.IsAny<CancellationToken>()),
            Times.Never);
    }

    [Fact]
    public async Task Handle_TooLargeFile_ReturnsFileTooLarge()
    {
        var handler = new UploadLeadFileHandler(new Mock<IUploadLeadFile>().Object);

        var result = await handler.HandleAsync(Cmd(size: MaxSize + 1), CancellationToken.None);

        Assert.False(result.IsSuccess);
        Assert.Equal("FILE_TOO_LARGE", result.ErrorCode);
    }

    [Fact]
    public async Task Handle_MaxSizeBoundary_Succeeds()
    {
        var upload = new Mock<IUploadLeadFile>();
        upload
            .Setup(u => u.UploadAsync(It.IsAny<Stream>(), It.IsAny<string>(), It.IsAny<string>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(new UploadedLeadFile("tok", "x.jpg", "image/jpeg"));

        var handler = new UploadLeadFileHandler(upload.Object);
        var result = await handler.HandleAsync(Cmd(size: MaxSize), CancellationToken.None);

        Assert.True(result.IsSuccess);
    }

    [Theory]
    [InlineData("image/heic")]
    [InlineData("image/heif")]
    public async Task Handle_HeicTypes_AreAllowed(string contentType)
    {
        var upload = new Mock<IUploadLeadFile>();
        upload
            .Setup(u => u.UploadAsync(It.IsAny<Stream>(), It.IsAny<string>(), It.IsAny<string>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(new UploadedLeadFile("tok", "x.heic", contentType));

        var handler = new UploadLeadFileHandler(upload.Object);
        var result = await handler.HandleAsync(Cmd(type: contentType), CancellationToken.None);

        Assert.True(result.IsSuccess);
    }

    [Theory]
    [InlineData("application/pdf")]
    [InlineData("video/mp4")]
    public async Task Handle_NotAllowedType_ReturnsFileTypeNotAllowed(string contentType)
    {
        var handler = new UploadLeadFileHandler(new Mock<IUploadLeadFile>().Object);

        var result = await handler.HandleAsync(Cmd(type: contentType), CancellationToken.None);

        Assert.False(result.IsSuccess);
        Assert.Equal("FILE_TYPE_NOT_ALLOWED", result.ErrorCode);
    }

    [Theory]
    [InlineData("")]
    [InlineData("   ")]
    [InlineData(null)]
    public async Task Handle_EmptyFilename_ReturnsFileNameRequired(string? filename)
    {
        var upload = new Mock<IUploadLeadFile>();
        var handler = new UploadLeadFileHandler(upload.Object);

        var result = await handler.HandleAsync(Cmd(name: filename!), CancellationToken.None);

        Assert.False(result.IsSuccess);
        Assert.Equal("FILE_NAME_REQUIRED", result.ErrorCode);
        upload.Verify(u => u.UploadAsync(
            It.IsAny<Stream>(), It.IsAny<string>(), It.IsAny<string>(), It.IsAny<CancellationToken>()),
            Times.Never);
    }

    [Fact]
    public async Task Handle_Valid_ReturnsUploadedToken()
    {
        var uploaded = new UploadedLeadFile("token-123", "plan.pdf.jpg", "image/webp");
        var upload = new Mock<IUploadLeadFile>();
        upload
            .Setup(u => u.UploadAsync(It.IsAny<Stream>(), "plan.pdf.jpg", "image/webp", It.IsAny<CancellationToken>()))
            .ReturnsAsync(uploaded);

        var handler = new UploadLeadFileHandler(upload.Object);
        var result = await handler.HandleAsync(
            Cmd(type: "image/webp", name: "plan.pdf.jpg"), CancellationToken.None);

        Assert.True(result.IsSuccess);
        Assert.Equal("token-123", result.Value!.Token);
        Assert.Equal("plan.pdf.jpg", result.Value!.Filename);
    }
}
