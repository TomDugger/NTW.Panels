using System.Windows;

namespace NTW.Panels {
    /// <summary>
    /// Simple standart abstract handler
    /// </summary>
    public abstract class CustomObject : Freezable
        , ICustom {

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
    }
}
