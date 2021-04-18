using MrSpex.Domain;

namespace MrSpex.Data
{
    internal class StockRepository : EfRepository<Stock>, IStockRepository
    {
        public StockRepository(AvailabilityDbContext context) : base(context)
        {
        }
    }
}