namespace Lodasharp;

using System.Collections;
using System.Collections.Immutable;
using System.Runtime.CompilerServices;
using System.Text.Json.Nodes;

[CollectionBuilder(typeof(JsObject), nameof(Obj))]
public sealed class JsObject : IEnumerable<(String, JsNode)>
{
    private readonly IImmutableDictionary<String, JsNode> _props;

    private JsObject(IImmutableDictionary<String, JsNode> props) => _props = props;
    public static JsNode Create(params ReadOnlySpan<(String, JsNode)> values) => Obj(values);
    public static JsObject Obj(params ReadOnlySpan<(String, JsNode)> values)
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

        var result = new JsObject(props);

        return result;
    }
    public IEnumerator<(String, JsNode)> GetEnumerator() => _props.Select(kvp => (kvp.Key, kvp.Value)).GetEnumerator();
    IEnumerator IEnumerable.GetEnumerator() => ( (IEnumerable)_props ).GetEnumerator();
    public static JsObject FromJson(JsonObject obj)
    {
        var resultProps = ImmutableDictionary.CreateBuilder<String, JsNode>();

        foreach(var (prop, value) in obj)
        {
            resultProps.Add(prop, JsNode.FromJson(value));
        }

        var result = new JsObject(resultProps.ToImmutable());

        return result;
    }
    public JsonNode ToJson() => _props.Aggregate(
        new JsonObject(),
        (acc, value) =>
        {
            acc.Add(value.Key, value.Value.ToJson());
            return acc;
        });
    public JsNode Get(String prop) => _props.TryGetValue(prop, out var value) ? value : new Unit();
    public JsNode With(String prop, JsNode value)
    {
        var isOverwrite = _props.TryGetValue(prop, out var existing);
        if(isOverwrite && existing == value)
            return this;

        var resultProps = _props.SetItem(prop, value);
        var result = new JsObject(resultProps);

        return result;
    }
}