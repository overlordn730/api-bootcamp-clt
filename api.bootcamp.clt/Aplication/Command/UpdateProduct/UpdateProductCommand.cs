using api.bootcamp.clt.Api.Request;
using api.bootcamp.clt.Api.Response;
using MediatR;

namespace api.bootcamp.clt.Aplication.Command.UpdateProduct
{
    public record UpdateProductCommand(int Id, UpdateProductRequest ProductRequest) : IRequest<ProductResponse>;
}
