namespace Lodasharp;

using System.Collections;
using System.Collections.Immutable;
using System.Text.Json.Nodes;

public sealed class LsArray(ImmutableArray<LsNode> values) : IEnumerable<LsNode>, IEquatable<LsArray>
{
    private ImmutableArray<LsNode> Values => values;

    public static LsArray Arr(params ReadOnlySpan<LsNode> values) => new(values.ToImmutableArray());
    public static LsArray Arr(params IEnumerable<LsNode> values) => new(values.ToImmutableArray());
    public IEnumerator<LsNode> GetEnumerator() => ( (IEnumerable<LsNode>)values ).GetEnumerator();
    IEnumerator IEnumerable.GetEnumerator() => ( (IEnumerable)values ).GetEnumerator();

    public static LsArray Empty => new([]);
    
    public LsNode Get(Int32 index) => index < values.Length && index >= 0 ? values[index] : new Unit();
    public LsNode With(Int32 index, LsNode value)
    {
        var paddingWidth = index - values.Length;
        var padding = paddingWidth > 0
            ? Enumerable.Repeat((LsNode)new Unit(), paddingWidth)
            : [];

        var newItems = index >= values.Length
            ? [.. values, .. padding, value]
            : values.SetItem(index, value);
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
    public JsonNode ToJson() => values.Aggregate(
        new JsonArray(),
        (acc, value) =>
        {
            acc.Add(value.ToJson());
            return acc;
        });

    public override Boolean Equals(Object? obj) => obj is not null && Equals(obj as LsArray);
    public Boolean Equals(LsArray? other) => other is not null && Values.SequenceEqual(other.Values);
    public override Int32 GetHashCode() => Values.Aggregate(new HashCode(), (hc, v) =>
    {
        hc.Add(v);
        return hc;
    }).ToHashCode();
}
