using MrSpex.Domain;

namespace MrSpex.Data
{
    internal class SalesChannelRepository : EfRepository<SalesChannel>, ISalesChannelRepository
    {
        public SalesChannelRepository(AvailabilityDbContext db) : base(db)
        {
        }
    }
}