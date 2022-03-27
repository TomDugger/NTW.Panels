using Examples.Designers;
using NTW.Panels;
using System.Collections.Specialized;
using System.Linq;
using System.Windows;
using System.Windows.Media;

namespace Examples.Data.Collection {
    public sealed class LineGroupsCollection: CustomCollection<LineGroupSetting> {

        #region Memebrs
        private DrawingGroup drawing;
        #endregion

        public LineGroupsCollection(): base() {

            this.drawing = new DrawingGroup();

            ((INotifyCollectionChanged)this).CollectionChanged += Collection_CollectionChanged;

            this.Add(new LineGroupSetting { Name = "." });
        }

        private void Collection_CollectionChanged(object sender, NotifyCollectionChangedEventArgs e) {
            switch (e.Action) {
                case NotifyCollectionChangedAction.Add:
                    foreach (var setting in e.NewItems?.Cast<LineGroupSetting>()) {

                        this.drawing?.Children.Add(setting.GetDrawing());

                        setting.OptionCalling += OptionCalled;
                    }

                    break;
                case NotifyCollectionChangedAction.Remove:
                    foreach (var setting in e.OldItems?.Cast<LineGroupSetting>()) {

                        this.drawing?.Children.Remove(setting.GetDrawing());

                        setting.OptionCalling -= OptionCalled;
                    }
                    break;
                case NotifyCollectionChangedAction.Reset:
                    this.drawing.Children.Clear();
                    break;
            }
        }

        private void OptionCalled(CustomObject sender, UpdateOptions option) {
            this.SetUpdateOption(sender, option);
        }

        
        public LineGroupsCollection SetDrawingGroup(DrawingGroup drawing) {

            this.drawing?.Children.Clear();

            this.drawing = drawing;

            // update with new rule
            foreach (var setting in this)
                this.drawing.Children.Add(setting.GetDrawing());

            return this;
        }


        public void RefreshLines() {
            foreach (var setting in this)
                setting.Refresh();
        }

        public void ClearLines() {
            foreach (var setting in this)
                setting.Clear();
        }

        public void UpdateLinePoint(UIElement element, int index, Point point, bool rebuild = true) {
            string names = ElementsPathDesigner.GetLineGroupNames(element);

            if (names == null) return;

            foreach (var split in names.Split(new char[] { ',' }, System.StringSplitOptions.RemoveEmptyEntries))
                foreach (var setting in this.Where(x => x.Name == split))
                    setting.UpdateLinePoint(index, point, rebuild);
        }
    }
}
