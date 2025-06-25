using System.Diagnostics;
using System.Reflection;

namespace Domain.EventSystem;
[AttributeUsage(AttributeTargets.Method)]
public class StaticInitializerAttribute(int priority = 0) : Attribute {
    public int Priority = priority;
    private static bool _called = false;
    public static void StaticInitialize(List<Type> residentTypes) {
        if (_called) {
            throw new InvalidOperationException("Method is aldready called");
        }
        _called = true;
        List<MethodInfo> initilizerList = [];
        Stopwatch sw = Stopwatch.StartNew();
        foreach (var residentType in residentTypes) {
            Assembly? assembly = Assembly.GetAssembly(residentType);
            if (assembly != null) {
                var types = assembly.GetTypes();
                foreach (var type in types) {
                    var methods = type.GetMethods(BindingFlags.Static | BindingFlags.Public | BindingFlags.NonPublic).
                                        Where(m => m.GetCustomAttributes(typeof(StaticInitializerAttribute), false).Length > 0);
                    foreach (var method in methods) {
                        initilizerList.Add(method);
                    }
                }
            }
        }

        Dictionary<int, List<MethodInfo>> methodsPriority = [];
        foreach (var initializer in initilizerList) {
            var priority = (initializer.GetCustomAttributes()
                .Where(a => a.GetType() == typeof(StaticInitializerAttribute))
                .ToList()[0] as StaticInitializerAttribute)!.Priority;
            if (methodsPriority.ContainsKey(priority)) {
                methodsPriority[priority].Add(initializer);
            }
            else {
                methodsPriority.Add(priority, [initializer]);
            }
        }
        List<int> priorities = methodsPriority.Keys.ToList();
        for (var i = priorities.Count - 1; i >= 0; i--) {
            Debug.WriteLine($"Priority {i}");
            var priority = priorities[i];
            var methods = methodsPriority[priority];
            foreach (var method in methods) {
                Debug.WriteLine($"---Calling {method.DeclaringType?.Name} {method.Name}");
                method.Invoke(null, null);
            }
        }
        initilizerList.Clear();
        Debug.WriteLine($"StaticInitializer running cost {sw.ElapsedMilliseconds} ms");
    }
}