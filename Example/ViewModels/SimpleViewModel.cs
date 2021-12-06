using Example.Locators;
using Example.Model;
using NTW.Panels;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Windows.Controls;
using System.Windows.Data;

namespace Example.ViewModels {
    public sealed class SimpleViewModel: NotifyProperty {

        public SimpleViewModel() {

            Random rnd = new Random();

            values = new ObservableCollection<Item>(Enumerable.Range(0, 1000).Select(x => new Item(x, $"group_{rnd.Next(1, 3)}")));

        }

        private ObservableCollection<Item> values;
        public IEnumerable<Item> Values => values;

        private CollectionViewSource source;
        public ICollectionView View => (source ?? (source = GetSource())).View;

        public Action<object, object> MoveUp
            => (current, to) => {

                int cIndex = values.IndexOf((Item)current);
                int tIndex = values.IndexOf((Item)to);

                if (cIndex >= 0 && cIndex < values.Count
                    && tIndex >= 0 && tIndex < values.Count) {

                    values.RemoveAt(cIndex);
                    values.Insert(tIndex, (Item)current);
                }
            };

        public Action<IEnumerable<object>> Selecting
            => (selected) => {

                foreach (var s in values) {
                    s.IsSelected = false;
                }

                foreach (Item sel in selected) {
                    sel.IsSelected = !sel.IsSelected;
                }
            };

        public Action<IEnumerable<object>> Selected
            => (selected) => {
                foreach (Item sel in selected) {
                    sel.IsSelected = !sel.IsSelected;
                }
            };

        private CollectionViewSource GetSource() {
            CollectionViewSource source = new CollectionViewSource();
            source.Source = values;
            source.GroupDescriptions.Add(new PropertyGroupDescription(nameof(Item.Group)));
            return source;
        }

        private Orientation panelOrientation = Orientation.Vertical;
        public Orientation PanelOrientation {
            get { return panelOrientation; }
            set { panelOrientation = value; this.Send(nameof(PanelOrientation)); }
        }

        private bool stopOnMouseLeave;
        public bool StopOnMouseLeave {
            get { return stopOnMouseLeave; }
            set { stopOnMouseLeave = value; this.Send(nameof(StopOnMouseLeave)); }
        }

        private bool rectContains;
        public bool RectContains {
            get { return rectContains; }
            set { rectContains = value; this.Send(nameof(RectContains)); }
        }

    }
}
