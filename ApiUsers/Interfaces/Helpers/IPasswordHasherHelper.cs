namespace ApiUsers.Interfaces.Helpers
{
    public interface IPasswordHasherHelper
    {
        string HashPassword(string plainText);
        bool VerifyHashedPassword(string plainPassword, string hashPassword);
    }
}
