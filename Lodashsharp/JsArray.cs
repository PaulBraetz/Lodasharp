namespace Lodasharp;

using System.Collections;
using System.Collections.Immutable;
using System.Text.Json.Nodes;

public sealed class JsArray : IEnumerable<JsNode>
{
    JsArray(ImmutableArray<JsNode> values) => _values = values;
    private readonly ImmutableArray<JsNode> _values;

    public static JsArray Arr(params ReadOnlySpan<JsNode> values) => new(values.ToImmutableArray());
    public IEnumerator<JsNode> GetEnumerator() => ( (IEnumerable<JsNode>)_values ).GetEnumerator();
    IEnumerator IEnumerable.GetEnumerator() => ( (IEnumerable)_values ).GetEnumerator();

    public JsNode Get(Int32 index) => index < _values.Length && index >= 0 ? _values[index] : new Unit();
    public JsNode With(Int32 index, JsNode value)
    {
        var paddingWidth = index - _values.Length;
        var padding = paddingWidth > 0
            ? Enumerable.Repeat((JsNode)new Unit(), paddingWidth)
            : [];

        var newItems = index >= _values.Length
            ? [.. _values, .. padding, value]
            : _values.SetItem(index, value);
        var result = new JsArray(newItems);

        return result;
    }
    public static JsArray FromJson(JsonArray arr)
    {
        var resultValues = ImmutableArray.CreateBuilder<JsNode>();

        foreach(var value in arr)
        {
            resultValues.Add(JsNode.FromJson(value));
        }

        var result = new JsArray(resultValues.ToImmutable());

        return result;
    }
    public JsonNode ToJson() => _values.Aggregate(
        new JsonArray(),
        (acc, value) =>
        {
            acc.Add(value.ToJson());
            return acc;
        });
}
