using System.Text.Json;
using System.Text.Json.Serialization;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using QueryBuilder;

var host = new HostBuilder()
    .ConfigureFunctionsWorkerDefaults()
    .ConfigureServices((hostContext, serviceCollection)
        => serviceCollection.AddInfrastructure(hostContext.Configuration))
    .ConfigureServices(services =>
                {
                    services.Configure<JsonSerializerOptions>(options =>
                    {
                        options.PropertyNamingPolicy = JsonNamingPolicy.CamelCase;
                        options.Converters.Add(new JsonStringEnumConverter());
                    });
                })
    .Build();

host.Run();
