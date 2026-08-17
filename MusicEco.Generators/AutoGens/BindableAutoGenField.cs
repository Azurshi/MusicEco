using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp.Syntax;

namespace MusicEco.Generators.AutoGens;

internal class BindableAutoGenField: ISourceGenField {
    public string AttributeName => "BindableAutoGenAttribute";
    public string AttributeMetadataName => "BindableAutoGenAttribute`1";
    public string AttributeClassDefinition => $$"""
        [global::System.AttributeUsage(global::System.AttributeTargets.Field)]
        public sealed class BindableAutoGenAttribute<T>: global::System.Attribute {
            public bool IsNullable {get; set;} = false;
        }
        """;
    public GeneratedSource? GetSource(string className, GeneratorAttributeSyntaxContext context, IFieldSymbol field) {
        string shortPropertyName = field.Name.Replace("Property", "");
        ITypeSymbol? extra = GetBindableValueType(context);
        if (extra != null) {
            string extraFieldType = extra.ToDisplayString(
                SymbolDisplayFormat.FullyQualifiedFormat
                .WithMiscellaneousOptions(
                SymbolDisplayMiscellaneousOptions.IncludeNullableReferenceTypeModifier
                | SymbolDisplayMiscellaneousOptions.EscapeKeywordIdentifiers));
            string access = Utility.GetAccessibility(field.DeclaredAccessibility);
            string valueTypeArgument = string.Empty;
            if (GetIsNullable(context)) {
                valueTypeArgument = "?";
            }
            string header = "";
            string content = $$"""
                {{access}} {{extraFieldType}}{{valueTypeArgument}} {{shortPropertyName}} {
                    get => ({{extraFieldType}}{{valueTypeArgument}})GetValue({{field.Name}});
                    set => SetValue({{field.Name}}, value);
                }
            """;
            return new(header, content);
        } else {
            return new("", "ERROR");
        }
    }
    private static ITypeSymbol? GetBindableValueType(GeneratorAttributeSyntaxContext context) {
        return context.Attributes[0].AttributeClass is INamedTypeSymbol attribute
            && attribute.TypeArguments.Length == 1
            ? attribute.TypeArguments[0]
            : null;
    }
    private static bool GetIsNullable(GeneratorAttributeSyntaxContext context) {
        foreach(var argument in context.Attributes[0].NamedArguments) {
            if (argument.Key == "IsNullable"
                && argument.Value.Value is bool isNullable) {
                return isNullable;
            }
        }
        return false;
    }
}
