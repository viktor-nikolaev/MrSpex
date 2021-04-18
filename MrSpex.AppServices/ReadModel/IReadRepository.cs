using System.Linq;

namespace MrSpex.AppServices.ReadModel
{
    /// <summary>
    ///     For query part we don't care that much about "purity", so expose IQueryable directly and have only one generic implementation
    /// </summary>
    public interface IReadRepository
    {
        IQueryable<T> Set<T>() where T : class;
    }
}