namespace DataStorage; 
internal static class Flags {
    internal static bool AutoSave = true;
    internal static bool Running = true;
    internal static bool GlobalChanged = false;
    internal static readonly object PersistanceLock = new();
    internal static bool IsLocked() {
        return Utility.IsLocked(PersistanceLock);
    }
    internal static object AcquireLock() {
        return PersistanceLock;
    }
}
