using Api.Contracts;
using Microsoft.AspNetCore.Mvc;
using SqlKata;
using SqlKata.Execution;

namespace Api.Controllers;

[ApiController]
[Route("[controller]")]
public class OlympicWinnersController : ControllerBase
{
    private readonly QueryFactory _db;

    public OlympicWinnersController(QueryFactory db)
    {
        _db = db;
        _db.Logger = compiled => Console.WriteLine(compiled.ToString());
    }

    [HttpPost("winners")]
    public ActionResult<IEnumerable<OlympicWinners>> GetRows(GetRowsRequest request)
    {
        var query = _db.Query("olympic_winners")
                        .Limit(request.EndRow - request.StartRow + 1)
                        .Offset(request.StartRow);

        SelectSql(query, request);
        WhereSql(query, request);
        OrderBySql(query, request);

        var result = query.Get<OlympicWinners>();

        return result.ToList();
    }

    private void SelectSql(Query query, GetRowsRequest request)
    {
        var rowGroupCols = request.RowGroupCols;
        var valueCols = request.ValueCols;
        var groupKeys = request.GroupKeys;

        if (IsDoingGrouping(rowGroupCols, groupKeys))
        {
            var rowGroupCol = rowGroupCols[groupKeys.Count];
            var colsToSelect = new List<string>() { rowGroupCol.Id };

            valueCols.ForEach(valueCol =>
                query.SelectRaw(valueCol.AggFunc + "(\"" + valueCol.Id + "\") AS " + valueCol.Id));

            request.RowGroupCols.Take(request.GroupKeys.Count + 1).ToList()
            .ForEach(r =>
            {
                query.Select(r.Field);
                query.GroupBy(r.Field);
            });
        }
    }

    private void WhereSql(Query query, GetRowsRequest request)
    {
        for (int i = 0; i < request.GroupKeys.Count; i++)
        {
            query.Where(request.RowGroupCols[i].Field, request.GroupKeys[i]);
        }
    }

    private void OrderBySql(Query query, GetRowsRequest request)
    {
        if (request.SortModel.Count == 0)
        {
            return;
        }
        var rowGroupCols = request.RowGroupCols;
        var valueCols = request.ValueCols;
        var groupKeys = request.GroupKeys;

        var isGrouping = IsDoingGrouping(rowGroupCols, groupKeys);

        var groupColIds = request.RowGroupCols.Take(groupKeys.Count + 1)
                            .Select(groupCol => groupCol.Id).ToList();

        var valueColIds = request.ValueCols
                            .ConvertAll(valCol => valCol.Id);

        request.SortModel.ForEach(s =>
        {
            if ( isGrouping && groupColIds.IndexOf(s.ColId) < 0 &&
                 valueColIds.IndexOf(s.ColId) < 0
             )
            {
                // ignore if it is grouping and the request is asking to sort by a column that is not
                // in the group or aggregation columns
            }
            else
            {
                query.OrderByRaw($"\"{s.ColId}\" {s.Sort} nulls last");
            }
        }
        );
    }

    private bool IsDoingGrouping(List<ColumnVO> rowGroupCols, List<string> groupKeys)
    {
        // we are not doing grouping if at the lowest level
        return rowGroupCols.Count > groupKeys.Count;
    }
}