using CompositeDto.Generator.Runtime;

namespace CompositeDto.Generator.FunctionalTests;

public partial class BasicTest
{
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
    public partial class MyDto : IInterface1, IInterface2
    {
        // Here can go extra properties that are not part of the interfaces
    }
}