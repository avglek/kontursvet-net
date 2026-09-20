using Kontursvet.Domain.Entities;

namespace Kontursvet.Application.Abstractions;

public interface ILeadRepository
{
    Task<long> SaveAsync(Lead lead, IReadOnlyList<LeadAttachment> attachments, CancellationToken ct);
}