using System.Diagnostics;

namespace MusicEco.Global.AbstractLayers;
public static class File {
    public static string Path(string path) {
        return path;
    }
    public static string Join(params string[] paths) {
        return System.IO.Path.Join(paths);
    }
    public static bool Exists(string path) {
        return System.IO.File.Exists(path);
    }
    public static void CreateDirectoryIfNotExist(string path) {
        if (!System.IO.Directory.Exists(path)) {
            System.IO.Directory.CreateDirectory(path);
        }
    }
    public static bool Move(string oldPath, string newPath) {
        try {
            System.IO.File.Move(oldPath, newPath);
            return true;
        }
        catch (Exception e) {
            Debug.WriteLine("-!-!- Abstract file access error" + e.Message);
            return false;
        }
    }
    public static bool Delete(string path) {
        try {
            System.IO.File.Delete(path);
            return true;
        }
        catch (Exception e) {
            Debug.WriteLine("-!-!- Abstract file access error" + e.Message);
            return false;
        }
    }
    public static string GetPersPath(string path) {
        string result = System.IO.Path.Combine(FileSystem.AppDataDirectory, path);
        return result;
    }
    public static string GetPersPath() {
        return FileSystem.AppDataDirectory;
    }
    public static string GetCacheImagePath(string name) {
        string folderPath = GetPersPath("cachedImage");
        if (!Directory.Exists(folderPath)) {
            Directory.CreateDirectory(folderPath);
        }
        return System.IO.Path.Combine(folderPath, name + ".png");
    }
    public static void SaveImage(ref byte[] image, string filePath) {
        System.IO.File.WriteAllBytes(filePath, image);
    }
    public static string TokenPath() {
        return GetPersPath("token");
    }
    public static string ReadAllText(string path) {
        return System.IO.File.ReadAllText(path);
    }
    public static byte[] ReadAllBytes(string path) {
        return System.IO.File.ReadAllBytes(path);
    }
    public static List<string> GetFiles(string directoryPath) {
        var filePaths = Directory.GetFiles(directoryPath);
        List<string> result = [.. filePaths];
        return result;
    }
    public static List<string> GetDirectories(string directoryPath) {
        var directoryPaths = Directory.GetDirectories(directoryPath);
        List<string> result = [.. directoryPaths];
        return result;
    }
    public static bool Exits(string path) {
        return System.IO.Path.Exists(path);
    }
    public static string? GetDirectoryName(string path) {
        return System.IO.Path.GetDirectoryName(path);
    }
    public static string GetExtension(string path) {
        return System.IO.Path.GetExtension(path);
    }
    public static string GetFileNameWithouExtension(string path) {
        return System.IO.Path.GetFileNameWithoutExtension(path);
    }
}