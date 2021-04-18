using System;

namespace MrSpex.Domain
{
    public class Lock
    {
        public Lock(string sku, string location, int amount, string reason, string transactionId)
        {
            SKU = sku;
            Location = location;
            Amount = amount;
            Reason = reason;
            TransactionId = transactionId;

            Id = Guid.NewGuid().ToString("D");
        }

        public string Id { get; set; }

        /// <summary>
        ///     Stock-keeping unit - SKU1, SKU2, SKU3 …SKUn
        /// </summary>
        public string SKU { get; set; }

        /// <summary>
        ///     LOC1, LOC2, LOC3 … LOCn
        /// </summary>
        public string Location { get; set; }

        /// <summary>
        ///     It’s the amount reserved or ordered
        /// </summary>
        public int Amount { get; set; }

        /// <summary>
        ///     In shopping basket, Ordered
        /// </summary>
        public string Reason { get; set; }

        public string TransactionId { get; set; }
    }
}