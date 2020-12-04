using System.IO;

public class FileService
{
    public static bool CheckIfFileExists(string path)
    {
        return File.Exists(path);
    }
}
