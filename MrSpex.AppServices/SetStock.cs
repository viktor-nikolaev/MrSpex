using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using FluentValidation;
using MediatR;
using MrSpex.Domain;
using MrSpex.SharedKernel;

namespace MrSpex.AppServices
{
    /// <summary>
    ///     Writing all the needed "feature" code in one place so it's easier to maintain it
    ///     Following J. Boggard pattern for writing commands
    ///     eg https://github.com/jbogard/ContosoUniversityDotNetCore/blob/master/ContosoUniversity/Features/Courses/Create.cs
    /// </summary>
    public static class SetStock
    {
        public record Command(IReadOnlyList<StockDto> Stocks) : IRequest<Response>;

        public record Response(bool Acknowledged);

        public record StockDto
        (
            string Location,
            string SKU,
            int Quantity
        );

        private class Validator : AbstractValidator<Command>
        {
            public Validator(StockDtoValidator stockDtoValidator)
            {
                RuleFor(x => x.Stocks).NotEmpty();
                RuleForEach(x => x.Stocks).SetValidator(stockDtoValidator);
            }
        }

        private class StockDtoValidator : AbstractValidator<StockDto>
        {
            public StockDtoValidator()
            {
                RuleFor(x => x.Location).NotEmpty();
                RuleFor(x => x.SKU).NotEmpty();
                RuleFor(x => x.Quantity).GreaterThanOrEqualTo(0);
            }
        }

        private class Handler : IRequestHandler<Command, Response>
        {
            private readonly IStockRepository _stockRepository;
            private readonly IUnitOfWork _uow;

            public Handler(IStockRepository stockRepository, IUnitOfWork uow)
            {
                _stockRepository = stockRepository;
                _uow = uow;
            }

            public async Task<Response> Handle(Command command, CancellationToken cancel)
            {
                // request.Stocks may have items for the same sku and location, so the last item wins here
                var stocksToCreateOrUpdate = command
                    .Stocks
                    .GroupBy(x => (x.SKU, x.Location))
                    .ToDictionary(x => x.Key, x => x.Last());

                await UpdateExistingStocks(stocksToCreateOrUpdate, cancel);
                CreateNewStocks(stocksToCreateOrUpdate);
                
                await _uow.Commit(cancel);

                return new Response(true);
            }

            private async Task UpdateExistingStocks
            (
                Dictionary<(string SKU, string Location), StockDto> stocksToCreateOrUpdate,
                CancellationToken cancel
            )
            {
                // find existing stocks for updating
                var specs = stocksToCreateOrUpdate.Keys.Select(x => StockSpecs.WithSKUInLocation(x.SKU, x.Location));
                var existingStocks = await _stockRepository.GetStocksSatisfiedByAnySpec(specs, cancel);

                foreach (var stock in existingStocks)
                {
                    var key = (stock.Location, stock.SKU);

                    var (_, _, newQuantity) = stocksToCreateOrUpdate[key];
                    stock.ChangeQuantity(newQuantity);

                    // trying not to rely on the EF change tracker
                    _stockRepository.Update(stock);

                    // remove it from the container so this item won't be added to the database on the next step
                    // yes, this makes method non-immutable, but it's easier to implement
                    stocksToCreateOrUpdate.Remove(key);
                }
            }

            private void CreateNewStocks(Dictionary<(string SKU, string Location), StockDto> stocksToCreateOrUpdate)
            {
                var stocks = stocksToCreateOrUpdate
                    .Values
                    .Select(x => new Stock(x.SKU, x.Location, x.Quantity));

                _stockRepository.AddRange(stocks);
            }
        }
    }
}