using System.Collections.Generic;
using System.Collections.Specialized;
using System.Linq;
using System.Windows;

namespace NTW.Panels {

    /// <summary>
    /// Handlers collection (is freezable Collection)
    /// </summary>
    /// <typeparam name="T"></typeparam>
    public class HandlersCollection<T> : FreezableCollection<T>
        where T : Freezable, ICustomHandler {

        private CustomPanel owner;

        internal HandlersCollection() : base(new List<T>()) {
            ((INotifyCollectionChanged)this).CollectionChanged += MouseHandlerCollection_CollectionChanged;
        }

        private void MouseHandlerCollection_CollectionChanged(object sender, NotifyCollectionChangedEventArgs e) {
            switch (e.Action) {
                case NotifyCollectionChangedAction.Add:
                    foreach (ICallingModifer calling in e.NewItems?.Cast<T>().Where(x => x is ICallingModifer))
                        calling.PanelCalling += owner.PanelCalling;

                    break;
                case NotifyCollectionChangedAction.Remove:
                    foreach (ICallingModifer calling in e.OldItems?.Cast<T>().Where(x => x is ICallingModifer))
                        calling.PanelCalling -= owner.PanelCalling;

                    break;
                case NotifyCollectionChangedAction.Reset:
                    foreach (ICallingModifer calling in this?.Cast<T>().Where(x => x is ICallingModifer))
                        calling.PanelCalling -= owner.PanelCalling;

                    break;
                case NotifyCollectionChangedAction.Replace:
                    foreach (ICallingModifer calling in e.OldItems?.Cast<T>().Where(x => x is ICallingModifer))
                        calling.PanelCalling -= owner.PanelCalling;

                    foreach (ICallingModifer calling in e.NewItems?.Cast<T>().Where(x => x is ICallingModifer))
                        calling.PanelCalling += owner.PanelCalling;

                    break;
            }
        }

        /// <summary>
        /// Set owner for collection
        /// </summary>
        /// <param name="owner">NEw owner</param>
        /// <returns></returns>
        internal HandlersCollection<T> SetOwner(CustomPanel owner) {

            var items = this?.Cast<T>().Where(x => x is ICallingModifer);

            foreach (ICallingModifer calling in items)
                calling.PanelCalling -= owner.PanelCalling;

            this.owner = owner;

            foreach (ICallingModifer calling in items)
                calling.PanelCalling += owner.PanelCalling;

            return this;
        }

        /// <summary>
        /// Clear owner
        /// </summary>
        /// <returns></returns>
        internal HandlersCollection<T> ClearOwner() {

            var items = this?.Cast<T>().Where(x => x is ICallingModifer);

            foreach (ICallingModifer calling in items)
                calling.PanelCalling -= owner.PanelCalling;

            this.owner = null;

            return this; 
        }
    }
}
