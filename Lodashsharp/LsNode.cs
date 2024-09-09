namespace Lodasharp;

using System.Collections;
using System.Runtime.CompilerServices;
using System.Text;

using RhoMicro.CodeAnalysis;

using static LsArray;
using static LsObject;
using System.Text.Json.Nodes;
using System.Text.Json.Serialization;
using System.Text.Json;
using System.Diagnostics.CodeAnalysis;

[UnionType<String, Int32, Boolean, Double, LsObject, LsArray, LsFunc, Unit>]
[UnionTypeSettings(ToStringSetting = ToStringSetting.None)]
[CollectionBuilder(typeof(LsObject), nameof(LsObject.Create))]
[JsonConverter(typeof(Converter))]
public readonly partial struct LsNode : IEnumerable<(String, LsNode)>
{
    sealed class Converter : JsonConverter<LsNode>
    {
        public override LsNode Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options)
        {
            var node = JsonNode.Parse(ref reader);
            var result = FromJson(node);

            return result;
        }
        public override void Write(Utf8JsonWriter writer, LsNode value, JsonSerializerOptions options)
        {
            var obj = value.ToJson();
            if(obj is null)
                writer.WriteRawValue("unit", skipInputValidation: true);
            else
                obj.WriteTo(writer, options);
        }
    }

    public static LsNode Create([UnionTypeFactory] String value)
    {
        if(value.StartsWith('$'))
            return new LsNode(value[1..]);

        try
        {
            return JsonSerializer.Deserialize<LsNode>(value, _options);
        } catch(JsonException)
        {
            return new LsNode(value);
        }
    }

    public IEnumerator<(String, LsNode)> GetEnumerator() =>
        ( IsUnit
        ? []
        : IsLsObject
        ? AsLsObject
        : [("", this)] ).GetEnumerator();

    IEnumerator IEnumerable.GetEnumerator() => ( (IEnumerable<(String, LsNode)>)this ).GetEnumerator();
    private LsNode GetCore(Path path, out LsNode parent)
    {
        if(path.TryAsInt32(out var index))
        {

            if(TryAsLsArray(out var arr))
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
    private LsNode GetFromStringPath(String path, out LsNode parent)
    {
        parent = new Unit();
        LsNode result = AsLsObject!;
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

        void getProperty(ref LsNode parent)
        {
            var path = buffer.ToString();
            var pathIsIndex = Int32.TryParse(path, out var index);
            _ = buffer.Clear();

            parent = result;

            result = result.TryAsLsArray(out var arr) && pathIsIndex
                ? arr.Get(index)
                : result.TryAsLsObject(out var lsObj) && !pathIsIndex
                ? lsObj.Get(path)
                : new Unit();
        }
    }
    public LsNode Get(Path path) => GetCore(path, out _);
    public LsNode With(Path path, LsNode value)
    {
        if(path.IsInt32)
        {
            if(!IsLsArray)
                return Arr().With(path.AsInt32, value);

            return AsLsArray.With(path.AsInt32, value);
        }

        var accessor = path.AsString;
        var periodIndex = accessor.IndexOf('.');
        var firstAccessor = periodIndex != -1 ? accessor[..periodIndex] : accessor;

        if(Int32.TryParse(firstAccessor, out var index))
        {
            if(!IsLsArray)
                return Arr().With(index, value);

            return AsLsArray.With(index, value);
        }

        if(periodIndex == -1)
        {
            if(!IsLsObject)
                return [(firstAccessor, value)];

            //leaf
            return AsLsObject.With(firstAccessor, value);
        } else
        {
            var accessorRemainder = accessor[( periodIndex + 1 )..];

            if(!IsLsObject)
                return [(firstAccessor, Obj().With(accessorRemainder, value))];

            //node
            var node = this;

            if(node.AsLsObject.Get(firstAccessor).IsUnit)
            {
                node = node.With(firstAccessor, []);
            }

            var prop = node.AsLsObject!.Get(firstAccessor).With(accessorRemainder, value);

            return node.With(firstAccessor, prop);
        }
    }
    public LsNode Call(LsNode arg) => TryAsLsFunc(out var f) ? f.Invoke(new Unit(), arg) : new Unit();
    public LsNode Call(Path path, LsNode arg) => GetCore(path, out var parent).TryAsLsFunc(out var f) ? f.Invoke(parent, arg) : new Unit();
    public static LsNode FromJson(JsonNode? node)
    {
        LsNode result = node switch
        {
            JsonArray arr => LsArray.FromJson(arr),
            JsonObject obj => LsObject.FromJson(obj),
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
    public static LsNode FromString([StringSyntax(StringSyntaxAttribute.Json)] String json) => JsonSerializer.Deserialize<LsNode>(json, _options);
}
