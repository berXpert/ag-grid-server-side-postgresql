using System.Text.Json.Serialization;
using Api.Common;

namespace Api.Contracts;

public record class GetRowsRequest
{
    public int StartRow { get; set; }
    public int EndRow { get; set; }

    public List<ColumnVO> RowGroupCols { get; set; } = null!;
    public List<ColumnVO> ValueCols { get; set; } = null!;

    public List<ColumnVO> PivotCols { get; set; } = null!;

    public bool PivotMode { get; set; }

    public List<string> GroupKeys { get; set; } = null!;

    public List<SortModel> SortModel { get; set; } = null!;

    public Dictionary<string, ColumnFilter> FilterModel { get; set; } = null!;
}

public class ColumnVO
{
    public string Id { get; set; } = null!;
    public string DisplayName { get; set; } = null!;
    public string Field { get; set; } = null!;
    public string? AggFunc { get; set; }
}

public class SortModel
{
    public string ColId { get; set; } = null!;
    public string Sort { get; set; } = null!;
}

public class ColumnFilter
{
    public string Type { get; set; } = null!;
    public string FilterType { get; set; } = null!;

    [JsonConverter(typeof(AutoNumberToStringConverter))]
    public string Filter { get; set; } = null!;

    [JsonConverter(typeof(AutoNumberToStringConverter))]
    public string? FilterTo { get; set; }
    public List<string>? Values { get; set; }
}