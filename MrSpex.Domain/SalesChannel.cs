using System.Collections.Generic;

namespace MrSpex.Domain
{
    public class SalesChannel
    {
        public SalesChannel(string salesChannelId, List<string> location)
        {
            SalesChannelId = salesChannelId;
            Location = location;
        }

        public int Id { get; set; }

        /// <summary>
        ///     SC1, SC2, SC3 … SCn - with non-relational db no duplicates on this field allowed
        /// </summary>
        public string SalesChannelId { get; set; }

        public List<string> Location { get; set; }
    }
}