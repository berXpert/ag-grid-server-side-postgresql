using Api;
using Api.Infrastructure;

var builder = WebApplication.CreateBuilder(args);
{
    builder.Services
        .AddInfrastructure(builder.Configuration)
        .AddPresentation();
}

var app = builder.Build();
{
    app.MapControllers();
    app.Run();
}
