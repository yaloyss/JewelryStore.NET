using MediatR;
using Microsoft.Extensions.Logging;
using System.Diagnostics;
using System.Text.Json;

namespace Reviews.Application.Behaviors
{
	public class LoggingBehavior<TRequest, TResponse> : IPipelineBehavior<TRequest, TResponse> where TRequest : IRequest<TResponse>
    {
        private readonly ILogger<LoggingBehavior<TRequest, TResponse>> _logger;

        public LoggingBehavior(ILogger<LoggingBehavior<TRequest, TResponse>> logger)
        {
            _logger = logger;
        }

        public async Task<TResponse> Handle(TRequest request, RequestHandlerDelegate<TResponse> next, CancellationToken cancellationToken)
        {
            var requestName = typeof(TRequest).Name;
            var requestId = Guid.NewGuid().ToString();
            var stopwatch = Stopwatch.StartNew();
            var requestJson = JsonSerializer.Serialize(request);

            _logger.LogInformation("[{RequestId}] Begining of query: {RequestName}. Data: {RequestData}", requestId, requestName, requestJson);

            try
            {
                var response = await next();
                stopwatch.Stop();

                _logger.LogInformation("[{RequestId}] Query {RequestName} completed successfully in {ElapsedMilliseconds}ms.", requestId, requestName, stopwatch.ElapsedMilliseconds);
                return response;
            }
            catch (Exception ex)
            {
                stopwatch.Stop();

                _logger.LogError(ex, "[{RequestId}] Error while executing query {RequestName} after {ElapsedMilliseconds}ms. MongoDB error: {ErrorMessage}",
                    requestId, requestName, stopwatch.ElapsedMilliseconds, ex.Message);
                throw;
            }
        }
    }
}

