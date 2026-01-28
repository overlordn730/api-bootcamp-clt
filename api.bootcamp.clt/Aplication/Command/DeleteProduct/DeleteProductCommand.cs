using MediatR;

namespace api.bootcamp.clt.Aplication.Command.DeleteProduct
{
    public record DeleteProductCommand(int Id) : IRequest<Unit>;
}
