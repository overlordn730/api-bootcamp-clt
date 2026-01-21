using api.bootcamp.clt.Infraestructure.Context;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace api.bootcamp.clt.Aplication.Command.DeleteProduct
{
    public class DeleteProductHandler : IRequestHandler<DeleteProductCommand, bool>
    {
        private readonly PostgresDbContext _postgresDbContext;

        public DeleteProductHandler(PostgresDbContext postgresDbContext)
        {
            _postgresDbContext = postgresDbContext;
        }

        public async Task<bool> Handle(DeleteProductCommand request, CancellationToken cancellationToken)
        {
            var productEntity = await _postgresDbContext.Products
                                                 .FirstOrDefaultAsync(p => p.Id == request.Id, cancellationToken);

            if (productEntity == null)
            {
                return false;
            }

            _postgresDbContext.Products.Remove(productEntity);

            await _postgresDbContext.SaveChangesAsync(cancellationToken);

            return true;
        }
    }
}
