using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using NSpecifications;

namespace MrSpex.Domain
{
    public interface IRepository<T> where T : class
    {
        Task<T?> Get(ASpec<T> specification, CancellationToken cancel = default);
        Task<IReadOnlyList<T>> FindAll(ASpec<T> specification, CancellationToken cancel = default);
        Task<bool> Contains(ASpec<T> specification, CancellationToken cancel = default);
        Task<int> Count(ASpec<T> specification, CancellationToken cancel = default);
        void Add(T entity);
        void AddRange(IEnumerable<T> entities);
        void Update(T entity);
        void UpdateRange(IEnumerable<T> entities);
    }
}