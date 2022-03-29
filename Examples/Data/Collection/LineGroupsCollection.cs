using Examples.Designers;
using NTW.Panels;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Linq;
using System.Windows;
using System.Windows.Media;

namespace Examples.Data.Collection {
    public sealed class LineGroupsCollection: CustomCollection<LineGroupSetting> {

        public LineGroupsCollection(): base() {

            ((INotifyCollectionChanged)this).CollectionChanged += Collection_CollectionChanged;

            this.Add(new LineGroupSetting { Name = "." });
        }

        private void Collection_CollectionChanged(object sender, NotifyCollectionChangedEventArgs e) {
            switch (e.Action) {
                case NotifyCollectionChangedAction.Add:
                    foreach (var setting in e.NewItems?.Cast<LineGroupSetting>()) {
                        setting.OptionCalling += OptionCalled;
                    }

                    break;
                case NotifyCollectionChangedAction.Remove:
                    foreach (var setting in e.OldItems?.Cast<LineGroupSetting>()) {
                        setting.OptionCalling -= OptionCalled;
                    }
                    break;
            }

            SetUpdateOption(null, UpdateOptions.ParentUpdate);
        }

        private void OptionCalled(CustomObject sender, UpdateOptions option) {
            this.SetUpdateOption(sender, option);
        }

        
        public Drawing GetDrawing(Dictionary<int, string[]> groups, Dictionary<int, Point> points) {
            DrawingGroup result = new DrawingGroup();

            foreach (var setting in this)
                result.Children.Add(setting.GetDrawing(groups.Where(x => x.Value.Contains(setting.Name)).Select(x => points[x.Key])));

            return result;
        }
    }
}
