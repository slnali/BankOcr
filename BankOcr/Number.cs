
using System.Numerics;

public class Number : INumber
{
    public int Value { get; set; }
}

public class PossibleNumber : INumber
{
    public List<int> Value { get; set; }
}

public interface INumber
{
}
