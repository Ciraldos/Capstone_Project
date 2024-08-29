namespace Capstone.Services.Interfaces.Auth
{
    public interface IPasswordHelper
    {
        string HashPassword(string password);
        bool VerifyPassword(string password, string hashedPassword);
    }
}
