using System.Linq;
using Microsoft.EntityFrameworkCore;
using MrSpex.AppServices.ReadModel;

namespace MrSpex.Data
{
    internal class EfReadRepository : IReadRepository
    {
        private readonly AvailabilityDbContext _db;

        public EfReadRepository(AvailabilityDbContext db)
        {
            _db = db;
        }

        public IQueryable<T> Set<T>() where T : class
        {
            return _db.Set<T>().AsNoTracking();
        }
    }
}