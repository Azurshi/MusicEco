using Microsoft.CodeAnalysis;

namespace MusicEco.Generators.SourceGenerators;

internal class BindedPropertyGenerator: IPropertyBasedGenerator {
    public string AttributeMetadataName => "BindedPropertyAttribute";
    public string AttributeClassDefinition => """
        [global::System.AttributeUsage(global::System.AttributeTargets.Property)]
        public sealed class BindedPropertyAttribute: global::System.Attribute;
        """;
    public GeneratedSource? GetSource(ISymbol classSymbol, GeneratorAttributeSyntaxContext context, IPropertySymbol property) {
        var propertyType = Utility.ToDisplayString(property.Type);
        string access = Utility.GetAccessibility(property.DeclaredAccessibility);
        string staticFieldName = property.Name + "Property";
        string header = "";
        string content = $$"""
                {{access}} partial {{propertyType}} {{property.Name}} {
                    get => ({{propertyType}})GetValue({{staticFieldName}});
                    set => SetValue({{staticFieldName}}, value);
                }
            """;
        return new(header, content);
    }
}
