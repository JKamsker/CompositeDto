using System;

using Xunit;

namespace CompositeDto.Generator.Tests;

public class Tests
{
    [Fact]
    public void Test1()
    {
        var source =
            """
            using CompositeDto.Generator.Runtime;

            namespace CompositeDto.Generator.Tests;

            public interface IInterface1
            {
                string Property1 { get; set; }
                int Property2 { get; set; }
            }

            public interface IInterface2
            {
                string Property3 { get; set; }
                int Property4 { get; set; }
            }

            [CompositeDto]
            // Optional: [CompositionSource(typeof(IInterface1), typeof(IInterface2))] // If set, you can only include the properties of the specified interfaces
            public partial class MyDto : IInterface1, IInterface2
            {
                // Here can go extra properties that are not part of the interfaces
            }
            """;

        var res = GeneratorTestHelper.RunGenerator(source);

        var gensource = res.Results
                .SelectMany(x => x.GeneratedSources)
                .Select(x => x.SourceText)
                .ToList()
            ;
        Assert.True(true);
    }

    [Fact]
    public void Test2_Partial()
    {
        var source =
            """
            using CompositeDto.Generator.Runtime;

            namespace CompositeDto.Generator.Tests;

            public interface IInterface1
            {
                string Property1 { get; set; }
                int Property2 { get; set; }
            }

            public interface IInterface2
            {
                string Property3 { get; set; }
                int Property4 { get; set; }
            }

            public partial class Wrapper
            {
                [CompositeDto]
                public partial class MyDto : IInterface1, IInterface2
                {
                    // Here can go extra properties that are not part of the interfaces
                }
            }
            """;

        var res = GeneratorTestHelper.RunGenerator(source);

        var gensource = res.Results
                .SelectMany(x => x.GeneratedSources)
                .Select(x => x.SourceText)
                .ToList()
            ;
    }

    [Fact]
    public void Class_Without_Interfaces_Should_Not_Generate_Source()
    {
        var source =
            """
            using CompositeDto.Generator.Runtime;

            namespace CompositeDto.Generator.Tests;


            [CompositeDto]
            public partial class MyDto //: IInterface1, IInterface2
            {
                // Here can go extra properties that are not part of the interfaces
            }
            """;

        var res = GeneratorTestHelper.RunGenerator(source);

        // var gensource = res.Results
        //         .SelectMany(x => x.GeneratedSources)
        //         .Select(x => x.SourceText)
        //         .ToList()
        //     ;

        Assert.Empty(res.Results.SelectMany(x => x.GeneratedSources));
    }

    [Fact]
    public void __Test()
    {
        var source =
            """
            using CompositeDto.Generator.Runtime;

            namespace CompositeDto.Generator.Tests;

            public interface IInterface1<T>
            {
                T Property1 { get; set; }
            }

            [CompositeDto]
            public partial class MyDto : IInterface1<Guid>, IInterface1<int>
            {
                // Here can go extra properties that are not part of the interfaces
            }
            """;

        var res = GeneratorTestHelper.RunGenerator(source);


        var sources = res.Results
            .SelectMany(x => x.GeneratedSources)
            .Select(x => x.SourceText.ToString())
            .ToList();

        var generated = sources[0];
    }

    [Fact]
    public void __Test2()
    {
        var source =
            """
            public interface IInterface1<T>
            {
                T Property1 { get; set; }
            }

            [CompositeDto]
            public partial class MyDto<T> : IInterface1<T>
            {
                // Here can go extra properties that are not part of the interfaces
            }
            """;

        var res = GeneratorTestHelper.RunGenerator(source);

        var sources = res.Results
            .SelectMany(x => x.GeneratedSources)
            .Select(x => x.SourceText.ToString())
            .ToList();

        var generated = sources[0];
    }


    /// <summary>
    /// Test if the docs have been copied in the right format
    /// </summary>
    [Fact]
    public void __Test3()
    {
        var source =
            """
            using CompositeDto.Generator.Runtime;

            namespace CompositeDto.Generator.Tests;

            public interface IInterface1
            {
                ///<summary>
                ///     The first property
                ///</summary>
                string Property1 { get; set; }
            }


            [CompositeDto]
            public partial class MyDto : IInterface1;
            """;

        var res = GeneratorTestHelper.RunGenerator(source);

        var gensource = res.Results
            .SelectMany(x => x.GeneratedSources)
            .Select(x => x.SourceText)
            .ToList();

        var generated = gensource[0].ToString();
        Assert.Contains("<summary>", generated);
        Assert.Contains("The first property", generated);
    }
}