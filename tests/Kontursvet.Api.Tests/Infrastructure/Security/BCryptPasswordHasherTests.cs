using Kontursvet.Infrastructure.Security;

namespace Kontursvet.Api.Tests.Infrastructure.Security;

/// <summary>
/// Тесты <see cref="BCryptPasswordHasher"/>: необратимость хэша, уникальность соли,
/// корректность проверки пароля.
/// </summary>
public class BCryptPasswordHasherTests
{
    private readonly BCryptPasswordHasher _hasher = new();

    [Fact]
    public void Hash_ReturnsDifferentValueFromInput()
    {
        var hash = _hasher.Hash("secret123");

        Assert.NotEqual("secret123", hash);
        Assert.StartsWith("$2", hash); // формат BCrypt
        Assert.Equal(60, hash.Length);  // стандартная длина
    }

    [Fact]
    public void Hash_SamePasswordTwice_ProducesDifferentHashes()
    {
        var h1 = _hasher.Hash("same");
        var h2 = _hasher.Hash("same");

        Assert.NotEqual(h1, h2); // разная соль каждый раз
    }

    [Fact]
    public void Verify_CorrectPassword_ReturnsTrue()
    {
        var hash = _hasher.Hash("MyP@ssw0rd");

        Assert.True(_hasher.Verify("MyP@ssw0rd", hash));
    }

    [Fact]
    public void Verify_WrongPassword_ReturnsFalse()
    {
        var hash = _hasher.Hash("correct");

        Assert.False(_hasher.Verify("wrong", hash));
    }

    [Theory]
    [InlineData("")]
    [InlineData("   ")]
    [InlineData("admin123")]
    [InlineData("Str0ng-Pa55-w0rd")]
    [InlineData("ПарольНаРусском")]
    public void Hash_Verify_RoundTrip_Works(string password)
    {
        var hash = _hasher.Hash(password);

        Assert.True(_hasher.Verify(password, hash));
    }

    [Fact]
    public void Verify_CrossHash_ReturnsFalse()
    {
        var hash1 = _hasher.Hash("password1");
        var hash2 = _hasher.Hash("password2");

        Assert.False(_hasher.Verify("password1", hash2));
        Assert.False(_hasher.Verify("password2", hash1));
    }
}
