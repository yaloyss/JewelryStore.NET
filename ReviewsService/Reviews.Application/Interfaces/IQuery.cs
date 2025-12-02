using MediatR;

namespace Reviews.Application.Interfaces
{
    public interface IQuery<out TResponse> : IRequest<TResponse> { }
}

