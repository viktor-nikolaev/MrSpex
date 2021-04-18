namespace MrSpex.Domain
{
    /// <summary>
    ///     Repositories are part of the domain but their implementation lies in MrSpex.Data assembly
    /// </summary>
    public interface IStockRepository : IRepository<Stock>
    {
    }
}