using Microsoft.CodeAnalysis;

namespace MusicEco.Generators.AutoGens;

internal class AppSettingProperty: ISourceGenProperty {
    public string AttributeName => "AppSettingPropertyAttribute";
    public string AttributeMetadataName => AttributeName;
    public string AttributeClassDefinition => $$"""
        [global::System.AttributeUsage(global::System.AttributeTargets.Property)]
        public sealed class AppSettingPropertyAttribute: global::System.Attribute {
            public object? DefaultValue;
            public string? StorageFieldName;
            public AppSettingPropertyAttribute(object? defaultValue) {
                this.DefaultValue = defaultValue;
                this.StorageFieldName = null;
            }
            public AppSettingPropertyAttribute(object? defaultValue, string storageFieldName) {
                this.DefaultValue = defaultValue;
                this.StorageFieldName = storageFieldName;
            }
        }
        """;
    public GeneratedSource? GetSource(string className, GeneratorAttributeSyntaxContext context, IPropertySymbol property) {
        var propertyType = property.Type.ToDisplayString(
            SymbolDisplayFormat.FullyQualifiedFormat
            .WithMiscellaneousOptions(
                SymbolDisplayMiscellaneousOptions.IncludeNullableReferenceTypeModifier
                | SymbolDisplayMiscellaneousOptions.EscapeKeywordIdentifiers));
        var attribute = context.Attributes[0];
        var defaultValue = ToSource(attribute.ConstructorArguments[0]);
        string storageFieldName;
        if (attribute.ConstructorArguments.Length > 1) {
            storageFieldName = (string)attribute.ConstructorArguments[1].Value!;
        } else {
            storageFieldName = $"{className}.{property.Name}";
        }
        string access = Utility.GetAccessibility(property.DeclaredAccessibility);
        string header = "";
        string content = $$"""
                {{access}} partial {{propertyType}} {{property.Name}} {
                    get => this._setting.Get<{{propertyType}}>({{defaultValue}}, "{{storageFieldName}}");
                    set {
                        if (this.{{property.Name}} != value) {
                            this._setting.Set(value, "{{storageFieldName}}");
                            this.OnPropertyChanged();
                        }
                    }
                }
            """;
        return new(header, content);
    }
    private static string ToSource(TypedConstant value) {
        if (value.Kind == TypedConstantKind.Enum
            && value.Type is INamedTypeSymbol enumType) {
            var enumTypeName = enumType.ToDisplayString(SymbolDisplayFormat.FullyQualifiedFormat);
            var enumMember = enumType.GetMembers()
                .OfType<IFieldSymbol>()
                .FirstOrDefault(member => member.HasConstantValue && Equals(member.ConstantValue, value.Value));
            if (enumMember != null) {
                return $"{enumTypeName}.{enumMember.Name}";
            }
            // Fallback to cast if failed
            return $"({enumTypeName}){value.Value}";
        }
        return value.Value switch {
            null => "null",
            string text => Microsoft.CodeAnalysis.CSharp.SymbolDisplay.FormatLiteral(text, true),
            bool boolean => boolean ? "true" : "false",
            _ => Convert.ToString(value.Value, System.Globalization.CultureInfo.InvariantCulture)
        };

    }
}
