using MediatR;
namespace Reviews.Application.Interfaces
{
	public interface ICommand : IRequest { }

    public interface ICommand<out TResponse> : IRequest<TResponse> { }
}

