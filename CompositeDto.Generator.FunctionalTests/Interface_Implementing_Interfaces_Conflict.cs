using CompositeDto.Generator.Runtime;

namespace CompositeDto.Generator.FunctionalTests;

public partial class Interface_Implementing_Interfaces_Conflict
{
    public interface IInterface1
    {
        string Property1 { get; set; }
    }
    
    public interface IInterface2
    {
        string Property1 { get; set; }
    }
    
    public interface IInterface4
    {
        int Property1 { get; set; }
    }
    
    public interface IInterface3 : IInterface1, IInterface2
    {
        string Property3 { get; set; }
    }
  
    
    [CompositeDto]
    public partial class MyDtoX : IInterface3, IInterface4
    {
        // Here can go extra properties that are not part of the interfaces
    }
}