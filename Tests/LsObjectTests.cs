namespace Tests;

using Lodasharp;

public class LsObjectTests
{
    [Fact]
    public void LsObject_equals_structurally_equivalent_object()
    {
        LsObject a = [("a", 1), ("b", 2)];
        LsObject b = [("a", 1), ("b", 2)];

        Assert.Equal(a, b);
    }
    [Fact]
    public void LsObject_not_equals_structurally_inequivalent_object()
    {
        LsObject a = [("a", 1), ("b", 2)];
        LsObject b = [("a", 1)];

        Assert.NotEqual(a, b);
    }
    [Fact]
    public void LsObject_equals_structurally_equivalent_object_out_of_order()
    {
        LsObject a = [("a", 1), ("b", 2)];
        LsObject b = [("b", 2), ("a", 1)];

        Assert.Equal(a, b);
    }
    [Fact]
    public void LsObject_equals_structurally_equivalent_object_nested()
    {
        LsObject a = [("a", 1), ("b", [("c", 3)])];
        LsObject b = [("a", 1), ("b", [("c", 3)])];

        Assert.Equal(a, b);
    }
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
