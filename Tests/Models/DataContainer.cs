using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Windows;
using System.Windows.Data;
using Tests.Converters;

namespace Tests.Models {
    public class DataContainer : Observable {

        public DataContainer() {
            this.itemsList = new ObservableCollection<DataItem>(Enumerable.Range(0, 30).Select(i => new DataItem($"Item {i}")));

            this.itemsViewSource = new CollectionViewSource { Source = ItemsList };

            MoveUpItems = (current, to) => {

                int cIndex = itemsList.IndexOf((DataItem)current);
                int tIndex = itemsList.IndexOf((DataItem)to);

                if (cIndex >= 0 && cIndex < itemsList.Count
                    && tIndex >= 0 && tIndex < itemsList.Count) {

                    itemsList.RemoveAt(cIndex);
                    itemsList.Insert(tIndex, (DataItem)current);
                }
            };

            SelectingItems = (selected) => {

                foreach (var s in itemsList)
                    s.IsSelected = false;

                foreach (DataItem sel in selected)
                    sel.IsSelected = !sel.IsSelected;
            };
        }

        #region Data items
        private ObservableCollection<DataItem> itemsList;
        public ObservableCollection<DataItem> ItemsList => itemsList;

        private CollectionViewSource itemsViewSource;
        public ICollectionView ItemsView => itemsViewSource.View;


        #region Activities
        public Action<object, object> MoveUpItems { get; }

        public Action<IEnumerable<object>> SelectingItems { get; }
        #endregion 
        #endregion

        private IEnumerable<Point> pointsList = Enumerable.Range(0, 100).Select(x => new Point(x % 10 * 0.1, (int)(x / 10) * 0.1));
        public IEnumerable<Point> PointsList => pointsList;
    }
}
