using Microsoft.Extensions.Hosting;
using QueryBuilder;

var host = new HostBuilder()
    .ConfigureFunctionsWorkerDefaults()
    .ConfigureServices((hostContext, serviceCollection)
        => serviceCollection.AddInfrastructure(hostContext.Configuration))
    .Build();

host.Run();
