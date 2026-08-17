using Microsoft.CodeAnalysis;

namespace MusicEco.Generators.AutoGens;

internal class ObservableProperty: ISourceGenProperty {
    public string AttributeName => "ObservablePropertyAttribute";
    public string AttributeMetadataName => AttributeName;
    public string AttributeClassDefinition => $"""
        [global::System.AttributeUsage(global::System.AttributeTargets.Property)]
        internal sealed class {AttributeName}: global::System.Attribute;
        """;
    public GeneratedSource? GetSource(string className, GeneratorAttributeSyntaxContext context, IPropertySymbol property) {
        var propertyType = property.Type.ToDisplayString(
            SymbolDisplayFormat.FullyQualifiedFormat
            .WithMiscellaneousOptions(
                SymbolDisplayMiscellaneousOptions.IncludeNullableReferenceTypeModifier
                | SymbolDisplayMiscellaneousOptions.EscapeKeywordIdentifiers));
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
