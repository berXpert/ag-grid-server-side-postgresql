using Api;
using Contracts;
using QueryBuilder;
using SqlKata.Execution;
using Api.Common;

var builder = WebApplication.CreateBuilder(args);
{
    builder.Services
        .AddPresentation()
        .AddInfrastructure(builder.Configuration);
}

var handleForTable = (string table,
   QueryFactory connection,
    GridRowsRequest request)
    => new PostgreSqlQueryBuilder().Build(table, connection, request);

var curried = handleForTable.Curry();

var handleForTableWired = curried("olympic_winners")(builder.Services.BuildServiceProvider().GetService<QueryFactory>());

var app = builder.Build();
{
    app.MapPost("/OlympicWinners/winners",
        handleForTableWired
    );

    app.Run();
}
