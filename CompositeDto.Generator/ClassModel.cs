using System;

namespace CompositeDto.Generator;

internal record Class : IEquatable<Class?>
{
    /// <summary>
    /// Class, Interface, Record, Struct
    /// </summary>
    public string Type { get; set; }
    
    internal required string Name { get; init; }
    internal required EquatableImmutableArray<InterfaceModel> Interfaces { get; init; }
    
}

internal record InterfaceModel
{
    internal string Namespace { get; set; }
    internal string Name { get; set; }

    public EquatableImmutableArray<PropertyModel> Properties { get; set; }
}

internal record PropertyModel
{
    public string Name { get; init; }
    public string Type { get; init; }

    public string? Docs { get; init; }
}

internal sealed record DtoTarget
{
    public string Namespace { get; init; }
    public Class Class { get; init; }
    public EquatableImmutableArray<Class> OuterClasses { get; init; }
}