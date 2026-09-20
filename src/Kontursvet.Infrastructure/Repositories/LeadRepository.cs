using Dapper;
using Kontursvet.Application.Abstractions;
using Kontursvet.Domain.Entities;
using Kontursvet.Infrastructure.Data;

namespace Kontursvet.Infrastructure.Repositories;

public sealed class LeadRepository(IDbConnectionFactory factory) : ILeadRepository
{
    public async Task<long> SaveAsync(Lead lead, IReadOnlyList<LeadAttachment> attachments, CancellationToken ct)
    {
        using var db = factory.Create();

        const string insertLead = """
            INSERT INTO leads (name, phone_digital, phone_format, home, location, message, created_at)
            VALUES (@Name, @PhoneDigital, @PhoneFormat, @Home, @Location, @Message, now())
            RETURNING id;
            """;

        var id = await db.ExecuteScalarAsync<long>(new CommandDefinition(insertLead, new
        {
            lead.Name,
            PhoneDigital = lead.Phone.Digital,
            PhoneFormat = lead.Phone.Format,
            lead.Home,
            lead.Location,
            lead.Message
        }, cancellationToken: ct));

        if (attachments.Count > 0)
        {
            const string insertAtt = """
                INSERT INTO lead_attachments (lead_id, filename, content_type, content_base64, encoding)
                VALUES (@LeadId, @Filename, @ContentType, @Content, @Encoding);
                """;

            await db.ExecuteAsync(new CommandDefinition(insertAtt,
                attachments.Select(a => new
                {
                    LeadId = id,
                    a.Filename,
                    a.ContentType,
                    a.Content,
                    a.Encoding
                }), cancellationToken: ct));
        }

        return id;
    }
}