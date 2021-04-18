using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using MrSpex.Domain;
using NSpecifications;

namespace MrSpex.Data
{
    public class StockRepository : IStockRepository
    {
        private readonly AvailabilityDbContext _context;

        public StockRepository(AvailabilityDbContext context)
        {
            _context = context;
        }

        public void AddRange(IEnumerable<Stock> stocks)
        {
            _context.AddRange(stocks);
        }

        public void Update(Stock stock)
        {
            _context.Update(stock);
        }

        public Task<IReadOnlyList<Stock>> GetStocksSatisfiedByAnySpec(IEnumerable<ASpec<Stock>> specification,
            CancellationToken cancel = default)
        {
            return _context.FindAllSatisfiedByAnySpec(specification, cancel);
        }
    }
}