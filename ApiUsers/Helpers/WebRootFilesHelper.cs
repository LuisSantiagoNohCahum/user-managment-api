namespace ApiUsers.Helpers
{
    /// <summary>
    /// Exposes methods to manage internal static files and the access from external downoad action.
    /// </summary>
    public class WebRootFilesHelper : IWebRootFilesHelper
    {
        //TODO. Enable append mode if exist text file, only in text conetent
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

        public async Task<string> GetFileUrlAsync(string fileContent, string fileName, bool isBase64 = false, CancellationToken ct = default)
            => await GetFileUrlAsync(fileContent, fileName, GetUploadsRootFolder(), isBase64, ct);

        public async Task<string> GetFileUrlAsync(string fileContent, string fileName, string directory, bool isBase64, CancellationToken ct = default)
            => await GetFileUrlAsync(isBase64 ? Convert.FromBase64String(fileContent) : Encoding.UTF8.GetBytes(fileContent), fileName, directory, ct);

        public async Task<string> GetFileUrlAsync(Stream fileContent, string fileName, CancellationToken ct = default)
            => await GetFileUrlAsync(fileContent, fileName, GetUploadsRootFolder(), ct);

        public async Task<string> GetFileUrlAsync(Stream fileContent, string fileName, string directory, CancellationToken ct = default)
            => await GetFileUrlAsync(fileContent.ToByteArray(), fileName, directory, ct);

        public async Task<string> GetFileUrlAsync(byte[] fileContent, string fileName, CancellationToken ct = default)
            => await GetFileUrlAsync(fileContent, fileName, GetUploadsRootFolder(), ct);

        public async Task<string> GetFileUrlAsync(byte[] fileContent, string fileName, string directory, CancellationToken ct = default)
        {
            string filePath = await SaveFileAsync(fileContent, fileName, directory, ct);

            int indexRoot = filePath.LastIndexOf("wwwroot\\");

            filePath = filePath.AsSpan(indexRoot).ToString().Replace("\\", "/");

            return string.Format("{0}/{1}", GetPrivateWebFolder(), filePath);
        }

        public async Task<string> SaveFileAsync(string fileContent, string fileName, bool isBase64 = false, CancellationToken ct = default)
            => await SaveFileAsync(fileContent, fileName, GetUploadsRootFolder(), isBase64, ct);

        public async Task<string> SaveFileAsync(string fileContent, string fileName, string directory, bool isBase64 = false, CancellationToken ct = default)
            => await SaveFileAsync(isBase64 ? Convert.FromBase64String(fileContent) : Encoding.UTF8.GetBytes(fileContent), fileName, directory, ct);

        public async Task<string> SaveFileAsync(Stream fileContent, string fileName, CancellationToken ct = default)
            => await SaveFileAsync(fileContent, fileName, GetUploadsRootFolder(), ct);

        public async Task<string> SaveFileAsync(Stream fileContent, string fileName, string directory, CancellationToken ct = default)
            => await SaveFileAsync(fileContent.ToByteArray(), fileName, directory, ct);

        public async Task<string> SaveFileAsync(byte[] fileContent, string fileName, CancellationToken ct = default)
            => await SaveFileAsync(fileContent, fileName, GetUploadsRootFolder(), ct);

        public async Task<string> SaveFileAsync(byte[] fileContent, string fileName, string directory, CancellationToken ct = default)
        {
            if (!Directory.Exists(directory)) Directory.CreateDirectory(directory);
            
            string filePath = Path.Combine(directory, fileName);

            using (var fileStream = new FileStream(filePath, FileMode.Create, FileAccess.Write))
            {
                await fileStream.WriteAsync(fileContent, ct);
            }

            return filePath;
        }

        public async Task SaveFileOrAppendAsync(string fileContent, string fileName, string directory, CancellationToken ct = default)
        {
            if (!Directory.Exists(directory)) Directory.CreateDirectory(directory);

            string filePath = Path.Combine(directory, fileName);

            await File.AppendAllTextAsync(filePath, fileContent, ct);
        }
    }
}
