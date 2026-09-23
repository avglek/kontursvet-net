using Kontursvet.Application.Abstractions.Users;
using Kontursvet.Domain.Common;

namespace Kontursvet.Application.Features.Auth;

public sealed record LoginCommand(string Username, string Password);

public sealed record LoginResult(string AccessToken, string Username, string Role);

public sealed class LoginHandler(
    IAdminUserRepository repository,
    IPasswordHasher hasher,
    IJwtTokenService jwt)
{
    public async Task<Result<LoginResult>> HandleAsync(LoginCommand cmd, CancellationToken ct)
    {
        if (string.IsNullOrWhiteSpace(cmd.Username) || string.IsNullOrWhiteSpace(cmd.Password))
            return Result<LoginResult>.Failure("Логин и пароль обязательны", "AUTH_CREDENTIALS_REQUIRED");

        var user = await repository.GetByUsernameAsync(cmd.Username, ct);
        if (user is null || String.IsNullOrEmpty(user.PasswordHash))
            return Result<LoginResult>.Failure("Неверный логин или пароль", "AUTH_INVALID");

        if (!hasher.Verify(cmd.Password, user.PasswordHash))
            return Result<LoginResult>.Failure("Неверный логин или пароль", "AUTH_INVALID");

        var token = jwt.CreateToken(user);
        return Result<LoginResult>.Success(new LoginResult(token, user.Username, user.Role));
    }
}