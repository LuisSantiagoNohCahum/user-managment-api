using ApiUsers.Models.Requests;

namespace ApiUsers.Interfaces
{
    public interface ILoginService
    {
        Task<string> LoginAsync(LoginRequest request, CancellationToken cancellationToken);
    }
}
