using MediatR;
using api.bootcamp.clt.Infraestructure.Context;

namespace api.bootcamp.clt.Aplication.Command.DeleteProduct
{
    public class DeleteProductHandler : IRequestHandler<DeleteProductCommand, Unit>
    {
        private readonly PostgresDbContext _context;

        public DeleteProductHandler(PostgresDbContext context)
        {
            _context = context;
        }

        public async Task<Unit> Handle(DeleteProductCommand request, CancellationToken cancellationToken)
        {
            var product = await _context.Products
                .FindAsync(new object[] { request.Id }, cancellationToken);

            if (product is null)
                throw new KeyNotFoundException($"Producto con ID {request.Id} no encontrado.");

            _context.Products.Remove(product);
            await _context.SaveChangesAsync(cancellationToken);

            return Unit.Value;
        }
    }
}