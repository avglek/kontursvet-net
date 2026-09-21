using Kontursvet.Application.Abstractions;
using Kontursvet.Domain.Common;

namespace Kontursvet.Application.Features.Files;

public sealed record UploadFileCommand(
    Stream Content,
    string Filename,
    string ContentType,
    long Size);

public sealed class UploadFileHandler(IFileStorage storage)
{
    // Максимальный размер файла — 10 МБ
    private const long MaxSizeBytes = 10 * 1024 * 1024;

    private static readonly HashSet<string> AllowedContentTypes = new(StringComparer.OrdinalIgnoreCase)
    {
        "image/jpeg",
        "image/png",
        "image/webp",
        "image/gif",
        "image/svg+xml",
        "image/avif"
    };

    public async Task<Result<StoredFile>> HandleAsync(UploadFileCommand cmd, CancellationToken ct)
    {
        if (cmd.Size <= 0)
            return Result<StoredFile>.Failure("Файл пуст", "FILE_EMPTY");

        if (cmd.Size > MaxSizeBytes)
            return Result<StoredFile>.Failure(
                $"Файл больше {MaxSizeBytes / 1024 / 1024} МБ", "FILE_TOO_LARGE");

        if (!AllowedContentTypes.Contains(cmd.ContentType))
            return Result<StoredFile>.Failure(
                $"Недопустимый тип файла: {cmd.ContentType}", "FILE_TYPE_NOT_ALLOWED");

        if (string.IsNullOrWhiteSpace(cmd.Filename))
            return Result<StoredFile>.Failure("Имя файла не задано", "FILE_NAME_REQUIRED");

        var stored = await storage.SaveAsync(cmd.Content, cmd.Filename, cmd.ContentType, ct);
        return Result<StoredFile>.Success(stored);
    }
}