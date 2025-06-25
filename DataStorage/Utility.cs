using System.Text.Json;

namespace DataStorage;
internal static class ListUtility {
    private static readonly Random rng = new();
    public static void Shuffle<T>(this IList<T> list) {
        int n = list.Count;
        while (n > 0) {
            n--;
            int k = rng.Next(n + 1);
            T value = list[k];
            list[k] = list[n];
            list[n] = value;
        }
    }
}
internal static class Utility {
    internal static bool IsLocked(object lockObject) {
        bool lockTaken = false;
        try {
            Monitor.TryEnter(lockObject, 0, ref lockTaken);
        }
        finally {
            if (lockTaken) Monitor.Exit(lockObject);
        }
        return !lockTaken;
    }

    internal static string ConvertFilePathToOld(string filePath) {
        string directory = Path.GetDirectoryName(filePath) ?? "";
        string fileName = Path.GetFileNameWithoutExtension(filePath);
        string extension = Path.GetExtension(filePath);
        string newFileName = $"{fileName}Old{extension}";
        string newPath = Path.Combine(directory, newFileName);
        return newPath;
    }
    internal static bool RenameFile(string oldPath, string newPath) {
        if (CustomFile.Exists(oldPath)) {
            return CustomFile.Move(oldPath, newPath);
        }
        else {
            return false;
        }
    }
    internal static bool DeleteFile(string filePath) {
        return CustomFile.Delete(filePath);
    }
}
