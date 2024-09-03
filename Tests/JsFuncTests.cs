namespace Tests;

using System.ComponentModel;

using Lodasharp;

using Microsoft.VisualStudio.TestPlatform.Common.Filtering;

public class JsFuncTests
{
    [Fact]
    public void This_parameter_is_contextual_direct_parent()
    {
        JsNode expectedThis = [("a","foo")];
        JsFunc identity = (@this,_)=>@this;
        expectedThis = expectedThis.With("identity", identity);

        var actualThis = expectedThis.Call("identity", new Unit());

        Assert.Equal(expectedThis, actualThis);
    }
    [Fact]
    public void This_parameter_is_contextual_nested_parent()
    {
        JsNode expectedThis = [("a", "foo")];
        JsFunc identity = (@this, _) => @this;
        expectedThis = expectedThis.With("identity", identity);

        JsNode container = [("nested", expectedThis)];

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
        String expectedThisJson,
        String nestedPath)
    {
        JsNode expectedThis = expectedThisJson;
        JsFunc identity = (@this, _) => @this;
        expectedThis = expectedThis.With("identity", identity);

        var container = ((JsNode)[]).With(nestedPath, expectedThis);

        var actualThis = container.Call($"{nestedPath}.identity", new Unit());

        Assert.Equal(expectedThis, actualThis);
    }
}