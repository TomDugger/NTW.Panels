using NTW.Panels;
using System;
using System.Linq;
using System.Windows;

namespace Examples.Locators {
    public class HierarchicalLocator : CustomLocator {

        #region Propeties
        public static double GetOffset(DependencyObject obj) {
            return (double)obj.GetValue(OffsetProperty);
        }

        public static void SetOffset(DependencyObject obj, double value) {
            obj.SetValue(OffsetProperty, value);
        }

        public static readonly DependencyProperty OffsetProperty =
            DependencyProperty.RegisterAttached("Offset", typeof(double), typeof(HierarchicalLocator), new PropertyMetadata(0.0, OffsetChanged));

        private static void OffsetChanged(DependencyObject sender, DependencyPropertyChangedEventArgs e) {
            if (sender is UIElement ui && GetRebuildArrangeChild(sender) is Action<UIElement> rebuild)
                rebuild(ui);
        }

        public Size ItemSize {
            get { return (Size)GetValue(ItemSizeProperty); }
            set { SetValue(ItemSizeProperty, value); }
        }

        public static readonly DependencyProperty ItemSizeProperty =
            DependencyProperty.Register("ItemSize", typeof(Size), typeof(HierarchicalLocator), new OptionPropertyMetadata(new Size(15, 15), UpdateOptions.Arrange));



        public double BeginHeight {
            get { return (double)GetValue(BeginHeightProperty); }
            set { SetValue(BeginHeightProperty, value); }
        }

        public static readonly DependencyProperty BeginHeightProperty =
            DependencyProperty.Register("BeginHeight", typeof(double), typeof(HierarchicalLocator), new OptionPropertyMetadata(0.0, UpdateOptions.Arrange));
        #endregion

        #region IItemsLocator
        public override Size Measure(Size originalSize, params UIElement[] elements) {
            foreach (UIElement child in elements) {
                child.Measure(originalSize);
            }

            return originalSize;
        }

        public override Size Arrange(Size originalSize, Vector offset, Vector itemsOffset, out Size verifySize, bool checkSize = false, params UIElement[] elements) {
            Rect[] rects = Enumerable.Range(0, elements.Length).Select(x => Rect.Empty).ToArray();

            var asList = elements.ToList();

            foreach (UIElement child in elements.OrderBy(x => GetOffset(x))) {
                int index = asList.IndexOf(child);

                Point position = default(Point);

                position.X = GetOffset(child) * originalSize.Width - (ItemSize.Width / 2);

                var result = rects[index] = ValidationRect(new Rect(position, ItemSize), rects.Where(x => !x.IsEmpty).ToArray(), originalSize);

                if (BeginHeight != 0)
                    result = new Rect(new Point(result.X, 0), new Size(ItemSize.Width, BeginHeight + result.Bottom));

                child.Arrange(result);
                SetRebuildArrangeChild(child, RefreshArrangeElementByOffsetValue);
            }

            return verifySize = originalSize;
        }

        public override Vector CalculateOffset(Size originalSize, Vector offset, UIElement element, bool asNext, params UIElement[] elements) {
            return default(Vector);
        }

        public override Rect GetOriginalBounds(UIElement element, Vector offset = default) {
            return default(Rect);
        }
        #endregion

        #region Helps
        private Rect ValidationRect(Rect rect, Rect[] rects, Size finalSize, double limitY = 0) {
            var copy = rect;
            var resultrect = new Rect[0];
            do {
                resultrect = rects.Where(x => x.Y == copy.Y && copy.IntersectsWith(x)).ToArray();

                foreach (Rect rec in resultrect) {
                    copy = new Rect(new Point(rect.X, rec.Bottom), rect.Size);
                }

                if (copy.Bottom > finalSize.Height) {
                    copy = new Rect(new Point(rect.X, rect.Y), rect.Size);
                    break;
                }
            }
            while (resultrect.Length > 0);
            return copy;
        }

        private void RefreshArrangeElementByOffsetValue(UIElement child) {
            this.SetUpdateOption(this, UpdateOptions.Arrange);
        }
        #endregion

        protected override Freezable CreateInstanceCore() {
            return new HierarchicalLocator();
        }
    }
}
