using System.CodeDom.Compiler;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading;

using CompositeDto.Generator.Helpers;

using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Microsoft.CodeAnalysis.Text;

using System;
using System.Collections.Generic;

namespace CompositeDto.Generator;

[Generator]
public class CompositeDtoGenerator : IIncrementalGenerator
{
    public void Initialize(IncrementalGeneratorInitializationContext context)
    {
        var provider = context.SyntaxProvider
            .ForAttributeWithMetadataName(
                "CompositeDto.Generator.Runtime.CompositeDtoAttribute",
                (syntaxNode, ct) => syntaxNode is ClassDeclarationSyntax,
                (syntaxContext, ct) => new Transformer(syntaxContext, ct).Transform()
            );

        context.RegisterSourceOutput(provider, (spc, source) =>
        {
            if (source is null)
                return;

            Execute(source, spc);
        });
    }

    private static void Execute(DtoTarget target, SourceProductionContext context)
    {
        if (target?.Class is null)
            return;

        var @class = target.Class;

        using var buffer = new StringWriter(new StringBuilder(capacity: 4096));

        Dictionary<string, PropertyModel> addedProperties = new();
        var lookup = GetLookup();

        {
            using var writer = new IndentedTextWriter(buffer);

            writer.WriteLine("using System;");
            writer.WriteLine($"namespace {target.Namespace}");
            writer.AppendOpenBracket();

            if (target.OuterClasses.Length > 0)
            {
                foreach (var outerClass in target.OuterClasses)
                {
                    writer.WriteLine($"partial class {outerClass.Name}");
                    writer.AppendOpenBracket();
                }
            }

            // Because the interfaces are already stated on the main part of the class, we dont need to add them here
            writer.WriteLine($"partial class {@class.Name}");
            writer.AppendOpenBracket();


            foreach (var iface in @class.Interfaces)
            {
                writer.WriteLine($"// Properties from {iface.Name}");
                foreach (var property in iface.Properties)
                {
                    if (addedProperties.TryGetValue(property.Name, out var alreadyAdded))
                    {
                        // Name already in use
                        if (alreadyAdded.Type == property.Type)
                        {
                            // Type equals, lets skip this
                            // write a note
                            writer.WriteLine($"// Property {property.Name} skipped: already exists with the same type");
                            continue;
                        }

                        // Type doesnt match, lets explicitly implement the interface
                        // eg: string IInterface.Property { get; set; }

                        // docs first
                        writer.WriteXmlDocComment(GetDocs(property));
                        writer.WriteLine($"{property.Type} {iface.Name}.{property.Name} {{ get; set; }}");
                        continue;
                    }

                    // docs first
                    writer.WriteXmlDocComment(GetDocs(property));
                    writer.WriteLine($"public {property.Type} {property.Name} {{ get; set; }}");
                    addedProperties.Add(property.Name, property);
                }
                // Assume the interface properties would be gathered here
                // This template is to show how you might format the generation logic
            }

            writer.UnwindOpenedBrackets();
        }

        var nameSpace = target.Namespace;
        var outerClassNames = target.OuterClasses.Select(x => x.Name);
        var className = @class.Name;

        var fileName =
            StringHelper.EscapeFileName($"{nameSpace}.{string.Join(".", outerClassNames)}.{className}_CompositeDto.g.cs");

        context.AddSource(fileName, SourceText.From(buffer.ToString(), Encoding.UTF8));

        string? GetDocs(PropertyModel property)
        {
            if (!string.IsNullOrEmpty(property.Docs))
            {
                return property.Docs;
            }

            return lookup.Value[(property.Name, property.Type)]
                .FirstOrDefault(xx => !string.IsNullOrEmpty(xx.Docs))
                ?.Docs;
        }

        Lazy<ILookup<(string Name, string Type), PropertyModel>> GetLookup()
        {
            return new
            (
                () => @class.Interfaces
                .SelectMany(x => x.Properties)
                .ToLookup(x => (x.Name, x.Type))
            );
        }
    }
}