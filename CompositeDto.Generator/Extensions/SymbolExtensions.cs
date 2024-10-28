using System.Collections.Generic;
using System.Linq;
using Microsoft.CodeAnalysis;

namespace CompositeDto.Generator.Extensions;

public static class SymbolExtensions
{
    // ReSharper disable once InconsistentNaming
    public static string ToNullableFQF(this ISymbol symbol)
    {
        _ = symbol ?? throw new System.ArgumentNullException(nameof(symbol));

        var format = SymbolDisplayFormat.FullyQualifiedFormat
            .WithMiscellaneousOptions(SymbolDisplayMiscellaneousOptions.IncludeNullableReferenceTypeModifier);

        return symbol.ToDisplayString(format);
    }
    
    public static bool IsPartial(this ITypeSymbol symbol)
    {
        return symbol.DeclaringSyntaxReferences.Any();
    }
    
    // Gets a flat list of all interfaces and their inherited interfaces for a given type symbol
    public static IEnumerable<INamedTypeSymbol> GetAllInterfaces(this ITypeSymbol symbol)
    {
        return symbol.AllInterfaces
            .SelectMany(i => i.GetAllInterfaces())
            .Concat(symbol.AllInterfaces)
            .Distinct(SymbolEqualityComparer.Default)
            .OfType<INamedTypeSymbol>()
            ;
    }
}