using Dapper;
using Kontursvet.Application.Abstractions.Users;
using Kontursvet.Domain.Entities;
using Kontursvet.Infrastructure.Data;

namespace Kontursvet.Infrastructure.Repositories;

public sealed class AdminUserRepository(IDbConnectionFactory factory) : IAdminUserRepository
{
    public async Task<AdminUser?> GetByUsernameAsync(string username, CancellationToken ct)
    {
        using var db = factory.Create();
        const string sql = """
            SELECT id, username, password_hash, role, created_at
            FROM admin_users WHERE username = @Username;
            """;
        return await db.QuerySingleOrDefaultAsync<AdminUser>(
            new CommandDefinition(sql, new { Username = username }, cancellationToken: ct));
    }

    public async Task<AdminUser?> GetByIdAsync(long id, CancellationToken ct)
    {
        using var db = factory.Create();
        const string sql = """
            SELECT id, username, password_hash, role, created_at
            FROM admin_users WHERE id = @Id;
            """;
        return await db.QuerySingleOrDefaultAsync<AdminUser>(
            new CommandDefinition(sql, new { Id = id }, cancellationToken: ct));
    }

    public async Task<long> CreateAsync(AdminUser user, CancellationToken ct)
    {
        using var db = factory.Create();
        const string sql = """
            INSERT INTO admin_users (username, password_hash, role, created_at)
            VALUES (@Username, @PasswordHash, @Role, now())
            RETURNING id;
            """;
        return await db.ExecuteScalarAsync<long>(
            new CommandDefinition(sql, user, cancellationToken: ct));
    }
}