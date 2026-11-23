using Kontursvet.Application.Features.Auth;
using Kontursvet.Application.Abstractions.Users;
using Kontursvet.Domain.Entities;
using Moq;

namespace Kontursvet.Application.Tests.Features.Auth;

/// <summary>
/// Тесты <see cref="LoginHandler"/>: проверка пустых полей, NotFound, неверный пароль, успешный вход.
/// </summary>
public class LoginHandlerTests
{
    [Fact]
    public async Task HandleAsync_EmptyUsername_ReturnsFailure()
    {
        // Arrange
        var repo = new Mock<IAdminUserRepository>();
        var hasher = new Mock<IPasswordHasher>();
        var jwt = new Mock<IJwtTokenService>();

        var handler = new LoginHandler(repo.Object, hasher.Object, jwt.Object);

        // Act
        var result = await handler.HandleAsync(
            new LoginCommand("", "password"), CancellationToken.None);

        // Assert
        Assert.False(result.IsSuccess);
        Assert.Equal("AUTH_CREDENTIALS_REQUIRED", result.ErrorCode);
        Assert.Equal("Логин и пароль обязательны", result.Error);
    }

    [Fact]
    public async Task HandleAsync_EmptyPassword_ReturnsFailure()
    {
        var handler = new LoginHandler(
            new Mock<IAdminUserRepository>().Object,
            new Mock<IPasswordHasher>().Object,
            new Mock<IJwtTokenService>().Object);

        var result = await handler.HandleAsync(
            new LoginCommand("admin", ""), CancellationToken.None);

        Assert.False(result.IsSuccess);
        Assert.Equal("AUTH_CREDENTIALS_REQUIRED", result.ErrorCode);
    }

    [Fact]
    public async Task HandleAsync_NullCredentials_ReturnsFailure()
    {
        var handler = new LoginHandler(
            new Mock<IAdminUserRepository>().Object,
            new Mock<IPasswordHasher>().Object,
            new Mock<IJwtTokenService>().Object);

        var result = await handler.HandleAsync(
            new LoginCommand(null!, null!), CancellationToken.None);

        Assert.False(result.IsSuccess);
        Assert.Equal("AUTH_CREDENTIALS_REQUIRED", result.ErrorCode);
    }

    [Fact]
    public async Task HandleAsync_WhitespaceCredentials_ReturnsFailure()
    {
        var handler = new LoginHandler(
            new Mock<IAdminUserRepository>().Object,
            new Mock<IPasswordHasher>().Object,
            new Mock<IJwtTokenService>().Object);

        var result = await handler.HandleAsync(
            new LoginCommand("   ", "   "), CancellationToken.None);

        Assert.False(result.IsSuccess);
        Assert.Equal("AUTH_CREDENTIALS_REQUIRED", result.ErrorCode);
    }

    [Fact]
    public async Task HandleAsync_UserNotFound_ReturnsInvalid()
    {
        // Arrange
        var repo = new Mock<IAdminUserRepository>();
        repo.Setup(r => r.GetByUsernameAsync("admin", It.IsAny<CancellationToken>()))
            .ReturnsAsync((AdminUser?)null);

        var handler = new LoginHandler(repo.Object, new Mock<IPasswordHasher>().Object, new Mock<IJwtTokenService>().Object);

        // Act
        var result = await handler.HandleAsync(
            new LoginCommand("admin", "secret"), CancellationToken.None);

        // Assert
        Assert.False(result.IsSuccess);
        Assert.Equal("AUTH_INVALID", result.ErrorCode);
    }

    [Fact]
    public async Task HandleAsync_EmptyPasswordHash_ReturnsInvalid()
    {
        // Arrange
        var user = new AdminUser { Id = 1, Username = "admin", PasswordHash = "", Role = "Admin" };
        var repo = new Mock<IAdminUserRepository>();
        repo.Setup(r => r.GetByUsernameAsync("admin", It.IsAny<CancellationToken>()))
            .ReturnsAsync(user);

        var handler = new LoginHandler(repo.Object, new Mock<IPasswordHasher>().Object, new Mock<IJwtTokenService>().Object);

        // Act
        var result = await handler.HandleAsync(
            new LoginCommand("admin", "secret"), CancellationToken.None);

        // Assert
        Assert.False(result.IsSuccess);
        Assert.Equal("AUTH_INVALID", result.ErrorCode);
    }

    [Fact]
    public async Task HandleAsync_WrongPassword_ReturnsInvalid()
    {
        // Arrange
        var user = new AdminUser { Id = 1, Username = "admin", PasswordHash = "$2a$11$h2sh", Role = "Admin" };
        var repo = new Mock<IAdminUserRepository>();
        repo.Setup(r => r.GetByUsernameAsync("admin", It.IsAny<CancellationToken>()))
            .ReturnsAsync(user);

        var hasher = new Mock<IPasswordHasher>();
        hasher.Setup(h => h.Verify("secret", user.PasswordHash)).Returns(false);

        var jwt = new Mock<IJwtTokenService>();

        var handler = new LoginHandler(repo.Object, hasher.Object, jwt.Object);

        // Act
        var result = await handler.HandleAsync(
            new LoginCommand("admin", "secret"), CancellationToken.None);

        // Assert
        Assert.False(result.IsSuccess);
        Assert.Equal("AUTH_INVALID", result.ErrorCode);
    }

    [Fact]
    public async Task HandleAsync_CorrectPassword_ReturnsToken()
    {
        // Arrange
        var user = new AdminUser { Id = 1, Username = "admin", PasswordHash = "$2a$11$h2sh", Role = "Admin" };
        var repo = new Mock<IAdminUserRepository>();
        repo.Setup(r => r.GetByUsernameAsync("admin", It.IsAny<CancellationToken>()))
            .ReturnsAsync(user);

        var hasher = new Mock<IPasswordHasher>();
        hasher.Setup(h => h.Verify("secret", user.PasswordHash)).Returns(true);

        const string expectedToken = "jwt-token-value";
        var jwt = new Mock<IJwtTokenService>();
        jwt.Setup(j => j.CreateToken(user)).Returns(expectedToken);

        var handler = new LoginHandler(repo.Object, hasher.Object, jwt.Object);

        // Act
        var result = await handler.HandleAsync(
            new LoginCommand("admin", "secret"), CancellationToken.None);

        // Assert
        Assert.True(result.IsSuccess);
        Assert.Equal(expectedToken, result.Value!.AccessToken);
        Assert.Equal("admin", result.Value!.Username);
        Assert.Equal("Admin", result.Value!.Role);
        jwt.Verify(j => j.CreateToken(user), Times.Once);
    }
}
