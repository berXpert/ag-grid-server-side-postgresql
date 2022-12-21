using System.Text.Json.Serialization;
using Api.Common;

namespace Api.Contracts;

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