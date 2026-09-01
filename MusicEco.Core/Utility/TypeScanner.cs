using System.Reflection;
namespace MusicEco.Core.Utility;

public static class TypeScanner {
    public static readonly List<Assembly> Assemblies = [];
    public static List<Type> FindImplementations<TType>() {
        List<Type> result = [];
        foreach (var assembly in Assemblies) {
            result.AddRange(assembly.GetTypes()
            .Where(t => typeof(TType).IsAssignableFrom(t) && t.IsClass && !t.IsAbstract));
        }
        return result;
    }
}