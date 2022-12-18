using System.Text.Json.Serialization;
using Api.Common;

namespace Api.Contracts;

public class GetRowsRequest
{
    public int StartRow { get; set; }
    public int EndRow { get; set; }

    public List<ColumnVO> RowGroupCols { get; set; }
    public List<ColumnVO> ValueCols { get; set; }

    public List<ColumnVO> PivotCols { get; set; }

    public bool PivotMode { get; set; }

    public List<string> GroupKeys { get; set; }

    public List<SortModel> SortModel { get; set; }

    public Dictionary<string, ColumnFilter> FilterModel { get; set; }
}

public class ColumnVO
{
    public string Id { get; set; }
    public string DisplayName { get; set; }
    public string Field { get; set; }
    public string? AggFunc { get; set; }
}

public class SortModel
{
    public string ColId { get; set; }
    public string Sort { get; set; }
}

public class ColumnFilter
{
    public string Type { get; set; }
    public string FilterType { get; set; }

    [JsonConverter(typeof(AutoNumberToStringConverter))]
    public string Filter { get; set; }

    [JsonConverter(typeof(AutoNumberToStringConverter))]
    public string? FilterTo { get; set; }
    public List<string>? Values { get; set; }
}