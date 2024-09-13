namespace Tests;

using Lodasharp;
using static Lodasharp.LsArray;

public class LsArrayTests
{
    [Fact]
    public void empty_array_equal_to_empty_creation()
    {
        LsArray exected = Arr([]);
        LsArray empty = LsArray.Empty;
        Assert.Equal(exected, empty);
    }
    
    [Fact]
    public void array_not_equal_to_empty_creation()
    {
        LsArray exected = Arr([1, 2, 3]);
        LsArray empty = LsArray.Empty;
        Assert.NotEqual(exected, empty);
    }
}