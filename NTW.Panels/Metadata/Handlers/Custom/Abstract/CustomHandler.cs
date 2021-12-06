using System.Windows;

namespace NTW.Panels {
    /// <summary>
    /// Simple standart abstract handler
    /// </summary>
    public abstract class CustomHandler : Freezable
        , ICustomHandler {

        /// <summary>
        /// Allow get the state of handler
        /// </summary>
        public bool IsActive {
            get { return (bool)GetValue(IsActiveProperty); }
            protected set { SetValue(IsActiveProperty, value); }
        }

        public static readonly DependencyProperty IsActiveProperty =
            DependencyProperty.Register("IsActive", typeof(bool), typeof(CustomHandler), new PropertyMetadata(true));

        /// <summary>
        /// Allow set the state of handler
        /// </summary>
        /// <param name="isActive">New state</param>
        public void SetState(bool isActive) {
            this.IsActive = isActive;
        }
    }
}
