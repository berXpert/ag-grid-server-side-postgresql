using System.Text.Json;
using System.Text.Json.Serialization;

namespace Contracts.Common;

/// <summary>
/// Serializes the contents of a string value as raw JSON.  The string is validated as being an RFC 8259-compliant JSON payload
/// </summary>
public class RawJsonConverter : JsonConverter<string>
{
    protected virtual bool SkipInputValidation => false;

    public override bool CanConvert(Type objectType)
    {
        return objectType == typeof(string);
    }

    public override string? Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options)
    {
        using var doc = JsonDocument.ParseValue(ref reader);
        return doc.RootElement.GetRawText();
    }

    public override void Write(Utf8JsonWriter writer, string value, JsonSerializerOptions options)
    {
        // skipInputValidation : true will improve performance, but only do this if you are certain the value represents well-formed JSON!
        writer.WriteRawValue(value, skipInputValidation: SkipInputValidation);
    }
}
