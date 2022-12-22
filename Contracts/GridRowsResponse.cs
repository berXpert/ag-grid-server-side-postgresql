using System.Text.Json.Serialization;
using Contracts.Common;

namespace Contracts;

public record class GridRowsResponse
{
    [JsonConverter(typeof(UnsafeRawJsonConverter))]
    public string Data { get; set; }
    public long LastRow { get; set; }
    public List<string> SecondaryColumns { get; set; }

    public GridRowsResponse()
    {
        Data = "[]";
        LastRow = 0;
        SecondaryColumns = new List<string>();
    }
}