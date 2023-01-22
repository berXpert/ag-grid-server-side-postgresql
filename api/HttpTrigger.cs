using Contracts;
using Microsoft.Azure.Functions.Worker;
using Microsoft.Azure.Functions.Worker.Http;
using Microsoft.Extensions.Logging;
using QueryBuilder;

namespace BerXpert.Functions
{
    public class HttpTrigger
    {
        private readonly ILogger _logger;
        private readonly IPostgreSqlQueryBuilder _queryBuilder;

        public HttpTrigger(ILoggerFactory loggerFactory, IPostgreSqlQueryBuilder queryBuilder)
        {
            _logger = loggerFactory.CreateLogger<HttpTrigger>();
            _queryBuilder = queryBuilder;
        }

        [Function("winners")]
        public async Task<GridRowsResponse> RunAsync(
            [HttpTrigger(AuthorizationLevel.Anonymous, "post")]
            HttpRequestData req)
        {
            _logger.LogInformation("C# HTTP trigger function processed a request.");

            var request = await req.ReadFromJsonAsync<GridRowsRequest>();

            if (request is not null)
            {
                return _queryBuilder.Build(request, "olympic_winners");
            }
            else
            {
                return new GridRowsResponse();
            }
        }
    }
}
