using Kontursvet.Application.Abstractions;
using Kontursvet.Domain.Common;
using Kontursvet.Domain.Entities;
using Microsoft.Extensions.Logging;

namespace Kontursvet.Infrastructure.Messaging;

/// <summary>
/// Заглушка: логирует лид. Позже заменим на TelegramDispatcher без изменения application-слоя.
/// </summary>
public sealed class NoOpMessageDispatcher(ILogger<NoOpMessageDispatcher> logger) : IMessageDispatcher
{
    public Task<Result> DispatchAsync(Lead lead, IReadOnlyList<LeadAttachment> attachments, CancellationToken ct)
    {

        logger.LogInformation("Lead received: {Name}, {Phone}, attachments: {Count}",
            lead.Name, lead.Phone.Digital, attachments.Count);
        return Task.FromResult(Result.Success());
    }

}