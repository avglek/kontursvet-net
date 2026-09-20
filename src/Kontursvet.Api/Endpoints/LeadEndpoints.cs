using Kontursvet.Api.Common;
using Kontursvet.Application.Features.Leads;
using Kontursvet.Domain.Entities;

namespace Kontursvet.Api.Endpoints;

public static class LeadEndpoints
{
    public static IEndpointRouteBuilder MapLeadEndpoints(this IEndpointRouteBuilder app)
    {
        var group = app.MapGroup("/api/messages").WithTags("Messages");

        group.MapPost("/", async (LeadMessageRequest req, SendLeadHandler handler, CancellationToken ct) =>
        {
            var lead = new Lead
            {
                Name = req.Text.Name,
                Phone = new LeadPhone
                {
                    Digital = req.Text.Phone.Digital,
                    Format = req.Text.Phone.Format
                },
                Home = req.Text.Home,
                Location = req.Text.Location,
                Message = req.Text.Message
            };

            var attachments = req.Attachments.Select(a => new LeadAttachment
            {
                Filename = a.Filename,
                Content = a.Content,
                ContentType = a.ContentType,
                Encoding = a.Encoding
            }).ToList();

            var result = await handler.HandleAsync(new SendLeadCommand(lead, attachments), ct);
            return result.ToHttp();
        })
        .WithName("SendLead")
        .Produces<long>(StatusCodes.Status200OK)
        .ProducesProblem(StatusCodes.Status400BadRequest);

        return app;
    }
}

// Request-модели — соответствуют твоим TS-интерфейсам ILeadMessage, ILead, ILeadPhone, ILeadAttachment
public sealed record LeadMessageRequest(LeadTextDto Text, List<LeadAttachmentDto> Attachments);
public sealed record LeadTextDto(string Name, LeadPhoneDto Phone, string Home, string Location, string Message);
public sealed record LeadPhoneDto(string Digital, string Format);
public sealed record LeadAttachmentDto(string Filename, string? Content, string? ContentType, string Encoding = "base64");