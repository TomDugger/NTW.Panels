using System.Collections.Generic;
using System.Windows;

namespace NTW.Panels {
    /// <summary>
    /// Simple abstract custom collection with notify update options
    /// </summary>
    /// <typeparam name="T"></typeparam>
    public abstract class CustomCollection<T>: 
        FreezableCollection<T>, INotifyOption
        where T: DependencyObject{

        public CustomCollection() : base(new List<T>()) { }

        #region INotifyOption
        public event OptionCallingHandler OptionCalling;

        protected void SetUpdateOption(CustomObject sender, UpdateOptions option) => OptionCalling?.Invoke(sender, option);
        #endregion
    }
}
