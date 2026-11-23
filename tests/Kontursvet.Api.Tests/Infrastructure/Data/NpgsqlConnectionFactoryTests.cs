using System.Data;
using Kontursvet.Infrastructure.Data;
using Npgsql;

namespace Kontursvet.Api.Tests.Infrastructure.Data;

/// <summary>
/// Тесты <see cref="NpgsqlConnectionFactory"/>: создаёт новые экземпляры
/// <see cref="NpgsqlConnection"/> с правильной строкой подключения.
/// </summary>
public sealed class NpgsqlConnectionFactoryTests
{
    private const string ConnectionString = "Host=localhost;Port=5432;Database=test;Username=t;Password=t";

    private static NpgsqlConnectionFactory Create() => new(ConnectionString);

    // ==================== Конструктор ====================

    [Fact]
    public void Constructor_StoresConnectionString()
    {
        var factory = Create();

        // Строка подключения читается только через созданное соединение
        using var conn = factory.Create();
        Assert.Equal(ConnectionString, conn.ConnectionString);
    }

    [Theory]
    [InlineData("   ")]
    [InlineData("not-a-connection-string-at-all")]
    [InlineData("Password=only-password")]
    public void Create_MalformedConnectionString_ThrowsOnCreate(string value)
    {
        // Фабрика не валидирует строку сама — но NpgsqlConnection парсит её в конструкторе.
        // Практический вывод: опечатка в ConnectionStrings:Postgres падает при первом Create(),
        // то есть на первом запросе к БД, а не на старте приложения.
        var factory = new NpgsqlConnectionFactory(value);

        Assert.ThrowsAny<Exception>(() => factory.Create());
    }

    [Fact]
    public void Create_EmptyConnectionString_IsAllowed()
    {
        // Пустая строка Npgsql принимает; соединение с ней открыться не сможет
        using var conn = new NpgsqlConnectionFactory(string.Empty).Create();

        Assert.Equal(string.Empty, conn.ConnectionString);
        Assert.Equal(ConnectionState.Closed, conn.State);
    }

    // ==================== Create ====================

    [Fact]
    public void Create_ReturnsNpgsqlConnection()
    {
        using var conn = Create().Create();

        Assert.IsAssignableFrom<NpgsqlConnection>(conn);
    }

    [Fact]
    public void Create_InitialState_IsClosed()
    {
        using var conn = Create().Create();

        Assert.Equal(ConnectionState.Closed, conn.State);
    }

    [Fact]
    public void Create_MultipleTimes_ReturnsDifferentInstances()
    {
        var factory = Create();
        using var first = factory.Create();
        using var second = factory.Create();

        Assert.NotSame(first, second);
    }

    // ==================== Строка подключения как есть ====================

    [Fact]
    public void Create_PreservesSpecialCharactersInConnectionString()
    {
        const string special = "Host=localhost;Database=test;Username=user_name;Password=p@ss/w0rd;";

        using var conn = new NpgsqlConnectionFactory(special).Create();

        Assert.Equal(special, conn.ConnectionString);
    }
}
