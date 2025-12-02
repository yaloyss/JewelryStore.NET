using MediatR;
using Microsoft.Extensions.Logging;
using Reviews.Domain.Exceptions;

namespace Reviews.Application.Behaviors
{
    public class ExceptionHandlingBehavior<TRequest, TResponse> : IPipelineBehavior<TRequest, TResponse>
       where TRequest : IRequest<TResponse>
    {
        private readonly ILogger<ExceptionHandlingBehavior<TRequest, TResponse>> _logger;

        public ExceptionHandlingBehavior(ILogger<ExceptionHandlingBehavior<TRequest, TResponse>> logger)
        {
            _logger = logger;
        }

        public async Task<TResponse> Handle(TRequest request, RequestHandlerDelegate<TResponse> next, CancellationToken cancellationToken)
        {
            try
            {
                return await next();
            }
            catch (ValidationException ex)
            {
                var requestName = typeof(TRequest).Name;
                _logger.LogWarning("Validation error for {RequestName}: {ValidationErrors}", requestName, ex.Message);
                throw;
            }
            catch (DomainException ex)
            {
                var requestName = typeof(TRequest).Name;
                _logger.LogWarning("Domain error while executing {RequestName}: {Message}", requestName, ex.Message);
                throw;
            }
            catch (Exception ex)
            {
                var requestName = typeof(TRequest).Name;
                _logger.LogError(ex, "Unexpected error while executing {RequestName}: {Message}", requestName, ex.Message);
                throw;
            }
        }
    }
    }

