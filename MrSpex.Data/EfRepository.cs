using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using MrSpex.Domain;
using NSpecifications;

namespace MrSpex.Data
{
    internal class EfRepository<T> : IRepository<T> where T : class
    {
        protected readonly DbContext Context;

        public EfRepository(DbContext context)
        {
            Context = context;
        }

        public async Task<T?> Get(ASpec<T> specification, CancellationToken cancel = default)
        {
            return await Context.Set<T>().FirstOrDefaultAsync(specification, cancel);
        }

        public async Task<IReadOnlyList<T>> FindAll(ASpec<T> specification, CancellationToken cancel = default)
        {
            return await Context.Set<T>().Where(specification).ToListAsync(cancel);
        }

        public async Task<bool> Contains(ASpec<T> specification, CancellationToken cancel = default)
        {
            return await Context.Set<T>().AnyAsync(specification, cancel);
        }

        public async Task<int> Count(ASpec<T> specification, CancellationToken cancel = default)
        {
            return await Context.Set<T>().CountAsync(specification, cancel);
        }

        public void Add(T entity)
        {
            Context.Add(entity);
        }

        public void AddRange(IEnumerable<T> entities)
        {
            Context.AddRange(entities);
        }

        public void Update(T entity)
        {
            Context.Update(entity);
        }

        public void UpdateRange(IEnumerable<T> entities)
        {
            Context.UpdateRange(entities);
        }
    }
}