using System.Windows;

namespace NTW.Panels {
    /// <summary>
    /// Special dependency property metadata. Allow to set Update option for automaticly update parent without calling SetUpdateOption from owner object.
    /// </summary>
    public class OptionPropertyMetadata: PropertyMetadata {

        public OptionPropertyMetadata(): base() { }

        public OptionPropertyMetadata(object defaultValue) 
            : base(defaultValue) { }

        public OptionPropertyMetadata(PropertyChangedCallback propertyChangedCallback) : base(propertyChangedCallback) { }

        public OptionPropertyMetadata(object defaultValue, PropertyChangedCallback propertyChangedCallback)
            : base(defaultValue, propertyChangedCallback) { }

        public OptionPropertyMetadata(object defaultValue, PropertyChangedCallback propertyChangedCallback, CoerceValueCallback coerceValueCallback)
            : base(defaultValue, propertyChangedCallback, coerceValueCallback) { }

        public OptionPropertyMetadata(object defaultValue, UpdateOptions option)
            : base(defaultValue) {
            this.Option = option;
        }

        public OptionPropertyMetadata(object defaultValue, UpdateOptions option, PropertyChangedCallback propertyChangedCallback)
            : base(defaultValue, propertyChangedCallback) {
            this.Option = option;
        }

        public OptionPropertyMetadata(object defaultValue, UpdateOptions option, PropertyChangedCallback propertyChangedCallback, CoerceValueCallback coerceValueCallback)
            : base(defaultValue, propertyChangedCallback, coerceValueCallback) {
            this.Option = option;
        }

        /// <summary>
        /// special update option of this dependency property
        /// </summary>
        public UpdateOptions Option {
            get;
            private set;
        } = UpdateOptions.Measure;
    }
}
