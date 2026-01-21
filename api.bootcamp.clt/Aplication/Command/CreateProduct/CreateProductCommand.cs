using api.bootcamp.clt.Api.Request;
using api.bootcamp.clt.Api.Response;
using MediatR;

namespace api.bootcamp.clt.Aplication.Command.CreateProduct
{
    public record CreateProductCommand(CreateProductRequest ProductRequest) : IRequest<ProductResponse>;
}
