using Contracts;
using Microsoft.AspNetCore.Mvc;
using QueryBuilder;

namespace Api.Controllers;

[ApiController]
[Route("[controller]")]
public class OlympicWinnersController : ControllerBase
{
    private const string Table = "olympic_winners";
    private readonly IPostgreSqlQueryBuilder _queryBuilder;

    public OlympicWinnersController(IPostgreSqlQueryBuilder queryBuilder)
    {
        _queryBuilder = queryBuilder;
    }

    [HttpPost("winners")]
    public ActionResult<GridRowsResponse> GetRows(GridRowsRequest request)
    {
        return _queryBuilder.Build(request, Table);
    }
}