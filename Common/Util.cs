
namespace MusicEco.Common;
public static class Util {
    public static string N(string fieldName) {
        String result = char.ToUpper(fieldName[0]) + fieldName.Substring(1);
        String.Intern(result);
        return result;
    }
    public static bool IsLocked(object lockObject) {
        bool lockTaken = false;
        try {
            Monitor.TryEnter(lockObject, 0, ref lockTaken);
        }
        finally {
            if (lockTaken) Monitor.Exit(lockObject);
        }
        return !lockTaken;
    }

}
