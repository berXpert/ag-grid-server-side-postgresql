using Api;

var builder = WebApplication.CreateBuilder(args);
{
    builder.Services
        .AddPresentation(builder.Configuration);
}

var app = builder.Build();
{
    app.MapControllers();
    app.Run();
}
