using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using NSpecifications;

namespace MrSpex.Domain
{
    /// <summary>
    ///     Repositories are part of the domain but their implementation lies in MrSpex.Data assembly
    /// </summary>
    public interface IStockRepository
    {
        void AddRange(IEnumerable<Stock> stocks);
        void Update(Stock stock);
        Task<IReadOnlyList<Stock>> GetStocksSatisfiedByAnySpec(IEnumerable<ASpec<Stock>> specification,
            CancellationToken cancel = default);
    }
}