using api.bootcamp.clt.Api.Response;
using api.bootcamp.clt.Infraestructure.Context;
using MediatR;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using System.Globalization;

namespace api.bootcamp.clt.Aplication.Query.GetProducts
{
    public class GetProductsHandler : IRequestHandler<GetProductsQuery, List<ProductResponse>>
    {
        private readonly PostgresDbContext _postgresDbContext;
        private readonly IConfiguration _configuration;
        public GetProductsHandler(PostgresDbContext postgresDbContext, IConfiguration configuration)
        {
            _postgresDbContext = postgresDbContext;
            _configuration = configuration;
        }

        public async Task<List<ProductResponse>> Handle(GetProductsQuery request, CancellationToken cancellationToken)
        {
            var productsEntity = await _postgresDbContext.Products.AsNoTracking().ToListAsync(cancellationToken);

            if (productsEntity.Count == 0)
                return new List<ProductResponse>();

            var percentDiscount = _configuration.GetValue<int>("percentDiscount");
            var categoriesDiscountRaw = _configuration["categoriesDiscount"] ?? string.Empty;

            var discountedCategoryIds = categoriesDiscountRaw
                .Split(',', StringSplitOptions.RemoveEmptyEntries | StringSplitOptions.TrimEntries)
                .Select(s => int.TryParse(s, NumberStyles.Integer, CultureInfo.InvariantCulture, out var id) ? (int?)id : null)
                .Where(id => id.HasValue)
                .Select(id => id!.Value)
                .ToHashSet();

            var applyDiscount = percentDiscount > 0 && discountedCategoryIds.Count > 0;

            decimal ApplyDiscountIfEligible(decimal price, int categoriaId)
            {
                if (!applyDiscount) return price;
                if (!discountedCategoryIds.Contains(categoriaId)) return price;

                var factor = (100 - percentDiscount) / 100m;

                return Math.Round(price * factor, 0);
            }

            var productResponses = productsEntity.Select(p => new ProductResponse(
                p.Id,
                p.Codigo,
                p.Nombre,
                p.Descripcion ?? string.Empty,
                ApplyDiscountIfEligible(p.Precio, p.CategoriaId),
                p.Activo,
                p.CategoriaId,
                p.FechaCreacion,
                p.FechaActualizacion,
                p.CantidadStock
            )).ToList();

            return productResponses;
        }
    }
}
