using NTW.Panels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows;
using System.Windows.Controls;

namespace Examples.Locators {
    public class StackLocator : Freezable, IItemsLocator {

        private Dictionary<UIElement, Rect> rects = new Dictionary<UIElement, Rect>();

        public Orientation Orientation {
            get { return (Orientation)GetValue(OrientationProperty); }
            set { SetValue(OrientationProperty, value); }
        }

        public static readonly DependencyProperty OrientationProperty =
            DependencyProperty.Register("Orientation", typeof(Orientation), typeof(StackLocator), new UIPropertyMetadata(Orientation.Vertical));


        public Size Measure(Size originalSize, params UIElement[] elements) {

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

        public Size Arrange(Size originalSize, Vector offset, Vector itemsOffset, out Size verifySize, bool checkSize = false, params UIElement[] elements) {

            double pos = 0;
            double max = 0;
            double nonUsed = 0;

            var globalOffset = offset + itemsOffset;
            Rect place = new Rect((Point)globalOffset, originalSize);

            if (checkSize)
                rects.Clear();

            ArrangeFunc funcArrange = null;
            switch (this.Orientation) {
                default:
                case Orientation.Vertical:
                    funcArrange = VerticalCalculation;
                    break;
                case Orientation.Horizontal:
                    funcArrange = HorizontalCalculation;
                    break;
            }

            foreach (UIElement child in elements) {

                if (checkSize)
                    rects[child] = funcArrange(child, originalSize, ref pos, ref nonUsed, ref max);

                if (!rects.ContainsKey(child)) continue;

                if (rects[child].IntersectsWith(place)) {
                    Rect rect = rects[child];
                    rect.Offset(-globalOffset);
                    child.Arrange(rect);
                    child.Visibility = Visibility.Visible;
                } else
                    child.Visibility = Visibility.Hidden;
            }

            verifySize = (Size)rects.Values.LastOrDefault().BottomRight;

            return originalSize;
        }


        public Vector CalculateOffset(Size originalSize, Vector offset, UIElement element, bool asNext, params UIElement[] elements) {
            Vector result = offset;

            if (!asNext) return (Vector)rects[element].TopLeft;

            switch (Orientation) {
                default:
                case Orientation.Vertical:
                    result = -new Vector(0, element.RenderSize.Height);
                    break;
                case Orientation.Horizontal:
                    result = -new Vector(element.RenderSize.Width, 0);
                    break;
            }

            return result;
        }

        public Rect GetOriginalBounds(UIElement element, Vector offset = default(Vector))
            => rects != null && rects.ContainsKey(element) ? Rect.Offset(rects[element], -offset) : default(Rect);

        #region Helps
        private Rect VerticalCalculation(UIElement child, Size originalSize, ref double pos, ref double nonUsed, ref double max) {
            var result = new Rect(0, pos, originalSize.Width, child.DesiredSize.Height);

            pos += child.DesiredSize.Height;
            max = Math.Max(max, child.DesiredSize.Width);

            return result;
        }

        private Rect HorizontalCalculation(UIElement child, Size originalSize, ref double pos, ref double nonUsed, ref double max) {
            var result = new Rect(pos, 0, child.DesiredSize.Width, originalSize.Height);

            pos += child.DesiredSize.Width;
            max = Math.Max(max, child.DesiredSize.Height);

            return result;
        }
        #endregion

        protected override Freezable CreateInstanceCore() {
            return new StackLocator();
        }
    }
}
