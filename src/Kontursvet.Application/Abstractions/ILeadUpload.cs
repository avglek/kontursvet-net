namespace Kontursvet.Application.Abstractions;

// public sealed record UploadLeadCommand(
//     Stream Content,
//     string Filename,
//     string ContentType,
//     long Size
//  );

public sealed record UploadedLeadFile(string Token, string Filename, string ContentType);
// public sealed record LeadTokenAttachment(
//    string Token,
//    string Filename);

public interface IUploadLeadFile
{
   /// <summary>
   /// Загружает файл в целевой сервис (MAX) и возвращает токен вложения.
   /// </summary>
   Task<UploadedLeadFile> UploadAsync(
       Stream content,
       string filename,
       string contentType,
       CancellationToken ct);
}