using ApiUsers.Models.Enums;

namespace ApiUsers.Models.Requests.Uers
{
    public class ImportFromFileRequest
    {
        public FileType Type { get; set; }
        public IFormFile LayoutFile { get; set; }
    }
}
