using Microsoft.AspNetCore.Mvc;
using SqlKata.Execution;

namespace Api.Controllers;

[ApiController]
[Route("[controller]")]
public class OlympicController : ControllerBase
{
    private readonly QueryFactory _db;

    public OlympicController(QueryFactory db)
    {
        _db = db;

        _db.Logger = compiled => {
            Console.WriteLine(compiled.ToString());
        };
    }
}