using CompositeDto.Generator.Extensions;

using Microsoft.CodeAnalysis;

using System.Collections.Generic;
using System.Collections.Immutable;
using System.Linq;
using System.Threading;

namespace CompositeDto.Generator;

internal class Transformer
{
    private readonly GeneratorAttributeSyntaxContext _context;
    private readonly CancellationToken _token;
    private readonly ITypeSymbol _symbol;

    public Transformer(GeneratorAttributeSyntaxContext context, CancellationToken token)
    {
        _context = context;
        _token = token;
        _symbol = (ITypeSymbol)context.TargetSymbol;
    }

    public DtoTarget? Transform()
    {
        if (_token.IsCancellationRequested)
            return default;

        var target = GetClass(_symbol);
        if (target is null || target.Interfaces.Length == 0)
        {
            return default;
        }

        var outerClasses = GetOuterClasses();
        var namespaceName = _symbol.ContainingNamespace.ToDisplayString();

        return new DtoTarget { Namespace = namespaceName, Class = target, OuterClasses = outerClasses };
    }

    private static Class? GetClass(ITypeSymbol symbol)
    {
        var interfaces = symbol
            .GetAllInterfaces()
            .Select(i => new InterfaceModel
            {
                Name = i.ToDisplayString(SymbolDisplayFormat.FullyQualifiedFormat),
                Namespace = i.ContainingNamespace.ToDisplayString(),
                Properties = i
                    .GetMembers()
                    .OfType<IPropertySymbol>()
                    .Select(p => new PropertyModel
                    {
                        Name = p.Name,
                        Type = SymbolExtensions.ToNullableFQF(p.Type),
                        Docs = GetDocumentationCommentXml(p),
                    })
                    .ToImmutableArray()
            }).ToImmutableArray();


        return new Class
        {
            Name = symbol.ToDisplayString(SymbolDisplayFormat.MinimallyQualifiedFormat),
            Interfaces = interfaces,
            // IsPartial = symbol.IsPartial,

            Type = symbol switch
            {
                { TypeKind: TypeKind.Interface } => "interface",
                { IsRecord: true, TypeKind: TypeKind.Struct, } => "record struct",
                { IsRecord: true, } => "record",
                { TypeKind: TypeKind.Struct, } => "struct",
                _ => "class",
            },
        };
    }

    private static string? GetDocumentationCommentXml(IPropertySymbol p)
    {
        var doc = p.GetDocumentationCommentXml(expandIncludes: true);
        var extracted = ExtractMemberContent(doc);
        
        return extracted;
    }
    
    private static string ExtractMemberContent(string xml)
    {
        const string startTag = "<member";
        const string endTag = "</member>";

        int startIndex = xml.IndexOf(startTag);
        int endIndex = xml.LastIndexOf(endTag);

        if (startIndex == -1 || endIndex == -1)
        {
            return string.Empty;
        }

        startIndex = xml.IndexOf('>', startIndex) + 1;
        // endIndex += endTag.Length;
        
        return xml.Substring(startIndex, endIndex - startIndex);
    }

    // private static string? GetDocs(IPropertySymbol p)
    // {
    //     var docs = p.GetDocumentationCommentXml();
    //     // Prepend a /// to each line
    //     // return docs is null ? null : "/// " + string.Join("\n/// ", docs.Split('\n'));
    // }

    private EquatableImmutableArray<Class> GetOuterClasses()
    {
        List<Class>? outerClasses = null;
        var outerSymbol = _symbol.ContainingType;
        while (outerSymbol is not null)
        {
            (outerClasses ??= []).Add(GetClass(outerSymbol));
            outerSymbol = outerSymbol.ContainingType;
        }

        if (outerClasses is null)
            return EquatableImmutableArray<Class>.Empty;

        outerClasses.Reverse();

        return outerClasses.ToEquatableImmutableArray();
    }
}