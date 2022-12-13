using Npgsql;
using SqlKata.Compilers;
using SqlKata.Execution;

namespace Api;

public static class DependencyInjection
{
    public static IServiceCollection AddPresentation(
        this IServiceCollection services,
        IConfiguration configuration)
    {
        services.AddControllers();
        services.AddSingleton<QueryFactory>( _ =>
        {
            var connString = configuration["db_connection"];
            var connection = new NpgsqlConnection(connString);
            var compiler = new PostgresCompiler();
            return new QueryFactory(connection: connection, compiler: compiler);
        }
        );
        return services;
    }
}