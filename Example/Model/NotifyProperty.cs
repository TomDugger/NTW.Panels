using System.ComponentModel;

namespace Example.Model {
    public abstract class NotifyProperty : INotifyPropertyChanged {
        public event PropertyChangedEventHandler PropertyChanged;

        protected void Send(string propertyName = "")
            => PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
    }
}
