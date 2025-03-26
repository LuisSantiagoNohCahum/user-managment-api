
namespace ApiUsers.Interfaces.Helpers
{
    public interface IWebRootFilesHelper
    {
        Task<string> GetFileUrlAsync(string fileContent, string fileName, bool isBase64 = false, CancellationToken cancellationToken = default);
        Task<string> GetFileUrlAsync(string fileContent, string fileName, string directory, bool isBase64, CancellationToken cancellationToken = default);
        Task<string> GetFileUrlAsync(Stream fileContent, string fileName, CancellationToken cancellationToken = default);
        Task<string> GetFileUrlAsync(Stream fileContent, string fileName, string directory, CancellationToken cancellationToken = default);
        Task<string> GetFileUrlAsync(byte[] fileContent, string fileName, CancellationToken cancellationToken = default);
        Task<string> GetFileUrlAsync(byte[] fileContent, string fileName, string directory, CancellationToken cancellationToken = default);
        Task<string> SaveFileAsync(string fileContent, string fileName, bool isBase64 = false, CancellationToken cancellationToken = default);
        Task<string> SaveFileAsync(string fileContent, string fileName, string directory, bool isBase64 = false, CancellationToken cancellationToken = default);
        Task<string> SaveFileAsync(Stream fileContent, string fileName, CancellationToken cancellationToken = default);
        Task<string> SaveFileAsync(Stream fileContent, string fileName, string directory, CancellationToken cancellationToken = default);
        Task<string> SaveFileAsync(byte[] fileContent, string fileName, CancellationToken cancellationToken = default);
        Task<string> SaveFileAsync(byte[] fileContent, string fileName, string directory, CancellationToken cancellationToken = default);
        Task SaveFileOrAppendAsync(string fileContent, string fileName, CancellationToken cancellationToken = default);
        Task SaveFileOrAppendAsync(string fileContent, string fileName, string directory, CancellationToken cancellationToken = default);
    }
}
