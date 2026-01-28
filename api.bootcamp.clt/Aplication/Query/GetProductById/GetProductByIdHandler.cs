using api.bootcamp.clt.Api.Response;
using api.bootcamp.clt.Domain.Entity;
using api.bootcamp.clt.Infraestructure.Context;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace api.bootcamp.clt.Aplication.Query.GetProductById
{
    public class GetProductByIdHandler : IRequestHandler<GetProductByIdQuery, ProductResponse?>
    {
        private readonly PostgresDbContext _postgresdbContext;

        public GetProductByIdHandler(PostgresDbContext dbContext)
        {
            _postgresdbContext = dbContext;
        }

        public async Task<ProductResponse?> Handle(GetProductByIdQuery request, CancellationToken cancellationToken)
        {
            var productEntity = await _postgresdbContext.Products.AsNoTracking().FirstOrDefaultAsync(p => p.Id == request.Id);

            if (productEntity == null)
            {
                throw new KeyNotFoundException($"Producto {request.Id} no encontrado");
            }

            return new ProductResponse(
                productEntity.Id,
                productEntity.Codigo,
                productEntity.Nombre,
                productEntity.Descripcion ?? string.Empty,
                productEntity.Precio,
                productEntity.Activo,
                productEntity.CategoriaId,
                productEntity.FechaCreacion,
                productEntity.FechaActualizacion,
                productEntity.CantidadStock
            );

        }
    }
}


