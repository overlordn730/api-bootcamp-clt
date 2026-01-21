using api.bootcamp.clt.Api.Response;
using MediatR;

namespace api.bootcamp.clt.Aplication.Query.GetProducts
{
    public record GetProductsQuery() : IRequest<List<ProductResponse>>;
}
