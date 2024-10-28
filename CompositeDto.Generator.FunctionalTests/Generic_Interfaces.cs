using CompositeDto.Generator.Runtime;

namespace CompositeDto.Generator.FunctionalTests;

public partial class Generic_Interfaces
{
    public interface IInterface1<T>
    {
        T Property1 { get; set; }
    }

    public interface IInterface2<T>
    {
        T Property2 { get; set; }
    }
    
    [CompositeDto]
    public partial class MyDto : IInterface1<Guid>, IInterface1<int>
    {
        // Here can go extra properties that are not part of the interfaces
    }
}