namespace Kontursvet.Application.Abstractions;

public sealed record StoredFile(
    string Url,          // относительный путь, например /uploads/2026/09/abc.jpg
    string Filename,     // оригинальное имя файла от клиента
    long Size,           // размер в байтах
    string ContentType);

public interface IFileStorage
{
    Task<StoredFile> SaveAsync(
        Stream content,
        string originalFilename,
        string contentType,
        CancellationToken ct);

    Task<bool> DeleteAsync(string url, CancellationToken ct);
}