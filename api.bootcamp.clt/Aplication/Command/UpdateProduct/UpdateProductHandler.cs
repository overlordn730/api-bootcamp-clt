using api.bootcamp.clt.Api.Response;
using api.bootcamp.clt.Domain.Entity;
using api.bootcamp.clt.Infraestructure.Context;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace api.bootcamp.clt.Aplication.Command.UpdateProduct
{
    public class UpdateProductHandler : IRequestHandler<UpdateProductCommand, ProductResponse>
    {
        private readonly PostgresDbContext _postgresDbContext;

        public UpdateProductHandler(PostgresDbContext postgresDbContext)
        {
            _postgresDbContext = postgresDbContext;
        }

        public async Task<ProductResponse> Handle(UpdateProductCommand request, CancellationToken cancellationToken)
        {
            var productEntity = await _postgresDbContext.Products.FirstOrDefaultAsync(p => p.Id == request.Id, cancellationToken);

            if (productEntity == null)
            {
                throw new KeyNotFoundException("Producto no encontrado.");
            }

            productEntity.Codigo = request.ProductRequest.Codigo;
            productEntity.Nombre = request.ProductRequest.Nombre;
            productEntity.Descripcion = request.ProductRequest.Descripcion;
            productEntity.Precio = request.ProductRequest.Precio;
            productEntity.Activo = request.ProductRequest.Activo;
            productEntity.CategoriaId = request.ProductRequest.CategoriaId;
            productEntity.CantidadStock = request.ProductRequest.CantidadStock;
            productEntity.FechaActualizacion = DateTime.UtcNow;

            await _postgresDbContext.SaveChangesAsync(cancellationToken);

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
