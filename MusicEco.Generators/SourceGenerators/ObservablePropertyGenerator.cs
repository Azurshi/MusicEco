using Microsoft.CodeAnalysis;

namespace MusicEco.Generators.SourceGenerators;

internal class ObservablePropertyGenerator : IPropertyBasedGenerator{
    public string AttributeMetadataName => "ObservablePropertyAttribute";
    public string AttributeClassDefinition => """
        [global::System.AttributeUsage(global::System.AttributeTargets.Property)]
        public sealed class ObservablePropertyAttribute: global::System.Attribute;
        """;
    public GeneratedSource? GetSource(ISymbol classSymbol, GeneratorAttributeSyntaxContext context, IPropertySymbol property) {
        var propertyType = Utility.ToDisplayString(property.Type);
        string access = Utility.GetAccessibility(property.DeclaredAccessibility);
        string header = "";
        string content = $$"""
                {{access}} partial {{propertyType}} {{property.Name}} {
                    get => field;
                    set {
                        if (field != value) {
                            field = value;
                            OnPropertyChanged();
                        }
                    }
                }
            """;
        return new(header, content);
    }
}
