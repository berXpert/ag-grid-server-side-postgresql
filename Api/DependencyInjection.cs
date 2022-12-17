using System.Data.Common;
using Npgsql;
using SqlKata.Compilers;
using SqlKata.Execution;

namespace Api;

public static class DependencyInjection
{
    public static IServiceCollection AddPresentation(
        this IServiceCollection services)
    {
        services.AddControllers();
        services.AddScoped<QueryFactory>( providers =>
        {
            var connection = providers.GetRequiredService<NpgsqlDataSource>()
                                .OpenConnection();
            var compiler = new PostgresCompiler();
            return new QueryFactory(connection: connection, compiler: compiler);
        }
        );
        return services;
    }
}