using Microsoft.CodeAnalysis;
using System.Collections.Immutable;

namespace MusicEco.Generators;

internal class GeneratedSource {
    public readonly string Header;
    public readonly string Content;
    public readonly ImmutableArray<Diagnostic> Diagnostics;
    public GeneratedSource(string header, string content, params Diagnostic[] diagnostics) {
        this.Header = header;
        this.Content = content;
        this.Diagnostics = diagnostics.ToImmutableArray();
    }
    public static GeneratedSource Diagnostic(Diagnostic diagnostic) {
        return new("", "", diagnostic);
    }
}
internal interface ISourceGen {
    public string AttributeMetadataName { get; }
    public string AttributeClassDefinition { get; }
}
internal interface IPropertyBasedGenerator: ISourceGen {
    public GeneratedSource? GetSource(ISymbol classSymbol, GeneratorAttributeSyntaxContext context, IPropertySymbol property);
}
internal interface IFieldBasedGenerator: ISourceGen {
    public GeneratedSource? GetSource(ISymbol classSymbol, GeneratorAttributeSyntaxContext context, IFieldSymbol field);
}
internal interface IMethodBasedGenerator: ISourceGen {
    public GeneratedSource? GetSource(ISymbol classSymbol, GeneratorAttributeSyntaxContext context, IMethodSymbol method);
}
internal class GeneratorSpec {
    public ISourceGen Generator;
    public ISymbol Symbol;
    public GeneratorAttributeSyntaxContext Context;
    public GeneratorSpec(ISourceGen generator, ISymbol symbol, GeneratorAttributeSyntaxContext context) {
        this.Generator = generator;
        this.Symbol = symbol;
        this.Context = context;
    }
    public GeneratedSource? GetSource(ISymbol classSymbol) {
        if (this.Generator is IFieldBasedGenerator fieldBasedGenerator
            && this.Symbol is IFieldSymbol fieldSymbol) {
            return fieldBasedGenerator.GetSource(classSymbol, this.Context, fieldSymbol);
        }
        else if (this.Generator is IPropertyBasedGenerator propertyBasedGenerator
            && this.Symbol is IPropertySymbol propertySymbol) {
            return propertyBasedGenerator.GetSource(classSymbol, this.Context, propertySymbol);
        }
        else if (this.Generator is IMethodBasedGenerator methodBasedGenerator
            && this.Symbol is IMethodSymbol methodSymbol) {
            return methodBasedGenerator.GetSource(classSymbol, this.Context, methodSymbol);
        }
        else {
            return null;
        }   
    }
}

internal static class Utility {
    public static string GetAccessibility(Accessibility accessibility) {
        return accessibility switch {
            Accessibility.Public => "public",
            Accessibility.Protected => "protected",
            Accessibility.Internal => "internal",
            Accessibility.Private => "private",
            Accessibility.ProtectedAndInternal => "private protected",
            Accessibility.ProtectedOrInternal => "protected private",
            _ => ""
        };
    }
    public static string ToDisplayString(ITypeSymbol symbol) {
        return symbol.ToDisplayString(
            SymbolDisplayFormat.FullyQualifiedFormat
            .WithMiscellaneousOptions(
                SymbolDisplayMiscellaneousOptions.IncludeNullableReferenceTypeModifier
                | SymbolDisplayMiscellaneousOptions.EscapeKeywordIdentifiers));
    }
    public static string ToDisplayStringWithoutNullable(ITypeSymbol symbol) {
        return symbol.ToDisplayString(
            SymbolDisplayFormat.FullyQualifiedFormat
            .WithMiscellaneousOptions(
                SymbolDisplayMiscellaneousOptions.EscapeKeywordIdentifiers));
    }
    public static string GetFieldName(string propertyName) {
        var name = propertyName;
        return "_" + char.ToLowerInvariant(name[0]) + name.Substring(1);
    }
}

