namespace api.bootcamp.clt.Aplication.Commands
{
    using api.bootcamp.clt.Api.Response;
    using api.bootcamp.clt.Aplication.Command.UpdateProductStatus;
    using api.bootcamp.clt.Domain.Entity;
    using api.bootcamp.clt.Infraestructure.Context;
    using MediatR;
    using Microsoft.EntityFrameworkCore;

    public class UpdateProductStatusHandler : IRequestHandler<UpdateProductStatusCommand, ProductResponse>
    {
        private readonly PostgresDbContext _postgresDbContext;

        public UpdateProductStatusHandler(PostgresDbContext postgresDbContext)
        {
            _postgresDbContext = postgresDbContext;
        }

        public async Task<ProductResponse> Handle(UpdateProductStatusCommand request, CancellationToken cancellationToken)
        {
            var productEntity = await _postgresDbContext.Products
                                                 .FirstOrDefaultAsync(p => p.Id == request.Id, cancellationToken);

            if (productEntity == null)
            {
                throw new KeyNotFoundException("Producto no encontrado.");
            }

            if (request.Activo.HasValue)
            {
                productEntity.Activo = request.Activo.Value;
                productEntity.FechaActualizacion = DateTime.UtcNow;
            }

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
