using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.Text;
using MusicEco.Generators.AutoGens;
using System.Collections.Immutable;

namespace MusicEco.Generators;

[Generator]
public sealed class PropertyGenerator: IIncrementalGenerator {
    public const string FileHeader = """
        namespace MusicEco.SourceGeneration;
        """;
    private const string LineSeparator = "\r\n";
    public const string FileName = "GeneratedPropertyAttributes.g.cs";
    private class PropertySpec {
        public ISourceGen Info;
        public ISymbol Symbol;
        public GeneratorAttributeSyntaxContext Context;
        public PropertySpec(ISourceGen info, ISymbol symbol, GeneratorAttributeSyntaxContext context) {
            this.Info = info;
            this.Symbol = symbol;
            this.Context = context;
        }
    }
    public void Initialize(IncrementalGeneratorInitializationContext context) {
        List<ISourceGen> properties = [
            //new GeneratedProperty(),
            new ObservableProperty(),
            new BindableAutoGenField(),
            new AppSettingProperty()
            ];
        // Static pass
        string fileSource = FileHeader;
        foreach (var property in properties) {
            fileSource += LineSeparator + property.AttributeClassDefinition;
        }
        context.RegisterPostInitializationOutput(context => {
            context.AddSource(FileName, SourceText.From(fileSource, System.Text.Encoding.UTF8));
        });
        // Dynamic pass
        IncrementalValueProvider<ImmutableArray<PropertySpec>>? allProperties = null;
        foreach (var property in properties) {
            IncrementalValuesProvider<PropertySpec> valuesProvider;
            if (property is ISourceGenProperty sourceGenProperty) {
                valuesProvider = FindProperties(context, property);
            }
            else if (property is ISourceGenField sourceGenField) {
                valuesProvider = FindFields(context, property);
            }
            else {
                continue;
            }
            if (allProperties == null) {
                allProperties = valuesProvider.Collect();
            }
            else {
                allProperties = allProperties.Value.Combine(valuesProvider.Collect()).Select((pair, _) => pair.Left.AddRange(pair.Right));
            }
        }
        if (allProperties != null) {
            context.RegisterSourceOutput(allProperties.Value, EmitClasses);
        }
    }
    private static IncrementalValuesProvider<PropertySpec> FindProperties(
        IncrementalGeneratorInitializationContext context,
        ISourceGen info) {
        return context.SyntaxProvider.ForAttributeWithMetadataName(
            $"MusicEco.SourceGeneration.{info.AttributeMetadataName}",
            static (node, _) => node is Microsoft.CodeAnalysis.CSharp.Syntax.PropertyDeclarationSyntax,
            (context, _) => new PropertySpec(info, (IPropertySymbol)context.TargetSymbol, context));
    }
    private static IncrementalValuesProvider<PropertySpec> FindFields(
        IncrementalGeneratorInitializationContext context,
        ISourceGen info) {
        return context.SyntaxProvider.ForAttributeWithMetadataName(
            $"MusicEco.SourceGeneration.{info.AttributeMetadataName}",
            static (node, _) => true,
            (context, _) => new PropertySpec(info, (IFieldSymbol)context.TargetSymbol, context));
    }
    private static void EmitClasses(
        SourceProductionContext context,
        ImmutableArray<PropertySpec> properties) {
        foreach (var group in properties.GroupBy(
            spec => spec.Symbol.ContainingType,
            SymbolEqualityComparer.Default)) {
            var type = group.Key;
            if (type == null || group.GroupBy(spec => spec.Symbol, SymbolEqualityComparer.Default)
                .Any(property => property.Select(x => x.Info.AttributeName).Distinct().Skip(1).Any())) {
                // Skip property that has more than one attributes
                continue;
            }
            List<string> headers = [];
            List<string> contents = [];
            string className = type.Name;
            foreach (var spec in group) {
                GeneratedSource? generatedSource = null;
                if (spec.Info is ISourceGenProperty sourceGenProperty
                    && spec.Symbol is IPropertySymbol propertySymbol) {
                    generatedSource = sourceGenProperty.GetSource(className, spec.Context, propertySymbol);
                }
                else if (spec.Info is ISourceGenField sourceGenField
                    && spec.Symbol is IFieldSymbol fieldSymbol) {
                    generatedSource = sourceGenField.GetSource(className, spec.Context, fieldSymbol);
                }
                if (generatedSource == null) {
                    continue;
                }
                if (generatedSource.Header.Trim().Length > 0) {
                    headers.Add(generatedSource.Header);
                }
                ;
                contents.Add(generatedSource.Content);
            }
            var totalHeader = string.Join(LineSeparator, headers);
            string source;
            string typeAccess = Utility.GetAccessibility(type.DeclaredAccessibility);
            if (totalHeader.Trim().Length > 0) {
                source = $$"""
                    {{totalHeader}}
                    namespace {{type.ContainingNamespace}};
                    {{typeAccess}} partial class {{className}} {
                    {{string.Join(LineSeparator, contents)}}
                    }
                    """;
            }
            else {
                source = $$"""
                    namespace {{type.ContainingNamespace}};
                    {{typeAccess}} partial class {{className}} {
                    {{string.Join(LineSeparator, contents)}}
                    }
                    """;
            }
            var hintName = $"{type.ToDisplayString(SymbolDisplayFormat.FullyQualifiedFormat).Replace("global::", "")}.g.cs";
            context.AddSource(hintName, SourceText.From(source, System.Text.Encoding.UTF8));
        }
    }
}
