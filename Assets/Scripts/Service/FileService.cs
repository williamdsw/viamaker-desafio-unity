using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;

public class FileService
{
    public static bool CheckIfFileExists(string path)
    {
        return File.Exists(path);
    }
}
