using Kontursvet.Application.Abstractions;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;

namespace Kontursvet.Infrastructure.Storage;

public sealed class LocalFileStorage(
    IConfiguration config,
    ILogger<LocalFileStorage> logger) : IFileStorage
{

    // Корень wwwroot, например /app/wwwroot в Docker или D:\proj\src\Kontursvet.Api\wwwroot

    private readonly string RootPath = config["Storage:LocalRoot"] ?? Path.Combine(AppContext.BaseDirectory, "wwwroot");

    private readonly string UploadsSegment = "uploads";

    public async Task<StoredFile> SaveAsync(
        Stream content,
        string originalFilename,
        string contentType,
        CancellationToken ct)
    {
        var now = DateTime.UtcNow;
        var subFolder = Path.Combine(now.ToString("yyyy"), now.ToString("MM"));

        var ext = Path.GetExtension(originalFilename).ToLowerInvariant();
        if (string.IsNullOrEmpty(ext))
            ext = GetExtensionFromContentType(contentType);

        var uniqueName = $"{Guid.NewGuid():N}{ext}";

        var absoluteFolder = Path.Combine(RootPath, UploadsSegment, subFolder);
        Directory.CreateDirectory(absoluteFolder);

        var absolutePath = Path.Combine(absoluteFolder, uniqueName);

        await using (var fs = File.Create(absolutePath))
        {
            await content.CopyToAsync(fs, ct);
        }

        // Относительный URL — то, что отдадим фронту
        var url = $"/{UploadsSegment}/{now:yyyy}/{now:MM}/{uniqueName}".Replace('\\', '/');

        long size;
        await using (var fs = File.OpenRead(absolutePath))
        {
            size = fs.Length;
        }

        logger.LogInformation("Файл сохранён: {Url} ({Size} байт)", url, size);

        return new StoredFile(url, originalFilename, size, contentType);
    }

    public Task<bool> DeleteAsync(string url, CancellationToken ct)
    {
        try
        {
            // url приходит в виде "/uploads/2026/09/xxx.jpg"
            var relative = url.TrimStart('/').Replace('/', Path.DirectorySeparatorChar);
            var absolute = Path.Combine(RootPath, relative);

            if (!File.Exists(absolute))
                return Task.FromResult(false);

            File.Delete(absolute);
            logger.LogInformation("Файл удалён: {Url}", url);
            return Task.FromResult(true);
        }
        catch (Exception ex)
        {
            logger.LogWarning(ex, "Не удалось удалить файл: {Url}", url);
            return Task.FromResult(false);
        }
    }

    private static string GetExtensionFromContentType(string contentType) => contentType switch
    {
        "image/jpeg" => ".jpg",
        "image/png" => ".png",
        "image/webp" => ".webp",
        "image/gif" => ".gif",
        "image/svg+xml" => ".svg",
        "image/avif" => ".avif",
        _ => ".bin"
    };
}