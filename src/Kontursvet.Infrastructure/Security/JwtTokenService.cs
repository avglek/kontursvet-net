using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using Kontursvet.Application.Abstractions.Users;
using Kontursvet.Domain.Entities;
using Microsoft.Extensions.Configuration;
using Microsoft.IdentityModel.Tokens;

namespace Kontursvet.Infrastructure.Security;

public sealed class JwtTokenService(IConfiguration config) : IJwtTokenService
{
    public string CreateToken(AdminUser user)
    {
        var secretKey = config["Jwt:SecretKey"]
            ?? throw new InvalidOperationException("Jwt:SecretKey не задан");
        var issuer = config["Jwt:Issuer"] ?? "kontursvet";
        var audience = config["Jwt:Audience"] ?? "kontursvet-admin";

        var claims = new List<Claim>
        {
            new(JwtRegisteredClaimNames.Sub, user.Id.ToString()),
            new(JwtRegisteredClaimNames.UniqueName, user.Username),
            new(ClaimTypes.Role, user.Role),
            new(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString())
        };

        var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(secretKey));
        var creds = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);

        var token = new JwtSecurityToken(
            issuer: issuer,
            audience: audience,
            claims: claims,
            expires: DateTime.UtcNow.AddHours(8),
            signingCredentials: creds);

        return new JwtSecurityTokenHandler().WriteToken(token);
    }
}