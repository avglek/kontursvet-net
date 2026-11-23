using Kontursvet.Domain.Common;
using Kontursvet.Domain.Entities;
using Kontursvet.Infrastructure.Messaging;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Logging.Abstractions;

namespace Kontursvet.Api.Tests.Infrastructure.Messaging;

/// <summary>
/// Тесты <see cref="NoOpMessageDispatcher"/>: заглушка всегда возвращает успех
/// и пишет в лог имя/телефон/число вложений, не обращаясь ни к какому мессенджеру.
/// </summary>
public sealed class NoOpMessageDispatcherTests
{
    private static Lead SampleLead() => new()
    {
        Name = "Иван",
        Phone = new LeadPhone { Digital = "79991234567", Format = "+7(999)123-45-67" },
        Home = "Коттедж",
        Location = "Ленинградская область",
        Message = "Фасадная подсветка",
        Estimate = "120 000 ₽"
    };

    // ==================== Результат ====================

    [Fact]
    public async Task Dispatch_WithoutAttachments_ReturnsSuccess()
    {
        var dispatcher = new NoOpMessageDispatcher(NullLogger<NoOpMessageDispatcher>.Instance);

        var result = await dispatcher.DispatchAsync(SampleLead(), [], CancellationToken.None);

        Assert.True(result.IsSuccess);
        Assert.Null(result.Error);
    }

    [Fact]
    public async Task Dispatch_WithAttachments_ReturnsSuccess()
    {
        var dispatcher = new NoOpMessageDispatcher(NullLogger<NoOpMessageDispatcher>.Instance);
        var attachments = new[]
        {
            new LeadAttachment { Filename = "a.jpg", Token = "token-a", ContentType = "image/jpeg" },
            new LeadAttachment { Filename = "b.png", Token = null, ContentType = null }
        };

        var result = await dispatcher.DispatchAsync(SampleLead(), attachments, CancellationToken.None);

        Assert.True(result.IsSuccess);
    }

    [Fact]
    public async Task Dispatch_CancellationRequested_DoesNotThrow()
    {
        // Заглушка ничего не ждёт в сети, поэтому токен отмены ей не страшен
        var dispatcher = new NoOpMessageDispatcher(NullLogger<NoOpMessageDispatcher>.Instance);
        using var cts = new CancellationTokenSource();
        cts.Cancel();

        var result = await dispatcher.DispatchAsync(SampleLead(), [], cts.Token);

        Assert.True(result.IsSuccess);
    }

    [Theory]
    [InlineData(0)]
    [InlineData(1)]
    [InlineData(30)]
    public async Task Dispatch_AnyNumberOfAttachments_Succeeds(int count)
    {
        var dispatcher = new NoOpMessageDispatcher(NullLogger<NoOpMessageDispatcher>.Instance);
        var attachments = Enumerable.Range(0, count)
            .Select(i => new LeadAttachment { Filename = $"f{i}.jpg", Token = $"t{i}" })
            .ToList();

        var result = await dispatcher.DispatchAsync(SampleLead(), attachments, CancellationToken.None);

        Assert.True(result.IsSuccess);
    }

    // ==================== Содержимое лога ====================

    [Fact]
    public async Task Dispatch_LogsNamePhoneAndAttachmentCount()
    {
        var logger = new CollectingLogger<NoOpMessageDispatcher>();
        var dispatcher = new NoOpMessageDispatcher(logger);
        var attachments = new[]
        {
            new LeadAttachment { Filename = "a.jpg", Token = "token-a" },
            new LeadAttachment { Filename = "b.jpg", Token = "token-b" },
            new LeadAttachment { Filename = "c.jpg", Token = null }
        };

        await dispatcher.DispatchAsync(SampleLead(), attachments, CancellationToken.None);

        var entry = Assert.Single(logger.Entries);
        Assert.Equal(LogLevel.Information, entry.Level);
        Assert.Null(entry.Exception);
        Assert.Equal("Иван", entry.Values["Name"]);
        Assert.Equal("79991234567", entry.Values["Phone"]);   // цифровое значение, не формат
        Assert.Equal(3, entry.Values["Count"]);               // считаются все вложения, даже без токена
    }

    [Fact]
    public async Task Dispatch_LogsEmptyPhoneForBareLead()
    {
        var logger = new CollectingLogger<NoOpMessageDispatcher>();
        var dispatcher = new NoOpMessageDispatcher(logger);

        await dispatcher.DispatchAsync(new Lead { Name = "Пётр" }, [], CancellationToken.None);

        var entry = Assert.Single(logger.Entries);
        Assert.Equal("Пётр", entry.Values["Name"]);
        Assert.Equal(string.Empty, entry.Values["Phone"]);
        Assert.Equal(0, entry.Values["Count"]);
    }

    // ==================== Фейк логгера ====================

    private sealed record CollectedLog(
        LogLevel Level,
        IReadOnlyDictionary<string, object?> Values,
        Exception? Exception);

    /// <summary>
    /// Собирает записи лога вместе с именованными значениями шаблона, чтобы тесты
    /// проверяли данные, а не отформатированную строку.
    /// </summary>
    private sealed class CollectingLogger<T> : ILogger<T>
    {
        public List<CollectedLog> Entries { get; } = [];

        public IDisposable? BeginScope<TState>(TState state) where TState : notnull => null;

        public bool IsEnabled(LogLevel logLevel) => true;

        public void Log<TState>(
            LogLevel logLevel,
            EventId eventId,
            TState state,
            Exception? exception,
            Func<TState, Exception?, string> formatter)
        {
            var values = new Dictionary<string, object?>(StringComparer.Ordinal);

            // state — это IReadOnlyList<KeyValuePair<string, object?>>: аргументы + {OriginalFormat}
            if (state is IReadOnlyList<KeyValuePair<string, object?>> pairs)
            {
                foreach (var pair in pairs)
                    if (pair.Key != "{OriginalFormat}")
                        values[pair.Key] = pair.Value;
            }

            Entries.Add(new CollectedLog(logLevel, values, exception));
        }
    }
}
