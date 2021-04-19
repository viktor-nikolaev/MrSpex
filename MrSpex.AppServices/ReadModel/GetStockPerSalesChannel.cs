using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using FluentValidation;
using MediatR;
using Microsoft.EntityFrameworkCore;
using MrSpex.Domain;

namespace MrSpex.AppServices.ReadModel
{
    public static class GetStockPerSalesChannel
    {
        public record Query
        (
            string SalesChannelId,
            IReadOnlyList<string> SKUs
        ) : IRequest<Response>;

        public record Response(IReadOnlyList<StockDto> Stock);

        public record StockDto(string SKU, IReadOnlyList<SKUInLocation> SKUInLocations);

        public record SKUInLocation(string Location, int Quantity);

        public class Validator : AbstractValidator<Query>
        {
            public Validator()
            {
                RuleFor(x => x.SalesChannelId).NotEmpty();
                RuleFor(x => x.SKUs).NotEmpty();
                RuleForEach(x => x.SKUs).NotEmpty();
            }
        }

        private class Handler : IRequestHandler<Query, Response>
        {
            private readonly IReadRepository _repository;

            public Handler(IReadRepository repository)
            {
                _repository = repository;
            }

            public async Task<Response> Handle(Query query, CancellationToken cancel)
            {
                var (salesChannelId, skus) = query;

                var q =
                    from salesChannel in _repository.Set<SalesChannel>()
                    where salesChannel.SalesChannelId == salesChannelId
                    from stock in _repository.Set<Stock>()
                    where salesChannel.Locations.Contains(stock.Location)
                          && stock.Quantity > 0 && skus.Contains(stock.SKU)
                    select stock;

                var list = await q.ToArrayAsync(cancel);

                // group by is not EF friendly, so grouping in memory
                var dto = list
                    .GroupBy(x => x.SKU, x => new SKUInLocation(x.Location, x.Quantity))
                    .Select(x => new StockDto(x.Key, x.ToArray()))
                    .ToList();

                return new Response(dto);
            }
        }
    }
}