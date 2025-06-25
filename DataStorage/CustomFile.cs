#if ANDROID
using MusicEco.Platforms.Android;
#endif
using System.Diagnostics;

namespace DataStorage;
internal static class CustomFile {
    public static string GetPersPath(string subPath) {
        return System.IO.Path.Combine(FileSystem.AppDataDirectory, subPath);
    }
    public static bool CreateDirectoryIfNotExists(string path) {
        if (!System.IO.Directory.Exists(path)) {
            System.IO.Directory.CreateDirectory(path);
            return true;
        }
        else {
            return false;
        }
    }
    public static List<string> GetFiles(string directoryPath) {
        var filePaths = Directory.GetFiles(directoryPath);
        List<string> result = new(filePaths);
        return result;
    }
    public static List<string> GetFolders(string directoryPath) {
        var folderPath = Directory.GetDirectories(directoryPath);
        List<string> result = new(folderPath);
        return result;
    }
    public static string? GetDirectoryName(string path) {
        return System.IO.Path.GetDirectoryName(path);
    }
    public static string GetExtension(string path) {
        return System.IO.Path.GetExtension(path);
    }
    public static string GetFileNameWithoutExtension(string path) {
        return System.IO.Path.GetFileNameWithoutExtension(path);
    }
    public static string Join(string path1, string path2) {
        return System.IO.Path.Join(path1, path2);
    }
    public static bool Move(string fromPath, string toPath) {
        try {
            System.IO.File.Move(fromPath, toPath);
            return true;
        }
        catch (Exception e) {
            Debug.WriteLine("-!-!- File access error" + e.Message);
            return false;
        }
    }
    public static bool Exists(string path) {
        bool exists = System.IO.File.Exists(path);
        if (!exists) {
            exists = Directory.Exists(path);
        }
        return exists;
    }
    public static bool Delete(string path) {
        try {
            System.IO.File.Delete(path);
            return true;
        }
        catch (Exception e) {
            Debug.WriteLine("-!-!- File access error" + e.Message);
            return false;
        }
    }
}
