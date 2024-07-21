namespace ApiUsers.Classes
{
    public class FileHelper
    {
        public static async Task<string> SaveTempFile(IFormFile _file)
        {
            try
            {
                string saveFolder = System.IO.Path.Combine(Environment.CurrentDirectory, "Uploads");
                //mover validacion a excel class
                string[] validExtentions = new[] { ".xlsx", ".xls" };

                string fileExtention = Path.GetExtension(_file.FileName);
                if (!validExtentions.Contains(fileExtention))
                    throw new Exception("La extencion del archivo no es valida.");
                if (_file.Length > (5 * 1024 * 1024))
                    throw new Exception("El tamaño del archivo debe ser inferior a 5Mb");

                string filename = Guid.NewGuid().ToString() + fileExtention;

                string fullfilepath = System.IO.Path.Combine(saveFolder, filename);

                using FileStream stream = new FileStream(fullfilepath, FileMode.Create);
                await _file.CopyToAsync(stream);

                return fullfilepath;
            }
            catch (Exception ex)
            {
                throw new Exception($"SaveTempFile: {ex.Message}");
            }
        }

        public static void DeleteTempFile(string _filepath)
        {
            if (System.IO.File.Exists(_filepath))
            {
                System.IO.File.Delete(_filepath);
            }
        }
    }
}
