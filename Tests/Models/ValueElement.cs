using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Tests.Models {
    public class ValueElement: Observable {

        public ValueElement(double value) {
            this._value = value;
        }

        private double _value;
        public double Value {
            get { return _value; }
            set { _value = value; this.Send(nameof(Value)); }
        }
    }
}
