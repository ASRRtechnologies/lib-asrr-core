using System.IO;

namespace ASRR.Core.File.Service
{
    public static class FileValidator
    {
        public static bool IsValidPath(string path)
        {
            return !string.IsNullOrEmpty(path) && path.Length <= 260 &&
                   !path.EndsWith(".") && !path.EndsWith(" ") && !path.EndsWith("\\") &&
                   !path.EndsWith("/") && !path.EndsWith(":") && !path.EndsWith("*") &&
                   !path.EndsWith("?") && !path.EndsWith("\"") && !path.EndsWith("<") &&
                   !path.EndsWith(">") && !path.EndsWith("|");
        }
        
        public static bool EnsureFolderExists(string folderPath)
        {
            if (!Directory.Exists(folderPath))
            {
                Directory.CreateDirectory(folderPath);
            }
            return true;
        }
    }
}