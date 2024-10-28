using System.Collections.Generic;
using System.Collections.Immutable;

namespace CompositeDto.Generator;

internal static class EquatableImmutableArrayExtensions
{
    public static EquatableImmutableArray<T> ToEquatableImmutableArray<T>(this IEnumerable<T>? items)
    {
        if (items == null)
        {
            return EquatableImmutableArray<T>.Empty;
        }
        
        return new EquatableImmutableArray<T>(items.ToImmutableArray());
    }
}