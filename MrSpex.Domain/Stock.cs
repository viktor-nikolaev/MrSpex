using NSpecifications;

namespace MrSpex.Domain
{
    /// <summary>
    ///     todo use optimistic versioning
    ///     todo add indices on locationa and sku 
    /// </summary>
    public class Stock
    {
        public Stock(string sku, string location, int quantity)
        {
            SKU = sku;
            Location = location;
            Quantity = quantity;
        }

        public int Id { get; private set; }

        /// <summary>
        ///     Stock-keeping unit - SKU1, SKU2, SKU3 … SKUn
        /// </summary>
        public string SKU { get; private set; }

        /// <summary>
        ///     LOC1, LOC2, LOC3 … LOCn
        /// </summary>
        public string Location { get; private set; }

        public int Quantity { get; private set; }

        public void ChangeQuantity(int quantity)
        {
            // todo validate and throw
            Quantity = quantity;
        }
    }

    public class StockSpecs
    {
        public static ASpec<Stock> WithSKUInLocation(string sku, string location)
        {
            return new Spec<Stock>(x => x.SKU == sku && x.Location == location);
        }
    }
}