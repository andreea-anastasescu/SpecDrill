//using Microsoft.CodeAnalysis;
//using Microsoft.CodeAnalysis.CSharp.Syntax;
//using Microsoft.CodeAnalysis.Text;
//using System;
//using System.Collections.Generic;
//using System.Collections.Immutable;
//using System.Diagnostics;
//using System.Linq;
//using System.Text;
//using System.Threading.Tasks;
////using static SpecDrill.Generator.AttributeExtractor;
//namespace SpecDrill.Generator
//{
//    public class TypeInformation {
//        public TypeInformation(string Assembly, string Namespace, string Name, bool IsNullable, int GenericParametersCount, string[] GenericArgumentTypes)
//         => (this.Assembly, this.Namespace, this.Name, this.IsNullable, this.GenericParametersCount, this.GenericArgumentTypes) = (Assembly, Namespace, Name, IsNullable, GenericParametersCount, GenericArgumentTypes);

//        public string Assembly { get; set; }
//        public string Namespace { get; set; }
//        string Name { get; set; }
//        bool IsNullable { get; set; }
//        int GenericParametersCount { get; set; }
//        string[] GenericArgumentTypes { get; set; }
//    }

//    public static class TypeExtractor
//    {
//        public static TypeInformation GetInformation(this TypeSyntax typeSyntax, SemanticModel semanticModel)
//        {

//            var (typeSymbol, isNullable) = (typeSyntax is NullableTypeSyntax nullableTypeSyntax) ?
//                (semanticModel.GetTypeInfo(nullableTypeSyntax.ElementType).Type, true) :
//                (semanticModel.GetTypeInfo(typeSyntax).Type, false);

//            if (typeSymbol is null) return default;

//            var nts = typeSymbol as INamedTypeSymbol;
//            return new TypeInformation(
//                        Assembly: typeSymbol.ContainingAssembly.Name,
//                        Namespace: typeSymbol.ContainingNamespace.Name,
//                        Name: typeSymbol.Name,
//                        IsNullable: true,
//                        GenericParametersCount: nts != null ? nts.TypeParameters.Length : 0,
//                        GenericArgumentTypes: nts != null ? nts.TypeArguments.Select(ta => string.Concat(ta.ContainingNamespace.Name ?? string.Empty, ta.Name)).ToArray() : Array.Empty<string>()
//                    );
//        }
//    }
//    public static class AttributeExtractor
//    {
//        public enum ArgumentType
//        {
//            Positional,
//            NameColon,
//            NameEquals
//        }

//        //public record AttributeArgumentInfo(ArgumentType ArgumentType, int Index, string? Name, string Value);
//        public class AttributeArgumentInfo
//        {
//            public AttributeArgumentInfo(ArgumentType argumentType, int index, string name, string value)
//                => (ArgumentType, Index, Name, Value) = (argumentType, index, name, value);

//            public ArgumentType ArgumentType { get; private set; }
//            public int Index { get; private set; }
//            public string Name { get; private set; }
//            public string Value { get; private set; }
//        }

//        //public record AttributeInfo(string Name, AttributeArgumentInfo[] Arguments);
//        public class AttributeInfo
//        {
//            public AttributeInfo(string name, AttributeArgumentInfo[] arguments)
//                => (Name, Arguments) = (name, arguments);

//            public string Name { get; private set; }
//            public AttributeArgumentInfo[] Arguments { get; private set; }
//        }

//        public static IEnumerable<AttributeInfo> ExtractAttribute(this IEnumerable<AttributeSyntax> attributes, string attributeName)
//        {
//            if (attributes == null) yield break;
//            Debug.WriteLine("ExtractAttribute(...):1");
//            string alias = (attributeName.Length > 9 && attributeName.EndsWith("Attribute")) ? attributeName.Remove(attributeName.Length - 9) : null;
//            Debug.WriteLine($"ExtractAttribute(alias = {alias}):2");
//            foreach (var attribute in attributes)
//            {
//                Debug.WriteLine("ExtractAttribute(...):3");
//                var aName = attribute.Name.ToString();
//                Debug.WriteLine($"ExtractAttribute(aName = {aName}):4");
//                if (aName == attributeName || (alias != null && aName == alias))
//                {
//                    Debug.WriteLine($"ExtractAttribute({aName}):5");
//                    Debug.WriteLine($"attributeArgumentsCount = {attribute.ArgumentList?.Arguments.Count() ?? -1}");
//                    yield return new AttributeInfo(aName, ReadAttribute(attribute.ArgumentList?.Arguments));
//                }
//            }
//        }

//        internal static AttributeArgumentInfo[] ReadAttribute(IEnumerable<AttributeArgumentSyntax> attributeArguments)
//        {
//            if (attributeArguments == null) return System.Array.Empty<AttributeArgumentInfo>();

//            var parsedArguments = new List<AttributeArgumentInfo>();

//            var i = 0;
//            foreach (var aa in attributeArguments)
//            {
//                AttributeArgumentInfo argInfo;
//                if (aa.NameColon == null && aa.NameEquals == null)
//                {
//                    string firstTokenText = aa.Expression.GetFirstToken().ValueText;
//                    bool isTypeof = firstTokenText == "typeof";

//                    argInfo = new AttributeArgumentInfo(ArgumentType.Positional, i, string.Empty, isTypeof ? aa.Expression.DescendantTokens().Skip(2).First().ValueText : firstTokenText);
//                    Debug.WriteLine($"AttributeArgumentInfo -> {argInfo.Name} = {argInfo.Value}");
//                }
//                else if (aa.NameColon != null)
//                {
//                    argInfo = new AttributeArgumentInfo(ArgumentType.NameColon, i, aa.NameColon.Name.Identifier.ValueText, aa.Expression.GetFirstToken().ValueText);
//                    Debug.WriteLine($"AttributeArgumentInfo -> {argInfo.Name} : {argInfo.Value}");
//                }
//                else if (aa.NameEquals != null)
//                {
//                    argInfo = new AttributeArgumentInfo(ArgumentType.NameEquals, i, aa.NameEquals.Name.Identifier.ValueText, aa.Expression.GetFirstToken().ValueText);
//                    Debug.WriteLine($"AttributeArgumentInfo -> {argInfo.Name} = {argInfo.Value}");
//                }
//                else
//                    throw new System.Exception("Must not be possible!");

//                parsedArguments.Add(argInfo);
//                i++;
//            }
//            return parsedArguments.ToArray();
//        }
//    }
//    class SyntaxReceiver : ISyntaxContextReceiver
//    {
//        public List<(string typeName, string fileContents)> Files { get; } = new();
//        /// <summary>
//        /// Called for every syntax node in the compilation, we can inspect the nodes and save any information useful for generation
//        /// </summary>
//        public void OnVisitSyntaxNode(GeneratorSyntaxContext context)
//        {
//            // any field with at least one attribute is a candidate for property generation
//            if (context.Node is TypeDeclarationSyntax typeDeclarationSyntax
//                && typeDeclarationSyntax.AttributeLists.Count > 0 && typeDeclarationSyntax.AttributeLists.Any(a => a.Attributes.ExtractAttribute("WebPage").Any()))
//            {
//                var type = context.SemanticModel.GetDeclaredSymbol(typeDeclarationSyntax) as INamedTypeSymbol;

//                if (type is not { })
//                    return;


//                var containingNamespace = type.ContainingNamespace;

//                if (containingNamespace is { })
//                {
//                    Files.Add((type.Name,
//                        $$"""
//                     using {{containingNamespace.Name}};
//                     public partial class {{type.Name}} : WebPage {}
//                    """));
//                }
//            }
//        }
//    }

//    [Generator]
//    public class PageObjectGenerator : ISourceGenerator
//    {
//        public void Execute(GeneratorExecutionContext context)
//        {
//            if (context.SyntaxContextReceiver is not SyntaxReceiver receiver)
//                return;

//            foreach (var (typeName, fileContents) in receiver.Files)
//                context.AddSource($"{typeName}_pageObject.g.cs", SourceText.From(fileContents, Encoding.UTF8));
//        }

//        public void Initialize(GeneratorInitializationContext context)
//        {
//            context.RegisterForSyntaxNotifications(() => new SyntaxReceiver());
//        }
//    }
//}
