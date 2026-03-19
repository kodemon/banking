using Cerbos.Sdk.Builder;

namespace Banking.Api.Identity;

internal sealed class Attributes
{
    private readonly Dictionary<string, AttributeValue> _attributes = new();

    public Attributes String(string key, string value)
    {
        return Add(key, AttributeValue.StringValue(value));
    }

    public Attributes Bool(string key, bool value)
    {
        return Add(key, AttributeValue.BoolValue(value));
    }

    public Attributes Number(string key, double value)
    {
        return Add(key, AttributeValue.DoubleValue(value));
    }

    public Attributes GuidList(string key, IEnumerable<Guid> guids)
    {
        return Add(
            key,
            AttributeValue.ListValue(
                guids.Select(g => AttributeValue.StringValue(g.ToString())).ToArray()
            )
        );
    }

    public Attributes List(string key, params AttributeValue[] values)
    {
        return Add(key, AttributeValue.ListValue(values));
    }

    public Attributes Map(string key, Dictionary<string, AttributeValue> dict)
    {
        return Add(key, AttributeValue.MapValue(dict));
    }

    public Attributes Null(string key)
    {
        return Add(key, AttributeValue.NullValue());
    }

    public Dictionary<string, AttributeValue> Commit() => new(_attributes);

    private Attributes Add(string key, AttributeValue value)
    {
        _attributes[key] = value;
        return this;
    }
}
