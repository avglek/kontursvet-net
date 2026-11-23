using Kontursvet.Application.Abstractions;
using Kontursvet.Application.Features.Leads;
using Kontursvet.Domain.Common;
using Kontursvet.Domain.Entities;
using Moq;

namespace Kontursvet.Application.Tests.Features.Leads;

/// <summary>
/// Тесты отправки лида: валидация имени и телефона, best-effort отправка в MAX,
/// поведение при сбое диспетчера.
/// </summary>
public class SendLeadHandlerTests
{
    private static Lead ValidLead() => new()
    {
        Name = "Иван",
        Phone = new LeadPhone { Digital = "79991234567", Format = "+7(999)123-45-67" },
        Home = "Квартира",
        Location = "Санкт-Петербург",
        Message = "Нужна подсветка кухни",
        Estimate = "≈ 45 000 ₽"
    };

    [Theory]
    [InlineData("")]
    [InlineData("   ")]
    [InlineData(null)]
    public async Task Handle_EmptyName_ReturnsNameRequired(string? name)
    {
        var lead = ValidLead();
        lead.Name = name!;

        var repo = new Mock<ILeadRepository>();
        var dispatcher = new Mock<IMessageDispatcher>();
        var handler = new SendLeadHandler(repo.Object, dispatcher.Object);

        var result = await handler.HandleAsync(
            new SendLeadCommand(lead, []), CancellationToken.None);

        Assert.False(result.IsSuccess);
        Assert.Equal("LEAD_NAME_REQUIRED", result.ErrorCode);
        dispatcher.Verify(
            d => d.DispatchAsync(It.IsAny<Lead>(), It.IsAny<IReadOnlyList<LeadAttachment>>(), It.IsAny<CancellationToken>()),
            Times.Never);
    }

    [Theory]
    [InlineData("")]
    [InlineData("   ")]
    [InlineData(null)]
    public async Task Handle_EmptyPhoneDigital_ReturnsPhoneRequired(string? digital)
    {
        var lead = ValidLead();
        lead.Phone.Digital = digital!;

        var dispatcher = new Mock<IMessageDispatcher>();
        var handler = new SendLeadHandler(new Mock<ILeadRepository>().Object, dispatcher.Object);

        var result = await handler.HandleAsync(
            new SendLeadCommand(lead, []), CancellationToken.None);

        Assert.False(result.IsSuccess);
        Assert.Equal("LEAD_PHONE_REQUIRED", result.ErrorCode);
        dispatcher.Verify(
            d => d.DispatchAsync(It.IsAny<Lead>(), It.IsAny<IReadOnlyList<LeadAttachment>>(), It.IsAny<CancellationToken>()),
            Times.Never);
    }

    [Fact]
    public async Task Handle_DispatcherFails_ReturnsFailureWithMessages()
    {
        var dispatcher = new Mock<IMessageDispatcher>();
        dispatcher
            .Setup(d => d.DispatchAsync(It.IsAny<Lead>(), It.IsAny<IReadOnlyList<LeadAttachment>>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(Result.Failure("MAX недоступен", "MAX_SEND_FAILED"));

        var handler = new SendLeadHandler(new Mock<ILeadRepository>().Object, dispatcher.Object);
        var result = await handler.HandleAsync(new SendLeadCommand(ValidLead(), []), CancellationToken.None);

        Assert.False(result.IsSuccess);
        // Обработчик возвращает фиксированное сообщение без кода ошибки
        Assert.Equal("error messages", result.Error);
        Assert.Null(result.ErrorCode);
    }

    [Fact]
    public async Task Handle_DispatcherSucceeds_ReturnsSuccess_AndPassesPayload()
    {
        Lead? sentLead = null;
        IReadOnlyList<LeadAttachment>? sentAttachments = null;

        var dispatcher = new Mock<IMessageDispatcher>();
        dispatcher
            .Setup(d => d.DispatchAsync(It.IsAny<Lead>(), It.IsAny<IReadOnlyList<LeadAttachment>>(), It.IsAny<CancellationToken>()))
            .Callback<Lead, IReadOnlyList<LeadAttachment>, CancellationToken>((l, a, _) =>
            {
                sentLead = l;
                sentAttachments = a;
            })
            .ReturnsAsync(Result.Success());

        var attachments = new List<LeadAttachment>
        {
            new() { Filename = "plan.jpg", Token = "tok-1", ContentType = "image/jpeg" }
        };

        var lead = ValidLead();
        var handler = new SendLeadHandler(new Mock<ILeadRepository>().Object, dispatcher.Object);
        var result = await handler.HandleAsync(new SendLeadCommand(lead, attachments), CancellationToken.None);

        Assert.True(result.IsSuccess);
        Assert.Same(lead, sentLead);
        Assert.Same(attachments, sentAttachments);
    }

    [Fact]
    public async Task Handle_DoesNotSaveLeadToRepository_Yet()
    {
        // Сохранение лида в БД сейчас закомментировано в обработчике — фиксируем поведение
        var repo = new Mock<ILeadRepository>();
        var dispatcher = new Mock<IMessageDispatcher>();
        dispatcher
            .Setup(d => d.DispatchAsync(It.IsAny<Lead>(), It.IsAny<IReadOnlyList<LeadAttachment>>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(Result.Success());

        var handler = new SendLeadHandler(repo.Object, dispatcher.Object);
        await handler.HandleAsync(new SendLeadCommand(ValidLead(), []), CancellationToken.None);

        repo.Verify(
            r => r.SaveAsync(It.IsAny<Lead>(), It.IsAny<IReadOnlyList<LeadAttachment>>(), It.IsAny<CancellationToken>()),
            Times.Never);
    }
}
