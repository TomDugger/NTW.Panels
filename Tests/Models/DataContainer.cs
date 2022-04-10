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

            this.hierarchicalItemList = new ObservableCollection<DataItem>(
                Enumerable.Range(0, 5).Select(i => 
                new HierarchicalDataItem($"Item {i}", new DataItem($"Item {i * 10}"), new DataItem($"Item {i * 10 + 1}"), new DataItem($"Item {i * 10 + 2}"))));

            hierarchicalItemsViewSource = new CollectionViewSource { Source = HierarchicalItemList };

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

            HierarchicalMoveUpItems = (cParent, current, tParent, to) => {

                if (cParent is HierarchicalDataItem cp) {
                    cp.Items.Remove((DataItem)current);
                    cp.ItemsView.Refresh();
                } else
                    this.HierarchicalItemList.Remove((DataItem)current);

                int tIndex = ((tParent as HierarchicalDataItem)?.Items ?? this.HierarchicalItemList).IndexOf((DataItem)to) + 1;

                if (tParent is HierarchicalDataItem tp) {
                    tp.Items.Insert(tIndex, (DataItem)current);
                    tp.ItemsView.Refresh();
                } else
                    this.HierarchicalItemList.Insert(tIndex, (DataItem)current);
            };
        }

        #region Data items
        #region Single
        private ObservableCollection<DataItem> itemsList;
        public ObservableCollection<DataItem> ItemsList => itemsList;

        private CollectionViewSource itemsViewSource;
        public ICollectionView ItemsView => itemsViewSource.View;
        #endregion


        #region Hierarchical
        private ObservableCollection<DataItem> hierarchicalItemList;
        public ObservableCollection<DataItem> HierarchicalItemList => hierarchicalItemList;

        private CollectionViewSource hierarchicalItemsViewSource;
        public ICollectionView HierarchicalItemsView => hierarchicalItemsViewSource.View; 
        #endregion

        #region Activities
        public Action<object, object> MoveUpItems { get; }

        public Action<IEnumerable<object>> SelectingItems { get; }

        public Action<object, object, object, object> HierarchicalMoveUpItems { get; }
        #endregion
        #endregion

        private IEnumerable<Point> pointsList = Enumerable.Range(0, 100).Select(x => new Point(x % 10 * 0.1, (int)(x / 10) * 0.1));
        public IEnumerable<Point> PointsList => pointsList;

        private IEnumerable<ValueElement> diagramList = Enumerable.Range(0, 10).Select(x => new ValueElement(x * 10)).ToArray();
        public IEnumerable<ValueElement> DiagramList => diagramList;

        private IEnumerable<ValueElement> hierarchicalList = Enumerable.Range(0, 10).Select(x => new ValueElement(0.2 + (double)x / 90)).ToArray();
        public IEnumerable<ValueElement> HierarchicalList => hierarchicalList;
    }
}
