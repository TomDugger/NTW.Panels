using System.Windows;

namespace NTW.Panels {
    /// <summary>
    /// Simple standart abstract handler
    /// </summary>
    public abstract class CustomObject : Freezable
        , ICustom, INotifyOption {

        /// <summary>
        /// Allow to get the state
        /// </summary>
        public bool IsActive {
            get { return (bool)GetValue(IsActiveProperty); }
            protected set { SetValue(IsActiveProperty, value); }
        }

        public static readonly DependencyProperty IsActiveProperty =
            DependencyProperty.Register("IsActive", typeof(bool), typeof(CustomObject), new PropertyMetadata(true));

        /// <summary>
        /// Allow to set the state
        /// </summary>
        /// <param name="isActive">New state</param>
        public void SetState(bool isActive) {
            this.IsActive = isActive;
        }

        #region INotifyOption
        public event OptionCallingHandler OptionCalling;

        /// <summary>
        /// Needs to use when the system needs to call Measure, Arrange and other in the Parent CustomPane
        /// </summary>
        /// <param name="option">Update option</param>
        protected void SetUpdateOption(CustomObject sender, UpdateOptions option) => OptionCalling?.Invoke(sender ?? this, option);
        #endregion

        protected override void OnPropertyChanged(DependencyPropertyChangedEventArgs e) {
            base.OnPropertyChanged(e);

            // if propertyMetadata of this property is OptionPropertyMetadata, the system read and send update option from it
            if (e.Property.GetMetadata(this.GetType()) is OptionPropertyMetadata optionPropertyMetadata)
                this.SetUpdateOption(this, optionPropertyMetadata.Option);
        }
    }
}
