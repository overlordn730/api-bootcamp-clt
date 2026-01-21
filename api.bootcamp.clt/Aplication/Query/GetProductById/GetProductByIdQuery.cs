using api.bootcamp.clt.Api.Response;
using MediatR;

namespace api.bootcamp.clt.Aplication.Query.GetProductById
{
    public record GetProductByIdQuery(int Id) : IRequest<ProductResponse?>;
 
}