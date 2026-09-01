using Microsoft.CodeAnalysis;

namespace MusicEco.Generators.SourceGenerators;

internal class AppSettingPropertyGenerator: IPropertyBasedGenerator {
    public string AttributeMetadataName => "AppSettingPropertyAttribute";
    public string AttributeClassDefinition => """
        [global::System.AttributeUsage(global::System.AttributeTargets.Property)]
        public sealed class AppSettingPropertyAttribute: global::System.Attribute {
            public object? DefaultValue;
            public string? StorageFieldName { get; set; }
            public bool IsObservableObject { get; set; }
            public AppSettingPropertyAttribute(object? defaultValue) {
                this.DefaultValue = defaultValue;
                this.StorageFieldName = null;
                this.IsObservableObject = true;
            }
        }
        """;
    public GeneratedSource? GetSource(ISymbol classSymbol, GeneratorAttributeSyntaxContext context, IPropertySymbol property) {
        var propertyType = Utility.ToDisplayString(property.Type);
        string access = Utility.GetAccessibility(property.DeclaredAccessibility);
        var attribute = context.Attributes[0];
        var defaultValue = ToSource(attribute.ConstructorArguments[0]);
        if (!(Utility.TryGetAttibuteNamedArguments(context, "StorageFieldName", out string? storageFieldName)
            && storageFieldName != null)) {
            storageFieldName = $"{classSymbol.Name}.{property.Name}";
        }
        if (!(Utility.TryGetAttibuteNamedArguments(context, "IsObservableObject", out bool? isObservableObject)
            && isObservableObject != null)) {
            isObservableObject = true;
        }
        string header = "";
        string content;
        if (isObservableObject.Value) {
            content = $$"""
                    {{access}} partial {{propertyType}} {{property.Name}} {
                        get => this._setting.Get<{{propertyType}}>(({{propertyType}}){{defaultValue}}, "{{storageFieldName}}");
                        set {
                            if (this.{{property.Name}} != value) {
                                this._setting.Set(value, "{{storageFieldName}}");
                                this.OnPropertyChanged();
                            }
                        }
                    }
                """;
        }
        else {
            content = $$"""
                    {{access}} partial {{propertyType}} {{property.Name}} {
                        get => this._setting.Get<{{propertyType}}>(({{propertyType}}){{defaultValue}}, "{{storageFieldName}}");
                        set {
                            if (this.{{property.Name}} != value) {
                                this._setting.Set(value, "{{storageFieldName}}");
                            }
                        }
                    }
                """;
        }
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
