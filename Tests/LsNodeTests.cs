namespace Tests;

using Lodasharp;

using static Lodasharp.LsArray;

public class LsNodeTests
{
    [Fact]
    public void LsNode_at_yields_array()
    {
        LsNode node = [];

        var actual = node.At();

        Assert.True(actual.IsLsArray);
        Assert.Empty(actual.AsLsArray);
    }
    [Fact]
    public void LsNode_at_yields_properties()
    {
        LsNode node = [("a", 1), ("b", 2), ("c", 3)];

        var actual = node.At("a", "c");

        Assert.True(actual.IsLsArray);
        Assert.Equal(actual.AsLsArray, Arr(1, 3));
    }
    [Fact]
    public void LsNode_at_yields_nested_properties()
    {
        LsNode node = [("a", 1), ("b", Arr([("d", 2)])), ("c", 3)];

        var actual = node.At("b.0.d", "a");

        Assert.True(actual.IsLsArray);
        Assert.Equal(actual.AsLsArray, Arr(2, 1));
    }
    [Fact]
    public void LsNode_with_invalid_Lson_returns_new_string_value()
    {
        LsNode node = "unit";

        Assert.True(node.IsString);
        Assert.Equal("unit", node.AsString);
    }
    [Fact]
    public void LsNode_parses_LsObject_from_Lson_object_string_literal()
    {
        LsNode node = """{"foo":"bar"}""";

        Assert.True(node.IsLsObject);
        Assert.Equal(LsObject.Create([("foo", "bar")]), node);
    }
    [Fact]
    public void LsNode_parses_string_value_from_Lson_object_string_literal_when_prefixed_with_dollar()
    {
        LsNode node = """${"foo":"bar"}""";

        Assert.True(node.IsString);
        Assert.Equal("""{"foo":"bar"}""", node.AsString);
    }
}
