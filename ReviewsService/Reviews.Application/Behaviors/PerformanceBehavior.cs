using MediatR;
using Microsoft.Extensions.Logging;
using System.Diagnostics;

namespace Reviews.Application.Behaviors
{
	public class PerformanceBehavior<TRequest, TResponse> : IPipelineBehavior<TRequest, TResponse> where TRequest : IRequest<TResponse>
    {
        private readonly ILogger<PerformanceBehavior<TRequest, TResponse>> _logger;
        private readonly Stopwatch _timer;
        private const int SlowRequestThreshold = 500;

        public PerformanceBehavior(ILogger<PerformanceBehavior<TRequest, TResponse>> logger)
        {
            _logger = logger;
            _timer = new Stopwatch();
        }

        public async Task<TResponse> Handle(TRequest request, RequestHandlerDelegate<TResponse> next, CancellationToken cancellationToken)
        {
            _timer.Start();
            var response = await next();
            _timer.Stop();
            var elapsedMilliseconds = _timer.ElapsedMilliseconds;

            if (elapsedMilliseconds > SlowRequestThreshold)
            {
                var requestName = typeof(TRequest).Name;
                _logger.LogWarning(
                    "Slow query detected: {RequestName} ({ElapsedMilliseconds}ms) exceeded {Threshold}ms. " + "Query: {@Request}",
                    requestName, elapsedMilliseconds, SlowRequestThreshold, request);
            }
            else
            {
                var requestName = typeof(TRequest).Name;
                _logger.LogDebug("Query {RequestName} executed in {ElapsedMilliseconds}ms", requestName, elapsedMilliseconds);
            }

            return response;
        }
    }
}

