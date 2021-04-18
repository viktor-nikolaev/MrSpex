using System;
using System.Collections.Generic;
using System.Linq;
using NSpecifications;

namespace MrSpex.Domain
{
    public class SalesChannel
    {
        public SalesChannel(string salesChannelId, IEnumerable<string> locations)
        {
            SalesChannelId = salesChannelId;
            Locations = locations.ToList();
        }

        [Obsolete("For ORM only", true)]
        private SalesChannel()
        {
            // for ORM
            SalesChannelId = null!;
            Locations = null!;
        }

        public int Id { get; private set; }

        /// <summary>
        ///     SC1, SC2, SC3 … SCn - with non-relational db no duplicates on this field allowed
        /// </summary>
        public string SalesChannelId { get; private set; }

        public List<string> Locations { get; private set; }

        public void ChangeLocations(IEnumerable<string> locations)
        {
            Locations = locations.ToList();
        }
    }

    public static class SalesChannelSpecs
    {
        public static ASpec<SalesChannel> WithSalesChannelId(string salesChannelId)
        {
            return new Spec<SalesChannel>(x => x.SalesChannelId == salesChannelId);
        }

        public static ASpec<SalesChannel> InLocations(IEnumerable<string> locations)
        {
            var efTranslationFix = locations.ToArray();
            return new Spec<SalesChannel>(x => x.Locations.Any(y => efTranslationFix.Contains(y)));
        }
    }
}