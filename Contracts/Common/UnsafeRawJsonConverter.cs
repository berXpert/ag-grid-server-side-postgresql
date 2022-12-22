namespace Contracts.Common;

/// <summary>
/// Serializes the contents of a string value as raw JSON.  The string is NOT validated as being an RFC 8259-compliant JSON payload
/// </summary>
public class UnsafeRawJsonConverter : RawJsonConverter
{
    protected override bool SkipInputValidation => true;
}