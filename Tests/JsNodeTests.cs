﻿namespace Tests;
using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Linq;

using Lodasharp;

public class LsNodeTests
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
