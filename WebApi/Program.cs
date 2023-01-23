using Api;
using Contracts;
using Microsoft.AspNetCore.Mvc;
using QueryBuilder;

var builder = WebApplication.CreateBuilder(args);
{
    builder.Services
        .AddPresentation()
        .AddInfrastructure(builder.Configuration);
}

var app = builder.Build();
{
    app.MapPost("/api/winners",
        ([FromServices]IPostgreSqlQueryBuilder queryBuilder,
         GridRowsRequest request
         ) => queryBuilder.Build(request, "olympic_winners")
    );

    app.Run();
}
