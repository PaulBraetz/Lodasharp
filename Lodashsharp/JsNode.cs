namespace Lodasharp;

using System.Collections;
using System.Runtime.CompilerServices;
using System.Text;

using RhoMicro.CodeAnalysis;

using static JsArray;
using static JsObject;
using System.Text.Json.Nodes;
using System.Text.Json.Serialization;
using System.Text.Json;
using System.Diagnostics.CodeAnalysis;

[UnionType<String, Int32, Boolean, Double, JsObject, JsArray, JsFunc, Unit>]
[UnionTypeSettings(ToStringSetting = ToStringSetting.None)]
[CollectionBuilder(typeof(JsObject), nameof(JsObject.Create))]
[JsonConverter(typeof(Converter))]
public readonly partial struct JsNode : IEnumerable<(String, JsNode)>
{
    sealed class Converter : JsonConverter<JsNode>
    {
        public override JsNode Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options)
        {
            var node = JsonNode.Parse(ref reader);
            var result = FromJson(node);

            return result;
        }
        public override void Write(Utf8JsonWriter writer, JsNode value, JsonSerializerOptions options)
        {
            var obj = value.ToJson();
            if(obj is null)
                writer.WriteRawValue("unit", skipInputValidation: true);
            else
                obj.WriteTo(writer, options);
        }
    }

    public static JsNode Create([UnionTypeFactory] String value)
    {
        if(value.StartsWith('$'))
            return new JsNode(value[1..]);

        try
        {
            return JsonSerializer.Deserialize<JsNode>(value, _options);
        } catch(JsonException)
        {
            return new JsNode(value);
        }
    }

    public IEnumerator<(String, JsNode)> GetEnumerator() =>
        ( IsUnit
        ? []
        : IsJsObject
        ? AsJsObject
        : [("", this)] ).GetEnumerator();

    IEnumerator IEnumerable.GetEnumerator() => ( (IEnumerable<(String, JsNode)>)this ).GetEnumerator();
    private JsNode GetCore(Path path, out JsNode parent)
    {
        if(path.TryAsInt32(out var index))
        {

            if(TryAsJsArray(out var arr))
            {
                parent = this;
                return arr.Get(index);
            }

            parent = new Unit();
            return new Unit();
        }

        var result = GetFromStringPath(path.AsString!, out parent);

        return result;
    }
    private JsNode GetFromStringPath(String path, out JsNode parent)
    {
        parent = new Unit();
        JsNode result = AsJsObject!;
        var i = 0;
        var buffer = new StringBuilder(2);
        while(i < path.Length)
        {
            var current = path[i];
            if(current == '.')
            {
                getProperty(ref parent);

                if(result.IsUnit)
                    return result;
            } else
            {
                _ = buffer.Append(current);
            }

            i++;
        }

        if(buffer.Length > 0)
            getProperty(ref parent);

        return result;

        void getProperty(ref JsNode parent)
        {
            var path = buffer.ToString();
            var pathIsIndex = Int32.TryParse(path, out var index);
            _ = buffer.Clear();

            parent = result;

            result = result.TryAsJsArray(out var arr) && pathIsIndex
                ? arr.Get(index)
                : result.TryAsJsObject(out var jsObj) && !pathIsIndex
                ? jsObj.Get(path)
                : new Unit();
        }
    }
    public JsNode Get(Path path) => GetCore(path, out _);
    public JsNode With(Path path, JsNode value)
    {
        if(path.IsInt32)
        {
            if(!IsJsArray)
                return Arr().With(path.AsInt32, value);

            return AsJsArray.With(path.AsInt32, value);
        }

        var accessor = path.AsString;
        var periodIndex = accessor.IndexOf('.');
        var firstAccessor = periodIndex != -1 ? accessor[..periodIndex] : accessor;

        if(Int32.TryParse(firstAccessor, out var index))
        {
            if(!IsJsArray)
                return Arr().With(index, value);

            return AsJsArray.With(index, value);
        }

        if(periodIndex == -1)
        {
            if(!IsJsObject)
                return [(firstAccessor, value)];

            //leaf
            return AsJsObject.With(firstAccessor, value);
        } else
        {
            var accessorRemainder = accessor[( periodIndex + 1 )..];

            if(!IsJsObject)
                return [(firstAccessor, Obj().With(accessorRemainder, value))];

            //node
            var node = this;

            if(node.AsJsObject.Get(firstAccessor).IsUnit)
            {
                node = node.With(firstAccessor, []);
            }

            var prop = node.AsJsObject!.Get(firstAccessor).With(accessorRemainder, value);

            return node.With(firstAccessor, prop);
        }
    }
    public JsNode Call(JsNode arg) => TryAsJsFunc(out var f) ? f.Invoke(new Unit(), arg) : new Unit();
    public JsNode Call(Path path, JsNode arg) => GetCore(path, out var parent).TryAsJsFunc(out var f) ? f.Invoke(parent, arg) : new Unit();
    public static JsNode FromJson(JsonNode? node)
    {
        JsNode result = node switch
        {
            JsonArray arr => JsArray.FromJson(arr),
            JsonObject obj => JsObject.FromJson(obj),
            JsonValue val =>
                val.TryGetValue(out String? s) ? s is null ? new Unit() : s :
                val.TryGetValue(out Int32 i) ? i :
                val.TryGetValue(out Boolean b) ? b :
                val.TryGetValue(out Double d) ? d :
                new Unit(),
            _ => new Unit()
        };

        return result;
    }
    public JsonNode? ToJson() => Match<JsonNode?>(
        s => JsonValue.Create(s),
        i => JsonValue.Create(i),
        b => JsonValue.Create(b),
        d => JsonValue.Create(d),
        c => c.ToJson(),
        v => v.ToJson(),
        f => null,
        u => null);
    static readonly JsonSerializerOptions _options = new() { WriteIndented = true, AllowTrailingCommas = true, PropertyNamingPolicy = JsonNamingPolicy.CamelCase };
    public override String ToString() => JsonSerializer.Serialize(this, _options);
    public static JsNode FromString([StringSyntax(StringSyntaxAttribute.Json)] String json) => JsonSerializer.Deserialize<JsNode>(json, _options);
}
