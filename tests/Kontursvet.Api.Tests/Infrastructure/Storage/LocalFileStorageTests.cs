using Kontursvet.Infrastructure.Storage;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Logging.Abstractions;

namespace Kontursvet.Api.Tests.Infrastructure.Storage;

/// <summary>
/// Тесты <see cref="LocalFileStorage"/>: раскладка по папкам год/месяц, имя на основе GUID,
/// определение расширения, содержимое возвращаемого URL, удаление файлов.
/// </summary>
public sealed class LocalFileStorageTests : IDisposable
{
    private readonly string _root;
    private readonly LocalFileStorage _storage;

    public LocalFileStorageTests()
    {
        _root = Path.Combine(Path.GetTempPath(), "kontursvet-storage-tests", Guid.NewGuid().ToString("N"));
        Directory.CreateDirectory(_root);

        var config = new ConfigurationBuilder()
            .AddInMemoryCollection(new Dictionary<string, string?> { ["Storage:LocalRoot"] = _root })
            .Build();

        _storage = new LocalFileStorage(config, NullLogger<LocalFileStorage>.Instance);
    }

    public void Dispose()
    {
        try
        {
            if (Directory.Exists(_root))
                Directory.Delete(_root, recursive: true);
        }
        catch (IOException)
        {
            // временные файлы тестов — не критично
        }
    }

    private static Stream Content(int bytes = 8) =>
        new MemoryStream(new byte[bytes].Select((_, i) => (byte)(i + 1)).ToArray());

    // ==================== Save ====================

    [Fact]
    public async Task Save_CreatesFileUnderYearMonthFolder()
    {
        var now = DateTime.UtcNow;
        var stored = await _storage.SaveAsync(Content(), "photo.jpg", "image/jpeg", CancellationToken.None);

        var expectedFolder = Path.Combine(_root, "uploads", now.ToString("yyyy"), now.ToString("MM"));
        Assert.True(Directory.Exists(expectedFolder));

        var physical = Path.Combine(expectedFolder, Path.GetFileName(stored.Url));
        Assert.True(File.Exists(physical));
    }

    [Fact]
    public async Task Save_UrlMatchesStorageContract()
    {
        var now = DateTime.UtcNow;
        var stored = await _storage.SaveAsync(Content(), "photo.jpg", "image/jpeg", CancellationToken.None);

        Assert.StartsWith($"/uploads/{now:yyyy}/{now:MM}/", stored.Url);
        Assert.DoesNotContain("\\", stored.Url); // URL всегда с прямыми слэшами
    }

    [Fact]
    public async Task Save_FileNameIsGuidWithOriginalExtension()
    {
        var stored = await _storage.SaveAsync(Content(), "My Photo.JPG", "image/jpeg", CancellationToken.None);

        var fileName = Path.GetFileName(stored.Url);
        var withoutExt = Path.GetFileNameWithoutExtension(fileName);

        Assert.Equal(".jpg", Path.GetExtension(fileName)); // расширение приводится к нижнему регистру
        Assert.True(Guid.TryParseExact(withoutExt, "N", out _));
    }

    [Fact]
    public async Task Save_PreservesContentBytesAndSize()
    {
        var stream = Content(16);
        var stored = await _storage.SaveAsync(stream, "a.png", "image/png", CancellationToken.None);

        Assert.Equal(16, stored.Size);
        Assert.Equal("a.png", stored.Filename);   // оригинальное имя клиента
        Assert.Equal("image/png", stored.ContentType);

        var physical = Path.Combine(_root, stored.Url.TrimStart('/').Replace('/', Path.DirectorySeparatorChar));
        Assert.Equal(16, new FileInfo(physical).Length);
    }

    [Theory]
    [InlineData("image/jpeg", ".jpg")]
    [InlineData("image/png", ".png")]
    [InlineData("image/webp", ".webp")]
    [InlineData("image/gif", ".gif")]
    [InlineData("image/svg+xml", ".svg")]
    [InlineData("image/avif", ".avif")]
    [InlineData("application/octet-stream", ".bin")]
    public async Task Save_WithoutExtension_TakesItFromContentType(string contentType, string expectedExt)
    {
        var stored = await _storage.SaveAsync(Content(), "no-extension", contentType, CancellationToken.None);

        Assert.Equal(expectedExt, Path.GetExtension(stored.Url));
    }

    [Fact]
    public async Task Save_TwoFiles_ProduceDifferentUrls()
    {
        var first = await _storage.SaveAsync(Content(), "a.jpg", "image/jpeg", CancellationToken.None);
        var second = await _storage.SaveAsync(Content(), "b.jpg", "image/jpeg", CancellationToken.None);

        Assert.NotEqual(first.Url, second.Url);
    }

    // ==================== Delete ====================

    [Fact]
    public async Task Delete_ExistingFile_ReturnsTrueAndRemovesIt()
    {
        var stored = await _storage.SaveAsync(Content(), "a.jpg", "image/jpeg", CancellationToken.None);
        var physical = Path.Combine(_root, stored.Url.TrimStart('/').Replace('/', Path.DirectorySeparatorChar));

        var deleted = await _storage.DeleteAsync(stored.Url, CancellationToken.None);

        Assert.True(deleted);
        Assert.False(File.Exists(physical));
    }

    [Fact]
    public async Task Delete_MissingFile_ReturnsFalse()
    {
        var deleted = await _storage.DeleteAsync("/uploads/1999/01/nobody.jpg", CancellationToken.None);

        Assert.False(deleted);
    }

    [Fact]
    public async Task Delete_MalformedUrl_DoesNotThrow()
    {
        // Путь за пределами корня — операция должна завершиться false, а не исключением
        var deleted = await _storage.DeleteAsync("../../etc/passwd", CancellationToken.None);

        Assert.False(deleted);
    }

    [Fact]
    public async Task Delete_WhenFileSystemRefusesRemoval_ReturnsFalse()
    {
        if (!OperatingSystem.IsMacOS() && !OperatingSystem.IsLinux())
            return; // права на каталог как способ вызвать ошибку удаления — только для POSIX

        if (Environment.UserName.Equals("root", StringComparison.OrdinalIgnoreCase))
            return; // root удаляет файлы независимо от прав на каталог

        var stored = await _storage.SaveAsync(Content(), "locked.jpg", "image/jpeg", CancellationToken.None);
        var physical = Path.Combine(_root, stored.Url.TrimStart('/').Replace('/', Path.DirectorySeparatorChar));
        var parent = Path.GetDirectoryName(physical)!;

        // Закрываем каталогу право на запись: File.Exists увидит файл, а File.Delete упадёт
        File.SetUnixFileMode(parent, UnixFileMode.UserRead | UnixFileMode.UserExecute);
        try
        {
            Assert.True(File.Exists(physical));

            var deleted = await _storage.DeleteAsync(stored.Url, CancellationToken.None);

            Assert.False(deleted);
            Assert.True(File.Exists(physical)); // файл остался на месте
        }
        finally
        {
            File.SetUnixFileMode(
                parent,
                UnixFileMode.UserRead | UnixFileMode.UserWrite | UnixFileMode.UserExecute);
        }
    }
}
