using System;
using System.Collections.Generic;
using System.Diagnostics.Contracts;

namespace FootballSimulationApp
{
    internal static class EnumerableExtensions
    {
        public static void ForEach<T>(this IEnumerable<T> source, Action<T> action)
        {
            Contract.Requires<ArgumentNullException>(source != null);
            Contract.Requires<ArgumentNullException>(action != null);

            foreach (var item in source)
                action(item);
        }
    }
}