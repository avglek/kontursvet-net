using Kontursvet.Application.Abstractions;
using Kontursvet.Domain.Common;
using Kontursvet.Domain.Entities;

namespace Kontursvet.Application.Features.Leads;

public sealed record SendLeadCommand(
    Lead Lead,
    IReadOnlyList<LeadAttachment> Attachments);

public sealed class SendLeadHandler
{
    private readonly ILeadRepository _repository;
    private readonly IMessageDispatcher _dispatcher;

    public SendLeadHandler(ILeadRepository repository, IMessageDispatcher dispatcher)
    {
        _repository = repository;
        _dispatcher = dispatcher;
    }

    public async Task<Result> HandleAsync(SendLeadCommand cmd, CancellationToken ct)
    {
        if (string.IsNullOrWhiteSpace(cmd.Lead.Name))
            return Result.Failure("Name is required", "LEAD_NAME_REQUIRED");

        if (string.IsNullOrWhiteSpace(cmd.Lead.Phone.Digital))
            return Result.Failure("Phone is required", "LEAD_PHONE_REQUIRED");

        // Сначала сохраняем — не потеряем лид даже если мессенджер упадёт
        //var id = await _repository.SaveAsync(cmd.Lead, cmd.Attachments, ct);

        // Затем отправляем (best-effort, ошибка не блокирует сохранение)
        var dispatch = await _dispatcher.DispatchAsync(cmd.Lead, cmd.Attachments, ct);
        if (!dispatch.IsSuccess)
        {
            // логируем, но не возвращаем failure — лид уже сохранён
            return Result.Failure("error messages");
        }

        return Result.Success();
    }
}