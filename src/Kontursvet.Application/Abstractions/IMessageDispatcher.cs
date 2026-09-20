using Kontursvet.Domain.Common;
using Kontursvet.Domain.Entities;

namespace Kontursvet.Application.Abstractions;

/// <summary>
/// Абстракция отправки лида в мессенджер (Telegram/WhatsApp/и т.д.).
/// Реализация подменяется через DI, application-слой ничего не знает о конкретном провайдере.
/// </summary>
public interface IMessageDispatcher
{
    Task<Result> DispatchAsync(Lead lead, IReadOnlyList<LeadAttachment> attachments, CancellationToken ct);
}