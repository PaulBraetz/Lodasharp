namespace Tests;

using Lodasharp;

public class JsObjectTests
{
    [Fact]
    public void JsObject_with_creates_property_union_0()
    {
        JsObject obj = [("a", 1)];
        var mutated = obj.With([("b", 2)]);

        Assert.True(mutated.AsJsObject!.Get("a").Equals(1));
        Assert.True(mutated.AsJsObject!.Get("b").Equals(2));
    }
    [Fact]
    public void JsObject_with_creates_property_union_1()
    {
        JsObject obj = [("a", 1), ("c", 3)];
        var mutated = obj.With([("b", 2)]);

        Assert.True(mutated.AsJsObject!.Get("a").Equals(1));
        Assert.True(mutated.AsJsObject!.Get("b").Equals(2));
        Assert.True(mutated.AsJsObject!.Get("c").Equals(3));
    }
    [Fact]
    public void JsObject_with_creates_property_union_2()
    {
        JsObject obj = [("a", 1), ("c", 3)];
        var mutated = obj.With([("b", 2), ("d", 4)]);

        Assert.True(mutated.AsJsObject!.Get("a").Equals(1));
        Assert.True(mutated.AsJsObject!.Get("b").Equals(2));
        Assert.True(mutated.AsJsObject!.Get("c").Equals(3));
        Assert.True(mutated.AsJsObject!.Get("d").Equals(4));
    }
    [Fact]
    public void JsObject_with_creates_property_union_with_overrides()
    {
        JsObject obj = [("a", 1), ("c", 3)];
        var mutated = obj.With([("b", 2), ("c", 4)]);

        Assert.True(mutated.AsJsObject!.Get("a").Equals(1));
        Assert.True(mutated.AsJsObject!.Get("b").Equals(2));
        Assert.True(mutated.AsJsObject!.Get("c").Equals(4));
    }
}
