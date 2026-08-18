using Microsoft.CodeAnalysis;

namespace MusicEco.Generators.SourceGenerators;

internal class RelayCommandGenerator: IMethodBasedGenerator {
    public string AttributeMetadataName => "RelayCommandAttribute";
    public string AttributeClassDefinition => """
        [global::System.AttributeUsage(global::System.AttributeTargets.Method)]
        public sealed class RelayCommandAttribute: global::System.Attribute {
            public string CanExecute { get; set; }
        }
        """;
    public GeneratedSource? GetSource(ISymbol classSymbol, GeneratorAttributeSyntaxContext context, IMethodSymbol method) {
        Location location = method.Locations.FirstOrDefault() ?? Location.None;

        string commandClass;
        string constructorArguments;
        string methodName = method.Name;
        string commandName = methodName + "Command";
        string fieldName = Utility.GetFieldName(commandName);
        string header = "using MusicEco.Core.Types;";
        bool isAsyncVoid = false;
        if (method.ReturnsVoid) {
            if (method.IsAsync) {
                isAsyncVoid = true;
            }
            commandClass = "SyncCommand";
            constructorArguments = methodName;
        }
        else if (IsReturnTask(method)) {
            commandClass = "AsyncCommand";
            constructorArguments = methodName;
        }
        else {
            return GeneratedSource.Diagnostic(
                Diagnostic.Create(
                    InvalidReturnType,
                    location,
                    method.Name,
                    method.ReturnType.ToDisplayString()));
        }
        string? canExecuteMethodName = GetCanExecute(context);
        if (canExecuteMethodName != null) {
            commandClass += "Extend";
            constructorArguments += $", {canExecuteMethodName}";
        }
        if (method.Parameters.Length == 0) {
            string content = $$"""
                private {{commandClass}}? {{fieldName}};
                public {{commandClass}} {{commandName}} => {{fieldName}} ??= new ({{constructorArguments}});
            """;
            if (isAsyncVoid) {
                return new(header, content, Diagnostic.Create(AsyncVoidReturn, location, method.Name));
            }
            else {
                return new(header, content);
            }
        }
        else if (method.Parameters.Length == 1) {
            string argumentType = Utility.ToDisplayStringWithoutNullable(method.Parameters[0].Type);
            string content = $$"""
                private {{commandClass}}<{{argumentType}}>? {{fieldName}};
                public {{commandClass}}<{{argumentType}}> {{commandName}} => {{fieldName}} ??= new ({{constructorArguments}});
            """;
            if (isAsyncVoid) {
                return new(header, content, Diagnostic.Create(AsyncVoidReturn, location, method.Name));
            }
            else {
                return new(header, content);
            }
        }
        else {
            return GeneratedSource.Diagnostic(
                Diagnostic.Create(
                    ToManyParamters,
                    location,
                    method.Name,
                    method.Parameters.Length));
        }
    }
    private static bool IsReturnTask(IMethodSymbol method) {
        return method.ReturnType is INamedTypeSymbol type
            && type.Name == "Task"
            && type.ContainingNamespace.ToDisplayString() == "System.Threading.Tasks"
            && type.Arity == 0;
    }
    private static string? GetCanExecute(GeneratorAttributeSyntaxContext context) {
        foreach(var argument in context.Attributes[0].NamedArguments) {
            if (argument.Key == "CanExecute"
                && argument.Value.Value is string canExecuteMethodName) {
                return canExecuteMethodName;
            }
        }
        return null;
    }
#pragma warning disable RS2008
    private static readonly DiagnosticDescriptor InvalidReturnType = new(
        id: "MGEN001",
        title: "Invalid relay-command return type",
        messageFormat:
            "Method '{0}' return '{1}'. Relay commands support only void or non-generic Task.",
        category: "MusicEco.Generators",
        defaultSeverity: DiagnosticSeverity.Error,
        isEnabledByDefault: true);
    private static readonly DiagnosticDescriptor ToManyParamters = new(
        id: "MGEN002",
        title: "To many relay-command parameters",
        messageFormat:
            "Method '{0}' has '{1}' parameters. Relay commands support at most one parameter.",
        category: "MusicEco.Generators",
        defaultSeverity: DiagnosticSeverity.Error,
        isEnabledByDefault: true);
    private static readonly DiagnosticDescriptor AsyncVoidReturn = new(
        id: "MGEN003",
        title: "Await void",
        messageFormat:
            "Method '{0}' return void. Async method should return Task.",
        category: "MusicEco.Generators",
        defaultSeverity: DiagnosticSeverity.Warning,
        isEnabledByDefault: true);
#pragma warning restore RS2008
}
