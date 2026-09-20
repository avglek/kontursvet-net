using System.Data;
using Npgsql;

namespace Kontursvet.Infrastructure.Data;

public interface IDbConnectionFactory
{
    IDbConnection Create();
}

public sealed class NpgsqlConnectionFactory(string connectionString) : IDbConnectionFactory
{
    public IDbConnection Create() => new NpgsqlConnection(connectionString);
}