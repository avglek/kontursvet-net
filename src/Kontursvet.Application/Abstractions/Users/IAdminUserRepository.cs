using Kontursvet.Domain.Entities;

namespace Kontursvet.Application.Abstractions.Users;

public interface IAdminUserRepository
{
    Task<AdminUser?> GetByUsernameAsync(string username, CancellationToken ct);
    Task<AdminUser?> GetByIdAsync(long id, CancellationToken ct);
    Task<long> CreateAsync(AdminUser user, CancellationToken ct);
}