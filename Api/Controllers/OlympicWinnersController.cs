using Api.Contracts;
using Microsoft.AspNetCore.Mvc;
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

        if( request.GroupKeys.Count > 0)
        {
            for(int i = 0; i < request.GroupKeys.Count; i++)
            {
                var field = request.RowGroupCols[i].Field;
                query.Select(field);
                query.Where(field, request.GroupKeys[i]);
                query.GroupBy(field);
            }
        }

        request.RowGroupCols.ForEach( row =>
        {
            query.Select(row.Field);
            query.GroupBy(row.Field);
        });

        if( request.SortModel.Count > 0)
        {
            request.SortModel.ForEach( sortModel =>
            {
                if(string.Equals(sortModel.Sort, "desc", StringComparison.OrdinalIgnoreCase))
                {
                    query.OrderByDesc(sortModel.ColId);
                }
                else{
                    query.OrderBy(sortModel.ColId);
                }
            });
        }

        var result = query.Get<OlympicWinners>();

        return result.ToList();
    }
}