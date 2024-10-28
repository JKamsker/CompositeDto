using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Linq;

namespace CompositeDto.Generator;

internal struct EquatableImmutableArray<T> : IEquatable<EquatableImmutableArray<T>>, IEnumerable<T>
{
    public ImmutableArray<T> Items { get; }
    
    public static EquatableImmutableArray<T> Empty { get; } = new(ImmutableArray<T>.Empty);

    public int Length => Items.Length;

    public EquatableImmutableArray() : this(ImmutableArray<T>.Empty)
    {
    }

    public EquatableImmutableArray(ImmutableArray<T> items)
    {
        Items = items;
    }

    public bool Equals(EquatableImmutableArray<T> other)
    {
        return Items.SequenceEqual(other.Items);
    }


    public override bool Equals(object? obj)
    {
        return obj is EquatableImmutableArray<T> other && Equals(other);
    }

    public override int GetHashCode()
    {
        var hash = new HashCode();
        foreach (var item in Items)
        {
            hash.Add(item);
        }

        return hash.ToHashCode();
    }


    public IEnumerator<T> GetEnumerator()
    {
        return GetEnumerable().GetEnumerator();
    }

    IEnumerator IEnumerable.GetEnumerator()
    {
        return GetEnumerable().GetEnumerator();
    }

    private IEnumerable<T> GetEnumerable()
    {
        return Items;
    }

    public static bool operator ==(EquatableImmutableArray<T> left, EquatableImmutableArray<T> right)
    {
        return left.Equals(right);
    }

    public static bool operator !=(EquatableImmutableArray<T> left, EquatableImmutableArray<T> right)
    {
        return !left.Equals(right);
    }

    // Implicit to and from ImmutableArray<T>
    public static implicit operator EquatableImmutableArray<T>(ImmutableArray<T> items) => new(items);
    public static implicit operator ImmutableArray<T>(EquatableImmutableArray<T> items) => items.Items;
}