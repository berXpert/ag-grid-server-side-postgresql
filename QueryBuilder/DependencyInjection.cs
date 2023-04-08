using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Npgsql;
using SqlKata.Compilers;
using SqlKata.Execution;

namespace QueryBuilder;

public static class DependencyInjection
{
    public static IServiceCollection AddInfrastructure(
        this IServiceCollection services,
        IConfiguration configuration)
    {
        services.AddNpgsqlDataSource(configuration["db_connection"]!);
        services.AddScoped<IQueryFactoryWrapper, QueryFactoryWrapper>();
        services.AddScoped(providers =>
        {
            var connection = providers.GetRequiredService<NpgsqlDataSource>()
                                .OpenConnection();
            var compiler = new PostgresCompiler();
            return new QueryFactory(connection: connection, compiler: compiler);
        });
        services.AddScoped<IPostgreSqlQueryBuilder, PostgreSqlQueryBuilder>();
        return services;
    }
}