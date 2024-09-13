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
using System.Collections.Immutable;
using System.Globalization;

[UnionType<String, Int32, Boolean, Double, LsObject, LsArray, LsFunc, Unit>]
[UnionType<DateTimeOffset, TimeSpan>]
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

    public static LsNode Create([StringSyntax(StringSyntaxAttribute.Json)] [UnionTypeFactory] String value)
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
    public LsNode At(params ReadOnlySpan<Path> paths)
    {
        var valuesBuilder = ImmutableArray.CreateBuilder<LsNode>();
        foreach(var path in paths)
        {
            var value = Get(path);
            valuesBuilder.Add(value);
        }

        var values = valuesBuilder.ToImmutable();
        var result = Arr(values);

        return result;
    }
    public LsNode At(params IEnumerable<Path> paths)
    {
        var values = paths.Select(Get);
        var result = Arr(values);

        return result;
    }
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
    public LsNode Bind(Func<String, LsNode>? onString = null, Func<LsNode, LsNode>? onElse = null) => Bind<String>(onString, onElse);
    public LsNode Bind(Func<Unit, LsNode>? onUnit = null, Func<LsNode, LsNode>? onElse = null) => Bind<Unit>(onUnit, onElse);
    public LsNode Bind(Func<Int32, LsNode>? onInt32 = null, Func<LsNode, LsNode>? onElse = null) => Bind<Int32>(onInt32, onElse);
    public LsNode Bind(Func<Boolean, LsNode>? onBoolean = null, Func<LsNode, LsNode>? onElse = null) => Bind<Boolean>(onBoolean, onElse);
    public LsNode Bind(Func<Double, LsNode>? onDouble = null, Func<LsNode, LsNode>? onElse = null) => Bind<Double>(onDouble, onElse);
    public LsNode Bind(Func<LsObject, LsNode>? onLsObject = null, Func<LsNode, LsNode>? onElse = null) => Bind<LsObject>(onLsObject, onElse);
    public LsNode Bind(Func<LsArray, LsNode>? onLsArray = null, Func<LsNode, LsNode>? onElse = null) => Bind<LsArray>(onLsArray, onElse);
    public LsNode Bind(Func<LsFunc, LsNode>? onLsFunc = null, Func<LsNode, LsNode>? onElse = null) => Bind<LsFunc>(onLsFunc, onElse);
    public LsNode Bind(Func<DateTimeOffset, LsNode>? onDateTimeOffset = null, Func<LsNode, LsNode>? onElse = null) => Bind<DateTimeOffset>(onDateTimeOffset, onElse);
    public LsNode Bind(Func<TimeSpan, LsNode>? onTimeSpan = null, Func<LsNode, LsNode>? onElse = null) => Bind<TimeSpan>(onTimeSpan, onElse);
    public LsNode Bind<T>(Func<T, LsNode>? onValue = null, Func<LsNode, LsNode>? onElse = null)
    {
        var isValue = Is<T>();

        if(onValue is { } && isValue)
            return onValue.Invoke(As<T>());

        if(onElse is { } && !isValue)
            return onElse.Invoke(this);

        return this;
    }
    public static LsNode FromJson(JsonNode? node)
    {
        LsNode result = node switch
        {
            JsonArray arr => LsArray.FromJson(arr),
            JsonObject obj => LsObject.FromJson(obj),
            JsonValue val =>
                val.TryGetValue(out Int32 i) ? i :
                val.TryGetValue(out Boolean b) ? b :
                val.TryGetValue(out Double d) ? d :
                val.TryGetValue(out DateTimeOffset dateTime) ? dateTime :
                val.TryGetValue(out String? s) ? s is null ? new Unit() :
                    TimeSpan.TryParse(s, CultureInfo.InvariantCulture, out var ts) ? ts : s :
                new Unit(),
            _ => new Unit()
        };

        return result;
    }
    public JsonNode? ToJson() => Match<JsonNode?>(
        dateTime => JsonValue.Create(dateTime),
        timeSpan => JsonValue.Create(timeSpan.ToString("G", CultureInfo.InvariantCulture)),
        @string => JsonValue.Create(@string),
        @int => JsonValue.Create(@int),
        @bool => JsonValue.Create(@bool),
        @double => JsonValue.Create(@double),
        @object => @object.ToJson(),
        array => array.ToJson(),
        func => null,
        unit => null);
    static readonly JsonSerializerOptions _options = new() { WriteIndented = true, AllowTrailingCommas = true, PropertyNamingPolicy = JsonNamingPolicy.CamelCase };
    public override String ToString() => JsonSerializer.Serialize(this, _options);
}
