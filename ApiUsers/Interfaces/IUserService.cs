namespace ApiUsers.Interfaces
{
    public interface IUserService
    {
        Task<bool> InsertGuestAsync(SignUpRequest request, CancellationToken cancellationToken);
        Task<UserDto> GetAsync(int id, CancellationToken cancellationToken);
        Task<IEnumerable<UserDto>> GetAllAsync(GetAllRequest request, CancellationToken cancellationToken);
        Task<int> InsertAsync(InsertRequest request, CancellationToken cancellationToken);
        Task<int> UpdateAsync(int id, UpdateRequest request, CancellationToken cancellationToken);
        Task<int> DeleteAsync(int id, CancellationToken cancellationToken);
        Task<string> ExportToExcelAsync(GetAllRequest request, CancellationToken cancellationToken);
        Task<int> ImportFromFileAsync(ImportFromFileRequest request, CancellationToken cancellationToken);
    }
}
