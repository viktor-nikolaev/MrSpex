using System.Threading;
using System.Threading.Tasks;

namespace MrSpex.SharedKernel
{
    public interface IUnitOfWork
    {
        Task Commit(CancellationToken cancel = default);
    }
}