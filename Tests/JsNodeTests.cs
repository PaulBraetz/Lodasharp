namespace Tests;
using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Linq;

using Lodasharp;

public class JsNodeTests
{
    [Fact]
    public void JsNode_with_invalid_json_returns_new_string_value()
    {
        JsNode node = "unit";

        Assert.True(node.IsString);
        Assert.Equal("unit", node.AsString);
    }
    [Fact]
    public void JsNode_parses_JsObject_from_json_object_string_literal()
    {
        JsNode node = """{"foo":"bar"}""";

        Assert.True(node.IsJsObject);
        Assert.Equal(JsObject.Create([("foo", "bar")]), node);
    }
    [Fact]
    public void JsNode_parses_string_value_from_json_object_string_literal_when_prefixed_with_dollar()
    {
        JsNode node = """${"foo":"bar"}""";

        Assert.True(node.IsString);
        Assert.Equal("""{"foo":"bar"}""", node.AsString);
    }
}
