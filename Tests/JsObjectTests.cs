namespace Tests;

using Lodasharp;

public class LsObjectTests
{
    [Fact]
    public void LsObject_with_creates_property_union_0()
    {
        LsObject obj = [("a", 1)];
        var mutated = obj.With([("b", 2)]);

        Assert.True(mutated.AsLsObject!.Get("a").Equals(1));
        Assert.True(mutated.AsLsObject!.Get("b").Equals(2));
    }
    [Fact]
    public void LsObject_with_creates_property_union_1()
    {
        LsObject obj = [("a", 1), ("c", 3)];
        var mutated = obj.With([("b", 2)]);

        Assert.True(mutated.AsLsObject!.Get("a").Equals(1));
        Assert.True(mutated.AsLsObject!.Get("b").Equals(2));
        Assert.True(mutated.AsLsObject!.Get("c").Equals(3));
    }
    [Fact]
    public void LsObject_with_creates_property_union_2()
    {
        LsObject obj = [("a", 1), ("c", 3)];
        var mutated = obj.With([("b", 2), ("d", 4)]);

        Assert.True(mutated.AsLsObject!.Get("a").Equals(1));
        Assert.True(mutated.AsLsObject!.Get("b").Equals(2));
        Assert.True(mutated.AsLsObject!.Get("c").Equals(3));
        Assert.True(mutated.AsLsObject!.Get("d").Equals(4));
    }
    [Fact]
    public void LsObject_with_creates_property_union_with_overrides()
    {
        LsObject obj = [("a", 1), ("c", 3)];
        var mutated = obj.With([("b", 2), ("c", 4)]);

        Assert.True(mutated.AsLsObject!.Get("a").Equals(1));
        Assert.True(mutated.AsLsObject!.Get("b").Equals(2));
        Assert.True(mutated.AsLsObject!.Get("c").Equals(4));
    }
}
