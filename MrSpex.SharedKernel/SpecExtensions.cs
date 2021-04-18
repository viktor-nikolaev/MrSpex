using System.Collections.Generic;
using System.Linq;
using NSpecifications;

namespace MrSpex.SharedKernel
{
    public static class SpecExtensions
    {
        public static ASpec<T> AtLeastOne<T>(this IEnumerable<ASpec<T>> specs)
        {
            return specs.Aggregate(Spec<T>.None, (agg, x) => agg | x);
        }
    }
}