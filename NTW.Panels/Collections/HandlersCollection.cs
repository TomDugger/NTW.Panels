using System.Collections.Generic;
using System.Collections.Specialized;
using System.Linq;
using System.Windows;

namespace NTW.Panels {

    /// <summary>
    /// Handlers collection (is freezable Collection)
    /// </summary>
    public class HandlersCollection : CustomCollection<CustomHandler> {

        private CustomPanel owner;

        internal HandlersCollection() : base() {
            ((INotifyCollectionChanged)this).CollectionChanged += MouseHandlerCollection_CollectionChanged;
        }

        private void MouseHandlerCollection_CollectionChanged(object sender, NotifyCollectionChangedEventArgs e) {
            switch (e.Action) {
                case NotifyCollectionChangedAction.Add:
                    foreach (CustomHandler item in e.NewItems?.Cast<CustomHandler>()) {
                        if (item is ICallingModifer calling)
                            calling.PanelCalling += owner.PanelCalling;

                        if (item is INotifyOption notify)
                            notify.OptionCalling += owner.UpdateOptionCalling;
                    }

                    break;
                case NotifyCollectionChangedAction.Remove:
                    foreach (CustomHandler item in e.OldItems?.Cast<CustomHandler>()) {
                        if (item is ICallingModifer calling)
                            calling.PanelCalling -= owner.PanelCalling;

                        if (item is INotifyOption notify)
                            notify.OptionCalling -= owner.UpdateOptionCalling;
                    }

                    break;
                case NotifyCollectionChangedAction.Reset:
                    foreach (CustomHandler item in this?.Cast<CustomHandler>()) {
                        if (item is ICallingModifer calling)
                            calling.PanelCalling -= owner.PanelCalling;

                        if (item is INotifyOption notify)
                            notify.OptionCalling -= owner.UpdateOptionCalling;
                    }

                    break;
                case NotifyCollectionChangedAction.Replace:
                    foreach (CustomHandler item in e.OldItems?.Cast<CustomHandler>()) {
                        if (item is ICallingModifer calling)
                            calling.PanelCalling -= owner.PanelCalling;

                        if (item is INotifyOption notify)
                            notify.OptionCalling -= owner.UpdateOptionCalling;
                    }

                    foreach (CustomHandler item in e.NewItems?.Cast<CustomHandler>()) {
                        if (item is ICallingModifer calling)
                            calling.PanelCalling += owner.PanelCalling;

                        if (item is INotifyOption notify)
                            notify.OptionCalling += owner.UpdateOptionCalling;
                    }

                    break;
            }
        }

        /// <summary>
        /// Set owner for collection
        /// </summary>
        /// <param name="owner">NEw owner</param>
        /// <returns></returns>
        internal HandlersCollection SetOwner(CustomPanel owner) {

            var items = this?.Cast<CustomHandler>().Where(x => x is ICallingModifer);

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
        internal HandlersCollection ClearOwner() {

            var items = this?.Cast<CustomHandler>().Where(x => x is ICallingModifer);

            foreach (ICallingModifer calling in items)
                calling.PanelCalling -= owner.PanelCalling;

            this.owner = null;

            return this; 
        }
    }
}
