using Api;

var builder = WebApplication.CreateBuilder(args);
{
    builder.Services
        .AddPresentation();
}

var app = builder.Build();
{
    app.MapControllers();
    app.Run();
}
