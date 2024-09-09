namespace Tests;

using Lodasharp;

public class LsFuncTests
{
    [Fact]
    public void This_parameter_is_contextual_direct_parent()
    {
        LsNode expectedThis = [("a","foo")];
        LsFunc identity = (@this,_)=>@this;
        expectedThis = expectedThis.With("identity", identity);

        var actualThis = expectedThis.Call("identity", new Unit());

        Assert.Equal(expectedThis, actualThis);
    }
    [Fact]
    public void This_parameter_is_contextual_nested_parent()
    {
        LsNode expectedThis = [("a", "foo")];
        LsFunc identity = (@this, _) => @this;
        expectedThis = expectedThis.With("identity", identity);

        LsNode container = [("nested", expectedThis)];

        var actualThis = container.Call("nested.identity", new Unit());

        Assert.Equal(expectedThis, actualThis);
    }
    [Theory]
    [InlineData("""{"a":"foo"}""", "nested")]
    [InlineData("32", "nested")]
    [InlineData("true", "nested")]
    [InlineData("$true", "nested")]
    [InlineData("""{"a":"foo"}""", "nested.nestedAlso")]
    public void This_parameter_is_contextual_nested_parent_1(
        String expectedThisLson,
        String nestedPath)
    {
        LsNode expectedThis = expectedThisLson;
        LsFunc identity = (@this, _) => @this;
        expectedThis = expectedThis.With("identity", identity);

        var container = ((LsNode)[]).With(nestedPath, expectedThis);

        var actualThis = container.Call($"{nestedPath}.identity", new Unit());

        Assert.Equal(expectedThis, actualThis);
    }
}