using System.Threading;
using System.Threading.Tasks;
using MrSpex.SharedKernel;

namespace MrSpex.Data
{
    public class EfUnitOfWork : IUnitOfWork
    {
        private readonly AvailabilityDbContext _dbContext;

        public EfUnitOfWork(AvailabilityDbContext dbContext)
        {
            _dbContext = dbContext;
        }

        public Task Commit(CancellationToken cancel)
        {
            return _dbContext.SaveChangesAsync(cancel);
        }
    }
}