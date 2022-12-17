using Npgsql;

namespace Api.Infrastructure;

public static class DependencyInjection
{
    public static IServiceCollection AddInfrastructure(
        this IServiceCollection services,
        IConfiguration configuration)
    {
        services.AddNpgsqlDataSource(configuration["db_connection"]!);
        return services;
    }
}