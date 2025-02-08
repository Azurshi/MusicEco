using System.Diagnostics;
using System.Reflection;

namespace MusicEco.Global.Attributes;
[AttributeUsage(AttributeTargets.Method)]
public class StaticInitializerAttribute(int priority = 0) : Attribute {
    public int Priority = priority;
    public readonly static List<MethodInfo> InitializerList = [];
    static StaticInitializerAttribute() {
        Stopwatch sw = Stopwatch.StartNew();
        var types = Assembly.GetExecutingAssembly().GetTypes();
        foreach (var type in types) {
            var methods = type.GetMethods(BindingFlags.Static | BindingFlags.Public | BindingFlags.NonPublic).
                                Where(m => m.GetCustomAttributes(typeof(StaticInitializerAttribute), false).Length > 0);
            foreach (var method in methods) {
                InitializerList.Add(method);
            }
        }
        Debug.WriteLine($"StaticInitializer collection cost {sw.ElapsedMilliseconds} ms");
    }
    public static void Dispose() {
        InitializerList.Clear();
    }
}
