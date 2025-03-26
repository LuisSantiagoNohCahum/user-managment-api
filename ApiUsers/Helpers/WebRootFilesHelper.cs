namespace ApiUsers.Helpers
{
    /// <summary>
    /// Exposes methods to manage internal static files and the access from external downoad action.
    /// </summary>
    public class WebRootFilesHelper : IWebRootFilesHelper
    {
        private readonly string _webRootPath;
        private readonly string _apiUrl;
        public WebRootFilesHelper(IWebHostEnvironment webHostEnvironment, IConfiguration configuration)
        {
            _apiUrl = configuration.GetValue<string>("ApiUrl") ?? "";
            _webRootPath = webHostEnvironment.WebRootPath;

            _apiUrl.Guard(nameof(_apiUrl), "The ApiUrl Key not configured in the environment.");
        }

        private string GetUploadsRootFolder() => Path.Combine(_webRootPath, "Uploads");

        private string GetPrivateWebFolder() => _apiUrl.EndsWith("/") ? string.Format("{0}{1}", "Private") : string.Format("{0}/{1}", "Private");

        public async Task<string> GetFileUrlAsync(string fileContent, string fileName, bool isBase64 = false, CancellationToken cancellationToken = default)
            => await GetFileUrlAsync(fileContent, fileName, GetUploadsRootFolder(), isBase64, cancellationToken);

        public async Task<string> GetFileUrlAsync(string fileContent, string fileName, string directory, bool isBase64, CancellationToken cancellationToken = default)
            => await GetFileUrlAsync(isBase64 ? Convert.FromBase64String(fileContent) : Encoding.UTF8.GetBytes(fileContent), fileName, directory, cancellationToken);

        public async Task<string> GetFileUrlAsync(Stream fileContent, string fileName, CancellationToken cancellationToken = default)
            => await GetFileUrlAsync(fileContent, fileName, GetUploadsRootFolder(), cancellationToken);

        public async Task<string> GetFileUrlAsync(Stream fileContent, string fileName, string directory, CancellationToken cancellationToken = default)
            => await GetFileUrlAsync(fileContent.ToByteArray(), fileName, directory, cancellationToken);

        public async Task<string> GetFileUrlAsync(byte[] fileContent, string fileName, CancellationToken cancellationToken = default)
            => await GetFileUrlAsync(fileContent, fileName, GetUploadsRootFolder(), cancellationToken);

        public async Task<string> GetFileUrlAsync(byte[] fileContent, string fileName, string directory, CancellationToken cancellationToken = default)
        {
            string filePath = await SaveFileAsync(fileContent, fileName, directory, cancellationToken);

            int indexRoot = filePath.LastIndexOf("wwwroot\\");

            filePath = filePath.AsSpan(indexRoot).ToString().Replace("\\", "/");

            return string.Format("{0}/{1}", GetPrivateWebFolder(), filePath);
        }

        public async Task<string> SaveFileAsync(string fileContent, string fileName, bool isBase64 = false, CancellationToken cancellationToken = default)
            => await SaveFileAsync(fileContent, fileName, GetUploadsRootFolder(), isBase64, cancellationToken);

        public async Task<string> SaveFileAsync(string fileContent, string fileName, string directory, bool isBase64 = false, CancellationToken cancellationToken = default)
            => await SaveFileAsync(isBase64 ? Convert.FromBase64String(fileContent) : Encoding.UTF8.GetBytes(fileContent), fileName, directory, cancellationToken);

        public async Task<string> SaveFileAsync(Stream fileContent, string fileName, CancellationToken cancellationToken = default)
            => await SaveFileAsync(fileContent, fileName, GetUploadsRootFolder(), cancellationToken);

        public async Task<string> SaveFileAsync(Stream fileContent, string fileName, string directory, CancellationToken cancellationToken = default)
            => await SaveFileAsync(fileContent.ToByteArray(), fileName, directory, cancellationToken);

        public async Task<string> SaveFileAsync(byte[] fileContent, string fileName, CancellationToken cancellationToken = default)
            => await SaveFileAsync(fileContent, fileName, GetUploadsRootFolder(), cancellationToken);

        public async Task<string> SaveFileAsync(byte[] fileContent, string fileName, string directory, CancellationToken cancellationToken = default)
        {
            if (!Directory.Exists(directory)) Directory.CreateDirectory(directory);
            
            string filePath = Path.Combine(directory, fileName);

            using (var fileStream = new FileStream(filePath, FileMode.Create, FileAccess.Write))
            {
                await fileStream.WriteAsync(fileContent, cancellationToken);
            }

            return filePath;
        }

        public async Task SaveFileOrAppendAsync(string fileContent, string fileName, CancellationToken cancellationToken = default)
            => await SaveFileOrAppendAsync(fileContent, fileName, GetUploadsRootFolder(), cancellationToken);

        public async Task SaveFileOrAppendAsync(string fileContent, string fileName, string directory, CancellationToken cancellationToken = default)
        {
            if (!Directory.Exists(directory)) Directory.CreateDirectory(directory);

            string filePath = Path.Combine(directory, fileName);

            await File.AppendAllTextAsync(filePath, fileContent, cancellationToken);
        }
    }
}
