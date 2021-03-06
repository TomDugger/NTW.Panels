using NTW.Panels;
using System;
using System.Linq;
using System.Windows;
using System.Windows.Controls;

namespace Examples.Locators {
    public class ExcludeLocator : CustomLocator {

        #region Properties
        public Orientation Orientation {
            get { return (Orientation)GetValue(OrientationProperty); }
            set { SetValue(OrientationProperty, value); }
        }

        public static readonly DependencyProperty OrientationProperty =
            DependencyProperty.Register("Orientation", typeof(Orientation), typeof(ExcludeLocator), new OptionPropertyMetadata(default(Orientation), UpdateOptions.Arrange));


        public Size ExcludeSize {
            get { return (Size)GetValue(ExcludeSizeProperty); }
            set { SetValue(ExcludeSizeProperty, value); }
        }

        public static readonly DependencyProperty ExcludeSizeProperty =
            DependencyProperty.Register("ExcludeSize", typeof(Size), typeof(ExcludeLocator), new OptionPropertyMetadata(default(Size), UpdateOptions.Arrange));
        #endregion

        #region IItemsLocator
        public override Size Measure(Size originalSize, params UIElement[] elements) {
            Size panelDesiredSize = originalSize;

            foreach (UIElement child in elements) {
                child.Measure(originalSize);
                panelDesiredSize = child.DesiredSize;
            }

            switch (this.Orientation) {
                default:
                case Orientation.Vertical:
                    panelDesiredSize = new Size(panelDesiredSize.Width, elements.Sum(x => x.DesiredSize.Height));
                    break;
                case Orientation.Horizontal:
                    panelDesiredSize = new Size(elements.Sum(x => x.DesiredSize.Width), panelDesiredSize.Height);
                    break;
            }

            return SizeHelper.CheckInfinity(originalSize, panelDesiredSize);
        }

        public override Size Arrange(Size originalSize, Vector offset, Vector itemsOffset, out Size verifySize, bool checkSize = false, params UIElement[] elements) {
            if (elements.Length <= 1) {
                if (elements.Length > 0)
                    elements[0].Visibility = Visibility.Hidden;
            } else {

                Size childSize = default(Size);
                Point position = default(Point);

                int count = elements.Length % 2 == 0 ? elements.Length : elements.Length + 1;

                switch (Orientation) {
                    case Orientation.Horizontal:
                        childSize = new Size(Math.Abs(originalSize.Width - ExcludeSize.Width) / count, originalSize.Height);
                        position = new Point(Math.Abs(originalSize.Width - ExcludeSize.Width) / count, 0);
                        break;

                    case Orientation.Vertical:
                        childSize = new Size(originalSize.Width, Math.Abs(originalSize.Height - ExcludeSize.Height) / count);
                        position = new Point(0, Math.Abs(originalSize.Height - ExcludeSize.Height) / count);
                        break;
                }

                var asList = elements.ToList();

                foreach (UIElement child in asList) {
                    child.Visibility = Visibility.Visible;

                    int index = asList.IndexOf(child);

                    if (count > elements.Length && index == elements.Length - 1)
                        switch (Orientation) {
                            case Orientation.Horizontal:
                                childSize = new Size(Math.Abs(originalSize.Width - ExcludeSize.Width) / count * 2, originalSize.Height);
                                break;
                            case Orientation.Vertical:
                                childSize = new Size(originalSize.Width, Math.Abs(originalSize.Height - ExcludeSize.Height) / count * 2);
                                break;
                        }

                    if (position.X * index >= Math.Abs(originalSize.Width - ExcludeSize.Width) / 2)
                        child.Arrange(new Rect(new Point(ExcludeSize.Width + position.X * index, position.Y * index), childSize));

                    else if (position.Y * index > Math.Abs(originalSize.Height - ExcludeSize.Height) / 2)
                        child.Arrange(new Rect(new Point(position.X * index, ExcludeSize.Height + position.Y * index), childSize));

                    else
                        child.Arrange(new Rect(new Point(position.X * index, position.Y * index), childSize));
                }
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

        protected override Freezable CreateInstanceCore() {
            return new ExcludeLocator();
        }
    }
}
