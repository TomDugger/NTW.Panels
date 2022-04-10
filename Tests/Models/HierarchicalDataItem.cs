using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Windows.Data;

namespace Tests.Models {
    public class HierarchicalDataItem: DataItem {

        public HierarchicalDataItem(string content) : base(content) {
            this.items = new ObservableCollection<DataItem>();
            this.itemsViewSource = new CollectionViewSource { Source = this.items };
        }

        public HierarchicalDataItem(string content, params DataItem[] items) : base(content) {
            this.items = new ObservableCollection<DataItem>(items);
            this.itemsViewSource = new CollectionViewSource { Source = this.items };
        }

        private ObservableCollection<DataItem> items;
        public ObservableCollection<DataItem> Items => items;

        private CollectionViewSource itemsViewSource;
        public ICollectionView ItemsView => itemsViewSource.View;
    }
}
