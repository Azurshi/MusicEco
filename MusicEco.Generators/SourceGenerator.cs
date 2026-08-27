using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.Text;
using MusicEco.Generators.SourceGenerators;
using System.Collections.Immutable;

namespace MusicEco.Generators;

[Generator]
public sealed class SourceGenerator: IIncrementalGenerator {
    private const string GeneratedNameSpace = "MusicEco.SourceGeneration";
    private const string GeneratedFiledName = "GeneratedAttributes.g.cs";
    private static readonly System.Text.Encoding Encoding = System.Text.Encoding.UTF8;
    public void Initialize(IncrementalGeneratorInitializationContext context) {
        List<ISourceGen> generators = [
            new AppSettingPropertyGenerator(),
            new BindableAutoGenGenerator(),
            new BindedPropertyGenerator(),
            new ObservablePropertyGenerator(),
            new RelayCommandGenerator(),
            ];
        // Attribute files
        string fileSource = $"""
            namespace {GeneratedNameSpace};
            """;
        foreach (var generator in generators) {
            fileSource += Utility.LineSeparator + generator.AttributeClassDefinition;
        }
        context.RegisterPostInitializationOutput(context => {
            context.AddSource(GeneratedFiledName, SourceText.From(fileSource, Encoding));
        });
        // Generators
        IncrementalValueProvider<ImmutableArray<GeneratorSpec>>? allGenerators = null;
        foreach(var generator in generators) {
            IncrementalValuesProvider<GeneratorSpec>? valuesProvider = null;
            if (generator is IFieldBasedGenerator) {
                valuesProvider = context.SyntaxProvider.ForAttributeWithMetadataName(
                    $"{GeneratedNameSpace}.{generator.AttributeMetadataName}",
                    (node, _) => true,
                    (context, _) => new GeneratorSpec(generator, context.TargetSymbol, context));
            }
            else if (generator is IPropertyBasedGenerator) {
                valuesProvider = context.SyntaxProvider.ForAttributeWithMetadataName(
                    $"{GeneratedNameSpace}.{generator.AttributeMetadataName}",
                    (node, _) => node is Microsoft.CodeAnalysis.CSharp.Syntax.PropertyDeclarationSyntax,
                    (context, _) => new GeneratorSpec(generator, context.TargetSymbol, context));
            }
            else if (generator is IMethodBasedGenerator) {
                valuesProvider = context.SyntaxProvider.ForAttributeWithMetadataName(
                    $"{GeneratedNameSpace}.{generator.AttributeMetadataName}",
                    (node, _) => node is Microsoft.CodeAnalysis.CSharp.Syntax.MethodDeclarationSyntax,
                    (context, _) => new GeneratorSpec(generator, context.TargetSymbol, context));
            }
            if (valuesProvider != null) {
                if (allGenerators == null) {
                    allGenerators = valuesProvider.Value.Collect();
                } else {
                    allGenerators = allGenerators.Value
                        .Combine(valuesProvider.Value.Collect())
                        .Select((pair, _) => pair.Left.AddRange(pair.Right));
                }
            }
        }
        if (allGenerators != null) {
            context.RegisterSourceOutput(allGenerators.Value, EmitCode);
        }
    }
    private static void EmitCode(SourceProductionContext context, ImmutableArray<GeneratorSpec> generators) {
        foreach(var group in generators.GroupBy(g => g.Symbol.ContainingType, SymbolEqualityComparer.Default)) {
            var classType = group.Key;
            if (classType == null) {
                continue;
            }
            List<string> headers = [];
            List<string> contents = [];
            string className = classType.Name;
            foreach(var generator in group) {
                var generatedSource = generator.GetSource(classType);
                if (generatedSource != null) {
                    foreach(var diagnostic in generatedSource.Diagnostics) {
                        context.ReportDiagnostic(diagnostic);
                    }
                    if (!string.IsNullOrWhiteSpace(generatedSource.Header)) {
                        headers.Add(generatedSource.Header);
                    }
                    if (!string.IsNullOrWhiteSpace(generatedSource.Content)) {
                        contents.Add(generatedSource.Content);
                    }
                }
            }
            var totalHeader = string.Join(Utility.LineSeparator, headers.ToImmutableHashSet().OrderBy(v => v));
            string source;
            string typeAccess = Utility.GetAccessibility(classType.DeclaredAccessibility);
            if (contents.Count == 0) {
                continue;
            }
            if (totalHeader.Trim().Length > 0) {
                source = $$"""
                    {{totalHeader}}
                    namespace {{classType.ContainingNamespace}};
                    {{typeAccess}} partial class {{className}} {
                    {{string.Join(Utility.LineSeparator, contents)}}
                    }
                    """;
            }
            else {
                source = $$"""
                    namespace {{classType.ContainingNamespace}};
                    {{typeAccess}} partial class {{className}} {
                    {{string.Join(Utility.LineSeparator, contents)}}
                    }
                    """;
            }
            var hintName = $"{classType.ToDisplayString(SymbolDisplayFormat.FullyQualifiedFormat).Replace("global::", "")}.g.cs";
            context.AddSource(hintName, SourceText.From(source, Encoding));
        }
    }
}
