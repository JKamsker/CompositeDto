# CompositeDto

[![CI](https://github.com/JKamsker/CompositeDto/actions/workflows/ci.yml/badge.svg)](https://github.com/JKamsker/CompositeDto/actions/workflows/ci.yml)
[![NuGet](https://img.shields.io/nuget/v/CompositeDto.Generator.svg?logo=nuget)](https://www.nuget.org/packages/CompositeDto.Generator/)

CompositeDto is a Roslyn incremental source generator that keeps your DTOs in sync with the interfaces they implement. Decorate a partial class with `[CompositeDto]`, implement as many interfaces as you like, and the generator writes the backing auto-properties (including XML docs) for you at compile time.

## Why CompositeDto?
- **Zero boilerplate DTOs** – copy an interface graph into a DTO with a single attribute.
- **Interface-aware** – walks inherited interfaces, nested types, and generics so you never forget a property.
- **Smart conflict handling** – reuses identical properties and falls back to explicit interface implementations when the same property name appears with different types.
- **Documentation preserved** – interface XML comments are emitted onto the generated properties (or borrowed from matching members).
- **Analyzer friendly** – ships as an analyzer NuGet package targeting `netstandard2.0`, so it works across the modern .NET SDKs.

## Table of Contents
- [Installation](#installation)
- [Quick Start](#quick-start)
- [What Gets Generated](#what-gets-generated)
- [Generation Rules](#generation-rules)
- [Developing & Testing](#developing--testing)
- [Continuous Integration](#continuous-integration)
- [Contributing](#contributing)
- [License](#license)

## Installation
CompositeDto consists of the generator plus a tiny runtime assembly that contains only the `CompositeDtoAttribute`. You just need to reference the NuGet package; the attribute and analyzer assets are included automatically.

### .NET CLI
```bash
dotnet add package CompositeDto.Generator
```

### Project file snippet
```xml
<ItemGroup>
  <PackageReference Include="CompositeDto.Generator"
                    Version="*"
                    PrivateAssets="all" />
</ItemGroup>
```
> Tip: keeping `PrivateAssets="all"` prevents the generator from flowing transitively to consumers of your library.

## Quick Start
```csharp
using CompositeDto.Generator.Runtime;

public interface IHasAuditInfo
{
    DateTime CreatedAt { get; set; }
    string CreatedBy { get; set; }
}

public interface IOrderLine
{
    Guid LineId { get; set; }
    string Sku { get; set; }
    decimal Quantity { get; set; }
}

[CompositeDto]
public partial class OrderLineDto : IOrderLine, IHasAuditInfo
{
    // You can still add your own bespoke properties or methods here.
    public bool IsGift { get; set; }
}
```

Build the project and the analyzer will drop a file similar to `OrderLineDto_CompositeDto.g.cs` under `obj/GeneratedFiles` (or `.generated` if you opt into `EmitCompilerGeneratedFiles` like the functional tests do).

## What Gets Generated
```csharp
// Properties from global::IOrderLine
public Guid LineId { get; set; }
public string Sku { get; set; }
public decimal Quantity { get; set; }

// Properties from global::IHasAuditInfo
public DateTime CreatedAt { get; set; }
public string CreatedBy { get; set; }
```

If the class implements two interfaces that both expose `string Name { get; set; }`, the property is emitted once, along with a comment explaining it was skipped for the duplicate. If the property names clash but types differ, the generator emits an explicit interface implementation (e.g., `string IFoo.Name { get; set; }`) so every contract is still fulfilled.

## Generation Rules
- Only partial classes marked with `[CompositeDto]` are processed.
- The generator traverses the entire interface graph (including inherited interfaces and nested/outer types) to gather properties.
- Properties keep the order in which interfaces are declared on your class.
- XML documentation from the interface is copied onto the generated members. If multiple interfaces declare the same `(Name, Type)` pair, the first one with documentation wins.
- Nested classes are supported – generated files recreate the full outer-class structure.
- No behavior code is emitted; you can continue adding custom logic, validation, or constructors inside your partial class without losing the generated properties.

## Developing & Testing
This repository ships a solution with the generator, runtime attribute, unit/functional tests, and CI workflow.

Common commands:
```bash
# Restore dependencies for all projects
dotnet restore CompositeDto.Generator.sln

# Build everything in Release mode
dotnet build CompositeDto.Generator.sln -c Release

# Run the full test suite (xUnit + functional scenarios)
dotnet test CompositeDto.Generator.sln -c Release --logger trx

# Produce a NuGet package locally
dotnet pack CompositeDto.Generator/CompositeDto.Generator.csproj -c Release -o artifacts
```

Functional scenarios under `CompositeDto.Generator.FunctionalTests` demonstrate:
- Basic DTO generation from multiple interfaces.
- Handling of interface inheritance and nested types.
- Generic interface implementations, including conflicting generic parameter bindings.
- Property conflicts across multiple interfaces.
Investigate the `.generated` folder in that project to inspect the emitted code.

## Continuous Integration
The `CI` workflow builds, tests, packs, and publishes the package to NuGet whenever `master` receives a push. Each workflow run automatically bumps the package version by combining the base version from `CompositeDto.Generator.csproj` with the GitHub `run_number`, guaranteeing unique versions for every deployment. Secrets needed for publishing are retrieved at run time from Bitwarden via `bitwarden/sm-action`.

## Contributing
Contributions are welcome! If you spot a bug or want to propose a new generation rule:
1. Open an issue describing the scenario.
2. Fork the repo and create a feature branch.
3. Add or update functional tests to cover the change.
4. Run `dotnet test` locally before opening a PR.

Feel free to use `EmitCompilerGeneratedFiles=true` in your test project to inspect how new features affect the generated output.

## License
Distributed under the [MIT License](LICENSE.txt).
