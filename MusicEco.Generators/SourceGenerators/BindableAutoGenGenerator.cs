using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp.Syntax;

namespace MusicEco.Generators.SourceGenerators;

internal class BindableAutoGenGenerator: IFieldBasedGenerator {
    public string AttributeMetadataName => "BindableAutoGenAttribute";
    public string AttributeClassDefinition => """
        [global::System.AttributeUsage(global::System.AttributeTargets.Field)]
        public sealed class BindableAutoGenAttribute: global::System.Attribute;
        """;
    public GeneratedSource? GetSource(ISymbol classSymbol, GeneratorAttributeSyntaxContext context, IFieldSymbol field) {
        string access = Utility.GetAccessibility(field.DeclaredAccessibility);
        string propertyName = field.Name.Replace("Property", "");
        var extra = GetBindableValueType(context, field);
        if (extra == null) {
            return null;
        }
        string extraFieldType = Utility.ToDisplayString(extra);
        string header = "";
        string content = $$"""
                {{access}} {{extraFieldType}} {{propertyName}} {
                    get => ({{extraFieldType}})GetValue({{field.Name}});
                    set => SetValue({{field.Name}}, value);
                }
            """;
        return new(header, content);
    }
    private static ITypeSymbol? GetBindableValueType(GeneratorAttributeSyntaxContext context, IFieldSymbol field) {
        if (field.DeclaringSyntaxReferences.FirstOrDefault()?.GetSyntax() is not VariableDeclaratorSyntax {
            Initializer.Value: InvocationExpressionSyntax invocation
        }) {
            return null;
        }
        GenericNameSyntax? genericName = invocation.Expression switch {
            // Create<ICommand?>(...)
            GenericNameSyntax name => name,
            // Utility.Create<ICommand?>(...)
            MemberAccessExpressionSyntax {
                Name: GenericNameSyntax name
            } => name,
            _ => null
        };
        if (genericName == null
            || genericName.TypeArgumentList.Arguments.Count < 1) {
            return null;
        }
        TypeSyntax typeArgument = genericName.TypeArgumentList.Arguments[0];
        ITypeSymbol? type = context.SemanticModel.GetTypeInfo(typeArgument).Type;
        if (typeArgument is NullableTypeSyntax && type?.IsReferenceType == true) {
            type = type.WithNullableAnnotation(NullableAnnotation.Annotated);
        }
        return type;
    }
}
