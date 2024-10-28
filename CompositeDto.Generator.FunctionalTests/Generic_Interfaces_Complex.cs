using CompositeDto.Generator.Runtime;

namespace CompositeDto.Generator.FunctionalTests;

public partial class Generic_Interfaces_Complex
{
    public interface IInterface1<T>
    {
        T Property1 { get; set; }
    }

    public interface IInterface2<T>
    {
        T Property2 { get; set; }
    }
    
    public interface IInterface3<T> : IInterface1<T>, IInterface2<string>
    {
    }
    
    [CompositeDto]
    public partial class MyDto<T> : IInterface3<int>
    {
        // Here can go extra properties that are not part of the interfaces
    }
}