namespace ApiUsers.Interfaces
{
    public interface IUserService
    {
        Task<bool> SignUpAsync(SignUpRequest request, CancellationToken cancellationToken);
        Task<UserDto?> GetAsync(int id, CancellationToken cancellationToken);
        Task<IEnumerable<UserDto>> GetAllAsync(GetAllRequest request, CancellationToken cancellationToken);
        Task<int> InsertAsync(InsertRequest request, CancellationToken cancellationToken);
        Task<int> UpdateAsync(UpdateRequest request, CancellationToken cancellationToken);
        Task<int> DeleteAsync(int id, CancellationToken cancellationToken);
        Task<FileStreamResult> ExportToExcelAsync(CancellationToken cancellationToken);
        Task<int> ImportFromExcelAsync(IFormFile layoutRequest, CancellationToken cancellationToken);
    }
}
