using ApiUsers.Models;
using ApiUsers.Models.Dtos;
using ApiUsers.Models.Requests;
using ApiUsers.Models.Requests.Uers;
using ApiUsers.Models.Responses.Users;
using Microsoft.AspNetCore.Mvc;

namespace ApiUsers.Interfaces
{
    public interface IUserService
    {
        Task<int> DeleteAsync(int id, CancellationToken ct);
        Task<FileStreamResult> ExportToExcelAsync(CancellationToken ct);
        Task<IEnumerable<UserDto>> GetAllAsync(GetAllRequest request, CancellationToken ct);
        Task<UserDto?> GetAsync(int id, CancellationToken ct);
        Task<int> ImportFromExcelAsync(IFormFile layoutRequest, CancellationToken ct);
        Task<int> InsertAsync(InsertRequest request, CancellationToken ct);
        Task<bool> SignUpAsync(SignUpRequest request, CancellationToken ct);
        Task<int> UpdateAsync(UpdateRequest request, CancellationToken ct);
    }
}
