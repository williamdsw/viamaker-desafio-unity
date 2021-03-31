using System.IO;

namespace Service
{
    public class FileService
    {
        public static bool CheckIfFileExists(string path)
        {
            return File.Exists(path);
        }
    }
}