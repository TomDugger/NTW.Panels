using NTW.Panels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows;
using System.Windows.Controls;

namespace Examples.Locators {
    public class WrapLocator : CustomLocator {

        private Dictionary<UIElement, Rect> rects = new Dictionary<UIElement, Rect>();

        public Orientation Orientation {
            get { return (Orientation)GetValue(OrientationProperty); }
            set { SetValue(OrientationProperty, value); }
        }

        public static readonly DependencyProperty OrientationProperty =
            DependencyProperty.Register("Orientation", typeof(Orientation), typeof(WrapLocator), new OptionPropertyMetadata(Orientation.Horizontal, UpdateOptions.Measure));


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

            double posX = 0;
            double posY = 0;
            double max = 0;

            Rect place = new Rect((Point)offset, originalSize);

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
                    rects[child] = funcArrange(child, originalSize, ref posX, ref posY, ref max);

                if (rects.ContainsKey(child) && rects[child].IntersectsWith(place)) {

                    Rect rect = rects[child];
                    if (itemsOffset != default(Vector)) {
                        rect.Offset(-itemsOffset);

                        if (!place.Contains(rect)) {
                            rect.Offset(itemsOffset);

                            var rs = rects.Values.Cast<Rect>().ToList();
                            int currentIndex = rs.IndexOf(rect);

                            // find last rect of element
                            if (IsPositiveVector(itemsOffset)) {
                                var lastElementRect = rs[currentIndex - 1];
                                rect.Location = lastElementRect.Location;
                            } else {
                                var nextElementRect = rs[currentIndex + 1];
                                rect.Location = nextElementRect.Location;
                            }
                        }
                    }

                    rect.Offset(-offset);
                    child.Arrange(rect);
                    child.Visibility = Visibility.Visible;
                } else
                    child.Visibility = Visibility.Hidden;
            }

            verifySize = (Size)rects.Values.LastOrDefault().BottomRight;

            return originalSize;
        }


        public override Vector CalculateOffset(Size originalSize, Vector offset, UIElement element, bool asNext, params UIElement[] elements) {
            Vector result = offset;

            if (!asNext) return new Vector();

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

        public override Rect GetOriginalBounds(UIElement element, Vector offset = default(Vector)) => rects != null && rects.ContainsKey(element) ? Rect.Offset(rects[element], -offset) : default(Rect); 
        #endregion

        #region Helps
        private bool IsPositiveVector(Vector vector) => vector.X >= 0 && vector.Y >= 0;

        private Rect VerticalCalculation(UIElement child, Size originalSize, ref double posX, ref double posY, ref double maxWidth) {

            var result = new Rect(new Point(posX, posY), child.DesiredSize);

            maxWidth = Math.Max(maxWidth, child.DesiredSize.Width);
            posY += child.DesiredSize.Height;

            if (posY + child.DesiredSize.Height > originalSize.Height) {
                posY = 0;
                posX += maxWidth;
            }

            return result;
        }

        private Rect HorizontalCalculation(UIElement child, Size originalSize, ref double posX, ref double posY, ref double maxHeight) {

            var result = new Rect(new Point(posX, posY), child.DesiredSize);

            maxHeight = Math.Max(maxHeight, child.DesiredSize.Height);
            posX += child.DesiredSize.Width;

            if (posX + child.DesiredSize.Width > originalSize.Width) {
                posX = 0;
                posY += maxHeight;
            }

            return result;
        }
        #endregion

        protected override Freezable CreateInstanceCore() {
            return new WrapLocator();
        }
    }
}
