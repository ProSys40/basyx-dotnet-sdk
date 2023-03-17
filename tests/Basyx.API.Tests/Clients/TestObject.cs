namespace Basyx.API.Tests.Clients;

public class TestObject
{
    public string? TestValue { get; set; }

    public override bool Equals(object? obj)
    {
        return obj is TestObject @object &&
               TestValue == @object.TestValue;
    }

    public override int GetHashCode()
    {
        return HashCode.Combine(TestValue);
    }
}
