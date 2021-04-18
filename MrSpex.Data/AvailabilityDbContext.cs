using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using MrSpex.Domain;
using NSpecifications;

namespace MrSpex.Data
{
    public class AvailabilityDbContext : DbContext
    {
        public AvailabilityDbContext(DbContextOptions<AvailabilityDbContext> options) : base(options)
        {
        }

        public DbSet<Stock> Stocks => Set<Stock>();

        public async Task<IReadOnlyList<T>> FindAllSatisfiedByAnySpec<T>(IEnumerable<ASpec<T>> specs,
            CancellationToken cancel = default) where T : class
        {
            var spec = specs.Aggregate(Spec<T>.None, (agg, x) => agg | x);
            return await Set<T>().Where(spec).ToListAsync(cancel);
        }

        protected override void OnModelCreating(ModelBuilder mb)
        {
            mb.Entity<Stock>(b =>
            {
                b.ToTable("Stock");
                b.HasIndex(x => new {x.Location, x.SKU});
            });
        }
    }
}