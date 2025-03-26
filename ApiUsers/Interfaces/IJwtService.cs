using ApiUsers.Models;

namespace ApiUsers.Interfaces
{
    public interface IJwtService
    {
        Task<string> Generate(User user);
    }
}
