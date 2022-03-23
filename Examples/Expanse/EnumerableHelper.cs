using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Examples.Expanse {
    public static class EnumerableHelper {

        public static IEnumerable<Pair<T>> WithPrevious<T>(this IEnumerable<T> source) {
            T previous = default;

            foreach (var item in source) {
                if (!previous.Equals(default(T)))
                    yield return new Pair<T>(item, previous);
                previous = item;
            }
        }
    }
}
