using Kontursvet.Application.Abstractions;
using Kontursvet.Domain.Common;

namespace Kontursvet.Application.Features.Leads;

public sealed record UploadFileCommand(
    Stream Content,
    string Filename,
    string ContentType,
    long Size);


public sealed class UploadLeadFileHandler(IUploadLeadFile upload)
{

    private const long MaxSizeBytes = 10 * 1024 * 1024;   // 10 МБ

    private static readonly HashSet<string> AllowedContentTypes =
        new(StringComparer.OrdinalIgnoreCase)
        {
            "image/jpeg", "image/png", "image/webp",
            "image/gif", "image/svg+xml", "image/avif",
            "image/heic", "image/heif"
        };

    public async Task<Result<UploadedLeadFile>> HandleAsync(UploadFileCommand cmd, CancellationToken ct)
    {

        if (cmd.Size <= 0)
            return Result<UploadedLeadFile>.Failure("Файл пуст", "FILE_EMPTY");

        if (cmd.Size > MaxSizeBytes)
            return Result<UploadedLeadFile>.Failure(
                $"Файл больше {MaxSizeBytes / 1024 / 1024} МБ", "FILE_TOO_LARGE");

        if (!AllowedContentTypes.Contains(cmd.ContentType))
            return Result<UploadedLeadFile>.Failure(
                $"Недопустимый тип файла: {cmd.ContentType}", "FILE_TYPE_NOT_ALLOWED");

        if (string.IsNullOrWhiteSpace(cmd.Filename))
            return Result<UploadedLeadFile>.Failure("Имя файла не задано", "FILE_NAME_REQUIRED");

        // Транзит: стрим сразу в MAX. Локально не сохраняем.
        var uploaded = await upload.UploadAsync(
            cmd.Content, cmd.Filename, cmd.ContentType, ct);

        return Result<UploadedLeadFile>.Success(uploaded);
    }

}
