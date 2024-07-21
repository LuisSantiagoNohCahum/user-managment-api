namespace ApiUsers.Classes
{
    public class FileHelper
    {
        public static string SaveTempFile(IFormFile _file)
        {
            string filePath = string.Empty;

            return string.Empty;
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
