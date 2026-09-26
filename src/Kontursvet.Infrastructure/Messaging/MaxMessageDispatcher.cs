using MAX.Bot;
using MAX.Bot.Interfaces.Models;
using MAX.Bot.Interfaces.Models.Request;
using MAX.Bot.Interfaces.Models.Request.Message;
using MAX.Bot.Interfaces.Models.Request.Message.Attachment;
using MAX.Bot.Interfaces.Models.Request.Message.Attachment.Payloads;

using Kontursvet.Application.Abstractions;
using Kontursvet.Domain.Common;
using Kontursvet.Domain.Entities;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;

namespace Kontursvet.Infrastructure.Messaging;

public sealed class MaxMessageDispatcher : IMessageDispatcher, IUploadLeadFile
{
    private readonly MaxBotClient _bot;
    private readonly long _chatId;
    private readonly ILogger<MaxMessageDispatcher> _logger;

    public MaxMessageDispatcher(IConfiguration config, ILogger<MaxMessageDispatcher> logger)
    {
        _logger = logger;

        var token = config["Max:Token"];
        if (string.IsNullOrEmpty(token))
            throw new InvalidOperationException("Max:Token не задан");

        var chatIdStr = config["Max:ChatId"];
        if (string.IsNullOrWhiteSpace(chatIdStr))
            throw new InvalidOperationException("Max:ChatId не задан");

        _chatId = long.Parse(chatIdStr);
        _bot = new MaxBotClient(token);
    }

    // ============ IFileUploader ============

    public async Task<UploadedLeadFile> UploadAsync(
        Stream content, string filename, string contentType, CancellationToken ct)
    {
        var token = await _bot.UploadsAsync(new UploadRequest
        {
            Type = UploadType.Image,
            Content = content,
            FileName = filename,
            ContentType = contentType,
        }, ct);

        return new UploadedLeadFile(token, filename, contentType);
    }

    // ============ IMessageDispatcher ============

    public async Task<Result> DispatchAsync(
        Lead lead,
        IReadOnlyList<LeadAttachment> attachments,
        CancellationToken ct)
    {
        try
        {
            var imageAttachments = new List<Attachment>();
            foreach (var att in attachments)
            {
                if (string.IsNullOrEmpty(att.Token))
                {
                    _logger.LogWarning(
                        "Вложение {Filename} пропущено: Token отсутствует", att.Filename);
                    continue;
                }

                imageAttachments.Add(new ImageAttachment
                {
                    Payload = new ImagePayload { Token = att.Token }
                });
            }

            // MAX: не более 12 картинок в одном сообщении
            const int maxImagesPerMessage = 12;
            if (imageAttachments.Count > maxImagesPerMessage)
            {
                _logger.LogWarning(
                    "Вложений {Total}, отправим только первые {Max}",
                    imageAttachments.Count, maxImagesPerMessage);
                imageAttachments = imageAttachments.Take(maxImagesPerMessage).ToList();
            }

            var message = new SendMessageRequest
            {
                ChatId = _chatId,
                Text = MarkdownText(lead),
                Format = MessageFormat.Markdown,
                Attachments = imageAttachments.Count > 0 ? imageAttachments : null
            };

            await SendWithRetryAsync(message, ct);
            return Result.Success();
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Ошибка отправки лида в MAX");
            return Result.Failure($"Ошибка MAX: {ex.Message}", "MAX_SEND_FAILED");
        }
    }

    // ============ Приватное ============

    private async Task SendWithRetryAsync(SendMessageRequest message, CancellationToken ct)
    {
        const int maxAttempts = 5;
        var delay = TimeSpan.FromSeconds(1);

        for (var attempt = 1; attempt <= maxAttempts; attempt++)
        {
            try
            {
                await _bot.SendMessageAsync(message, null, ct);
                return;
            }
            catch (Exception ex) when (IsAttachmentNotReady(ex))
            {
                if (attempt == maxAttempts)
                {
                    _logger.LogError(ex,
                        "MAX attachment.not.ready после {Max} попыток", maxAttempts);
                    throw;
                }

                _logger.LogWarning(
                    "MAX attachment.not.ready, попытка {Attempt}/{Max}, пауза {Delay}с",
                    attempt, maxAttempts, delay.TotalSeconds);

                await Task.Delay(delay, ct);
                delay = TimeSpan.FromSeconds(delay.TotalSeconds * 2);
            }
        }
    }

    private static bool IsAttachmentNotReady(Exception ex)
        => ex.Message.Contains("attachment.not.ready", StringComparison.OrdinalIgnoreCase)
        || (ex.InnerException?.Message.Contains("attachment.not.ready",
                StringComparison.OrdinalIgnoreCase) ?? false);

    private string MarkdownText(Lead lead)
    {
        var text = $"""
            **Заказчик:** {lead.Name}
            **Телефон:** [{lead.Phone.Format}](tel:{lead.Phone.Digital})
            """;

        if (!string.IsNullOrWhiteSpace(lead.Home))
            text += $"\n**Тип объекта:** {lead.Home}";
        if (!string.IsNullOrWhiteSpace(lead.Location))
            text += $"\n**Где находится:** {lead.Location}";
        if (!string.IsNullOrWhiteSpace(lead.Message))
            text += $"\n**Коротко о задаче:** {lead.Message}";

        return text;
    }
}