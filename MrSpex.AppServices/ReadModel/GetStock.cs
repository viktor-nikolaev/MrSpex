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
    public static class GetStock
    {
        public record Query(IReadOnlyList<string> SKUs) : IRequest<Response>;
        public record Response(IReadOnlyList<StockDto> Stocks);

        public class Validator : AbstractValidator<Query>
        {
            public Validator()
            {
                RuleFor(x => x.SKUs).NotEmpty();
            }
        }

        private class Handler : IRequestHandler<Query, Response>
        {
            private readonly IReadRepository _repository;

            public Handler(IReadRepository repository)
            {
                _repository = repository;
            }

            public async Task<Response> Handle(Query request, CancellationToken cancel)
            {
                var items = await _repository
                    .Set<Stock>()
                    .Where(x => request.SKUs.Contains(x.SKU))
                    .Select(x => new StockDto(x.Location, x.SKU, x.Quantity))
                    .ToListAsync(cancel);

                return new Response(items);
            }
        }
    }
}