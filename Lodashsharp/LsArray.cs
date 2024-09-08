namespace Lodasharp;

using System.Collections;
using System.Collections.Immutable;
using System.Text.Json.Nodes;

public sealed class LsArray : IEnumerable<LsNode>
{
    LsArray(ImmutableArray<LsNode> values) => _values = values;
    private readonly ImmutableArray<LsNode> _values;

    public static LsArray Arr(params ReadOnlySpan<LsNode> values) => new(values.ToImmutableArray());
    public IEnumerator<LsNode> GetEnumerator() => ( (IEnumerable<LsNode>)_values ).GetEnumerator();
    IEnumerator IEnumerable.GetEnumerator() => ( (IEnumerable)_values ).GetEnumerator();

    public LsNode Get(Int32 index) => index < _values.Length && index >= 0 ? _values[index] : new Unit();
    public LsNode With(Int32 index, LsNode value)
    {
        var paddingWidth = index - _values.Length;
        var padding = paddingWidth > 0
            ? Enumerable.Repeat((LsNode)new Unit(), paddingWidth)
            : [];

        var newItems = index >= _values.Length
            ? [.. _values, .. padding, value]
            : _values.SetItem(index, value);
        var result = new LsArray(newItems);

        return result;
    }
    public static LsArray FromJson(JsonArray arr)
    {
        var resultValues = ImmutableArray.CreateBuilder<LsNode>();

        foreach(var value in arr)
        {
            resultValues.Add(LsNode.FromJson(value));
        }

        var result = new LsArray(resultValues.ToImmutable());

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
