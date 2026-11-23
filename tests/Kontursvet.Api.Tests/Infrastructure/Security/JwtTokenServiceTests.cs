using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using Kontursvet.Domain.Entities;
using Kontursvet.Infrastructure.Security;
using Microsoft.Extensions.Configuration;

namespace Kontursvet.Api.Tests.Infrastructure.Security;

/// <summary>
/// Тесты <see cref="JwtTokenService"/>: структура токена, claims, issuer/audience,
/// требование секретного ключа.
/// </summary>
public class JwtTokenServiceTests
{
    private const string Secret = "test-secret-key-at-least-32-chars-long-ok";

    private static IConfiguration Config(Dictionary<string, string?>? overrides = null)
    {
        var data = new Dictionary<string, string?>
        {
            ["Jwt:SecretKey"] = Secret,
            ["Jwt:Issuer"] = "kontursvet",
            ["Jwt:Audience"] = "kontursvet-admin"
        };

        if (overrides is not null)
            foreach (var (k, v) in overrides)
                data[k] = v;

        return new ConfigurationBuilder().AddInMemoryCollection(data).Build();
    }

    private static AdminUser User() => new()
    {
        Id = 7,
        Username = "tester",
        Role = "Admin",
        PasswordHash = "$2a$11$dummy"
    };

    private static JwtSecurityToken Read(string token)
    {
        // MapInboundClaims=false — иначе handler переписывает короткие имена (sub/role/jti)
        // в URI-типы ClaimTypes.*, и проверять claims по коротким именам не получится
        var handler = new JwtSecurityTokenHandler { MapInboundClaims = false };
        return handler.ReadJwtToken(token);
    }

    [Fact]
    public void CreateToken_ReturnsNonEmptyJwt()
    {
        var service = new JwtTokenService(Config());

        var token = service.CreateToken(User());

        Assert.False(string.IsNullOrWhiteSpace(token));
        Assert.Equal(3, token.Split('.').Length); // header.payload.signature
    }

    [Fact]
    public void CreateToken_ContainsExpectedClaims()
    {
        var service = new JwtTokenService(Config());
        var user = User();

        var jwt = Read(service.CreateToken(user));

        Assert.Equal(user.Id.ToString(), jwt.Claims.Single(c => c.Type == JwtRegisteredClaimNames.Sub).Value);
        Assert.Equal(user.Username, jwt.Claims.Single(c => c.Type == JwtRegisteredClaimNames.UniqueName).Value);
        // Роль пишется под полным URI-типом ClaimTypes.Role (JwtSecurityTokenHandler при
        // проверке токена мапит её обратно в ClaimTypes.Role — см. AdminEndpoints /api/admin/me)
        Assert.Equal(user.Role, jwt.Claims.Single(c => c.Type == ClaimTypes.Role).Value);
        Assert.False(string.IsNullOrWhiteSpace(jwt.Claims.Single(c => c.Type == JwtRegisteredClaimNames.Jti).Value));
    }

    [Fact]
    public void CreateToken_UsesIssuerAndAudienceFromConfig()
    {
        var service = new JwtTokenService(Config());

        var jwt = Read(service.CreateToken(User()));

        Assert.Equal("kontursvet", jwt.Issuer);
        Assert.Equal("kontursvet-admin", jwt.Audiences.Single());
    }

    [Fact]
    public void CreateToken_MissingIssuerAudience_UsesDefaults()
    {
        var service = new JwtTokenService(Config(new()
        {
            ["Jwt:Issuer"] = null,
            ["Jwt:Audience"] = null
        }));

        var jwt = Read(service.CreateToken(User()));

        Assert.Equal("kontursvet", jwt.Issuer);
        Assert.Equal("kontursvet-admin", jwt.Audiences.Single());
    }

    [Fact]
    public void CreateToken_SetsExpiryAboutEightHoursAhead()
    {
        var service = new JwtTokenService(Config());

        var jwt = Read(service.CreateToken(User()));

        // exp: не в прошлом и не дальше разумного запаса (8 часов по реализации)
        Assert.True(jwt.ValidTo > DateTime.UtcNow, $"ValidTo {jwt.ValidTo:o} должен быть в будущем");
        Assert.True(jwt.ValidTo <= DateTime.UtcNow.AddHours(8).AddMinutes(1),
            $"ValidTo {jwt.ValidTo:o} не должен превышать 8 часов");
    }

    [Fact]
    public void CreateToken_UniqueJtiPerCall()
    {
        var service = new JwtTokenService(Config());
        var user = User();

        var jti1 = Read(service.CreateToken(user))
            .Claims.Single(c => c.Type == JwtRegisteredClaimNames.Jti).Value;
        var jti2 = Read(service.CreateToken(user))
            .Claims.Single(c => c.Type == JwtRegisteredClaimNames.Jti).Value;

        Assert.NotEqual(jti1, jti2);
    }

    [Fact]
    public void CreateToken_NoSecretKey_Throws()
    {
        var service = new JwtTokenService(Config(new() { ["Jwt:SecretKey"] = null }));

        var ex = Assert.Throws<InvalidOperationException>(() => service.CreateToken(User()));
        Assert.Contains("Jwt:SecretKey", ex.Message);
    }

    [Fact]
    public void CreateToken_TooShortSecret_Throws()
    {
        // HMAC-SHA256 требует ключ не короче 256 бит (32 символа)
        var service = new JwtTokenService(Config(new() { ["Jwt:SecretKey"] = "short" }));

        Assert.Throws<ArgumentOutOfRangeException>(() => service.CreateToken(User()));
    }
}
