namespace Kontursvet.Application.Abstractions.Users;

public interface IPasswordHasher
{
    string Hash(string password);
    bool Verify(string password, string hash);
}