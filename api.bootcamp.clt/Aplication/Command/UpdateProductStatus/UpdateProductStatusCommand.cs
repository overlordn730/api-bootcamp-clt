using api.bootcamp.clt.Api.Response;
using MediatR;

namespace api.bootcamp.clt.Aplication.Command.UpdateProductStatus
{
    public record UpdateProductStatusCommand(int Id, bool? Activo) : IRequest<ProductResponse>;
}
