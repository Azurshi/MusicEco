using Microsoft.CodeAnalysis;

namespace MusicEco.Generators;

internal class GeneratedSource {
    public readonly string Header;
    public readonly string Content;
    public GeneratedSource(string header, string content) {
        this.Header = header;
        this.Content = content;
    }
}
internal interface ISourceGen {
    public string AttributeName { get; }
    public string AttributeMetadataName { get; }
    public string AttributeClassDefinition { get; }
}

internal interface ISourceGenProperty: ISourceGen {
    public GeneratedSource? GetSource(string className, GeneratorAttributeSyntaxContext context, IPropertySymbol property);
}

internal interface ISourceGenField: ISourceGen {
    public GeneratedSource? GetSource(string className, GeneratorAttributeSyntaxContext context, IFieldSymbol property);
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
}