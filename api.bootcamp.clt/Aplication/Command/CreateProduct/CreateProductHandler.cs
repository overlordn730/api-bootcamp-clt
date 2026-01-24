using api.bootcamp.clt.Api.Response;
using api.bootcamp.clt.Domain.Entity;
using api.bootcamp.clt.Infraestructure.Context;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace api.bootcamp.clt.Aplication.Command.CreateProduct
{
    public class CreateProductHandler : IRequestHandler<CreateProductCommand, ProductResponse>
    {
        private readonly PostgresDbContext _postgresDbContext;
        public CreateProductHandler(PostgresDbContext postgresDbContext)
        {
            _postgresDbContext = postgresDbContext;
        }

        public async Task<ProductResponse> Handle(CreateProductCommand request, CancellationToken cancellationToken)
        {
            var productEntity = new Product
            {
                Codigo = request.ProductRequest.Codigo,
                Nombre = request.ProductRequest.Nombre,
                Descripcion = request.ProductRequest.Descripcion,
                Precio = request.ProductRequest.Precio,
                Activo = request.ProductRequest.Activo,
                CategoriaId = request.ProductRequest.CategoriaId,
                FechaCreacion = DateTime.UtcNow,
                CantidadStock = 0 
            };

            _postgresDbContext.Products.Add(productEntity);

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
