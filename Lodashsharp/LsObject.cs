namespace Lodasharp;

using System.Collections;
using System.Collections.Immutable;
using System.Runtime.CompilerServices;
using System.Text.Json.Nodes;

[CollectionBuilder(typeof(LsObject), nameof(Obj))]
public sealed class LsObject : IEnumerable<(String, LsNode)>
{
    private readonly IImmutableDictionary<String, LsNode> _props;

    private LsObject(IImmutableDictionary<String, LsNode> props) => _props = props;
    public static LsNode Create(params ReadOnlySpan<(String, LsNode)> values) => Obj(values);
    public static LsObject Obj(params ReadOnlySpan<(String, LsNode)> values)
    {
        var props = values
            .ToArray()
            .Select(t =>
            {
                foreach(var c in t.Item1)
                {
                    if(!Char.IsLetter(c))
                        throw new ArgumentException($"{nameof(values)} contains invalid property name: {t.Item1}.", nameof(values));
                }

                return KeyValuePair.Create(t.Item1, t.Item2);
            })
            .ToImmutableDictionary();

        var result = new LsObject(props);

        return result;
    }
    public IEnumerator<(String, LsNode)> GetEnumerator() => _props.Select(kvp => (kvp.Key, kvp.Value)).GetEnumerator();
    IEnumerator IEnumerable.GetEnumerator() => ( (IEnumerable)_props ).GetEnumerator();
    public static LsObject FromJson(JsonObject obj)
    {
        var resultProps = ImmutableDictionary.CreateBuilder<String, LsNode>();

        foreach(var (prop, value) in obj)
        {
            resultProps.Add(prop, LsNode.FromJson(value));
        }

        var result = new LsObject(resultProps.ToImmutable());

        return result;
    }
    public JsonNode ToJson() => _props.Aggregate(
        new JsonObject(),
        (acc, value) =>
        {
            acc.Add(value.Key, value.Value.ToJson());
            return acc;
        });
    public LsNode Get(String prop) => _props.TryGetValue(prop, out var value) ? value : new Unit();
    public LsNode With(String prop, LsNode value)
    {
        var isOverwrite = _props.TryGetValue(prop, out var existing);
        if(isOverwrite && existing == value)
            return this;

        var resultProps = _props.SetItem(prop, value);
        var result = new LsObject(resultProps);

        return result;
    }
    public LsNode With(IEnumerable<(String, LsNode)> props)
    {
        var kvps = props.Select(t => KeyValuePair.Create(t.Item1, t.Item2));
        var result = WithCore(kvps);

        return result;
    }
    private LsObject WithCore(IEnumerable<KeyValuePair<String, LsNode>> kvps)
    {
        var resultProps = _props.SetItems(kvps);
        var result = new LsObject(resultProps);

        return result;
    }
    public LsNode With(IEnumerable<LsNode> nodes) => With(nodes.SelectMany(n => n));
    private LsObject WithCore(LsNode node)
    {
        var result = node.TryAsLsObject(out var obj)
            ? WithCore(obj._props)
            : WithCore([("", node)]);

        return result;
    }
    public LsNode With(params ReadOnlySpan<LsNode> nodes)
    {
        var result = this;
        foreach(var node in nodes)
            result = result.WithCore(node);

        return result;
    }
}