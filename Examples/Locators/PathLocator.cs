using NTW.Panels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;

namespace Examples.Locators {
    public class PathLocator : CustomLocator {

        private Dictionary<UIElement, Rect> rects = new Dictionary<UIElement, Rect>();
        private PathGeometry pg;
        private double pathLength;


        #region Properties
        public PathGeometry GeometryPath {
            get { return (PathGeometry)GetValue(GeometryPathProperty); }
            set { SetValue(GeometryPathProperty, value); }
        }

        public static readonly DependencyProperty GeometryPathProperty =
            DependencyProperty.Register("GeometryPath", typeof(PathGeometry), typeof(PathLocator), new OptionPropertyMetadata(null, UpdateOptions.Measure));


        public Orientation Orientation {
            get { return (Orientation)GetValue(OrientationProperty); }
            set { SetValue(OrientationProperty, value); }
        }

        public static readonly DependencyProperty OrientationProperty =
            DependencyProperty.Register("Orientation", typeof(Orientation), typeof(PathLocator), new OptionPropertyMetadata(Orientation.Horizontal, UpdateOptions.Measure));
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

            double pos = 0;
            double max = 0;
            double nonUsed = 0;

            if (GeometryPath != null) {
                pg = PathGeometry.CreateFromGeometry(GeometryPath);
                pathLength = GetPathLength(pg);
            }

            var globalOffset = offset + itemsOffset;

            Rect place = new Rect((Point)globalOffset, originalSize);

            if (checkSize) {
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
                    rects[child] = funcArrange(child, originalSize, ref pos, ref nonUsed, ref max);
                }
            }

            verifySize = (Size)rects.Values.LastOrDefault().BottomRight;

            #region Calculate line position
            double linePosition = 0;

            switch (Orientation) {
                default:
                case Orientation.Vertical:

                    // find first element Start distance
                    if (elements.FirstOrDefault() is UIElement uiY) {
                        Rect first = rects[uiY];
                        linePosition = first.Y;
                    }

                    linePosition -= globalOffset.Y;
                    break;
                case Orientation.Horizontal:

                    // find first element Start distance
                    if (elements.FirstOrDefault() is UIElement uiX) {
                        Rect first = rects[uiX];
                        linePosition = first.X;
                    }

                    linePosition -= globalOffset.X;
                    break;
            }
            #endregion

            foreach (UIElement child in elements) {

                if (!rects.ContainsKey(child)) continue;

                Rect childRect = rects[child];

                double distance = 0;

                Point position = default(Point);
                switch (Orientation) {
                    case Orientation.Vertical:
                        distance = child.DesiredSize.Height;
                        break;
                    case Orientation.Horizontal:
                        distance = child.DesiredSize.Width;
                        break;
                }

                if (pg != null) {

                    double progress = linePosition / pathLength;
                    if (progress <= 1) {
                        pg.GetPointAtFractionLength(progress, out position, out Point outPoint);

                        var angle = GetAngle(outPoint);

                        distance = GetActualProgress(distance, Math.Abs(angle));
                    }

                    switch (Orientation) {
                        case Orientation.Vertical:
                            childRect = new Rect(new Point(position.X, childRect.Y), new Size(Math.Abs(childRect.Width - position.X), childRect.Height));
                            break;
                        case Orientation.Horizontal:
                            childRect = new Rect(new Point(childRect.X, position.Y), new Size(childRect.Width, Math.Abs(childRect.Height - position.Y)));
                            break;
                    }

                    linePosition += distance;
                }

                if (childRect.IntersectsWith(place)) {
                    childRect.Offset(-globalOffset);

                    child.Arrange(childRect);
                    child.Visibility = Visibility.Visible;
                } else
                    child.Visibility = Visibility.Hidden;
            }

            return originalSize;
        }

        public override Vector CalculateOffset(Size originalSize, Vector offset, UIElement element, bool asNext, params UIElement[] elements) {
            Vector result = offset;

            if (!asNext) return (Vector)rects[element].TopLeft; ;

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
        private double GetPathLength(PathGeometry path, double steps = 100) {
            Point pointOnPath;
            Point previousPointOnPath;
            Point tangent;

            double length = 0;

            path.GetPointAtFractionLength(0, out previousPointOnPath, out tangent);

            for (double progress = (1 / steps); progress < 1; progress += (1 / steps)) {
                path.GetPointAtFractionLength(progress, out pointOnPath, out tangent);
                length += Distance(previousPointOnPath, pointOnPath);
                previousPointOnPath = pointOnPath;
            }
            path.GetPointAtFractionLength(1, out pointOnPath, out tangent);
            length += Distance(previousPointOnPath, pointOnPath);

            return length;
        }

        private double Distance(Point fPoint, Point sPoint)
            => Math.Sqrt(Math.Pow(fPoint.X - sPoint.X, 2) + Math.Pow(fPoint.Y - sPoint.Y, 2));

        private double GetAngle(Point outPoint) {
            double result = 0;

            result = Math.Atan2(outPoint.X, outPoint.Y) * (180 / Math.PI) * -1;

            return result;
        }

        private double GetActualProgress(double distance, double angle) {
            double result = 0.0;

            result = distance / Math.Cos(angle);

            return result;
        }

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
            return new PathLocator { Orientation = Orientation };
        }
    }
}
