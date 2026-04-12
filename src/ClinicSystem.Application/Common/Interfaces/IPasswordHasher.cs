namespace ClinicSystem.Application.Common.Interfaces;

public interface IPasswordHasher
{
    string HashPassword(string password);
    bool Verify(string password, string hash);
}
