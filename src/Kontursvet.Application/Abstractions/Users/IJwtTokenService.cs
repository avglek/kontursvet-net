using Kontursvet.Domain.Entities;

namespace Kontursvet.Application.Abstractions.Users;

public interface IJwtTokenService
{
    string CreateToken(AdminUser user);
}