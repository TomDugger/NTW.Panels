using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Examples.Expanse {

    public struct Pair<T> {

        public Pair(T current, T previous) {
            this.Current = current;
            this.Previous = previous;
        }

        public T Current { get; }
        public T Previous { get; }
    }
}
