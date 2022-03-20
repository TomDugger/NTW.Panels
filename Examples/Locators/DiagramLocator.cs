using NTW.Panels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows;
using System.Windows.Media;

namespace Examples.Locators {
    public class DiagramLocator : DesignedLocator, IDrawingPresenter {

        private Size size;
        private DrawingGroup backGroup;
        private GeometryDrawing background;
        private GeometryDrawing middleStroke;

        public DiagramLocator() : base() {

            backGroup = new DrawingGroup();
            background = new GeometryDrawing { Brush = this.Fill, Pen = new Pen(this.Stroke, this.StrokeThickness) };
            backGroup.Children.Add(background);

            middleStroke = new GeometryDrawing { Pen = new Pen(this.Stroke, this.StrokeThickness) };

            backDrawing.Children.Add(backGroup);

            backDrawing.Children.Add(this.Designers.BackDrawing);
        }

        #region Depdendency properties
        public double InnerRadius {
            get { return (double)GetValue(InnerRadiusProperty); }
            set { SetValue(InnerRadiusProperty, value); }
        }

        public static readonly DependencyProperty InnerRadiusProperty =
            DependencyProperty.Register("InnerRadius", typeof(double), typeof(DiagramLocator), new OptionPropertyMetadata(0.0, UpdateOptions.Arrange));


        public double OuterRadius {
            get { return (double)GetValue(OuterRadiusProperty); }
            set { SetValue(OuterRadiusProperty, value); }
        }

        public static readonly DependencyProperty OuterRadiusProperty =
            DependencyProperty.Register("OuterRadius", typeof(double), typeof(DiagramLocator), new OptionPropertyMetadata(50.0, UpdateOptions.Arrange));


        public Brush Fill {
            get { return (Brush)GetValue(FillProperty); }
            set { SetValue(FillProperty, value); }
        }

        public static readonly DependencyProperty FillProperty =
            DependencyProperty.Register("Fill", typeof(Brush), typeof(DiagramLocator), new PropertyMetadata(Brushes.Yellow, FillChanged));

        private static void FillChanged(DependencyObject sender, DependencyPropertyChangedEventArgs e) {
            if (sender is DiagramLocator locator)
                locator.background.Brush = (Brush)e.NewValue;
        }


        public Brush Stroke {
            get { return (Brush)GetValue(StrokeProperty); }
            set { SetValue(StrokeProperty, value); }
        }

        public static readonly DependencyProperty StrokeProperty =
            DependencyProperty.Register("Stroke", typeof(Brush), typeof(DiagramLocator), new PropertyMetadata(Brushes.Black, StrokeChanged));

        private static void StrokeChanged(DependencyObject sender, DependencyPropertyChangedEventArgs e) {
            if (sender is DiagramLocator locator) {
                locator.background.Pen.Brush = (Brush)e.NewValue;
                locator.middleStroke.Pen.Brush = (Brush)e.NewValue;
            }
        }


        public double StrokeThickness {
            get { return (double)GetValue(StrokeThicknessProperty); }
            set { SetValue(StrokeThicknessProperty, value); }
        }

        public static readonly DependencyProperty StrokeThicknessProperty =
            DependencyProperty.Register("StrokeThickness", typeof(double), typeof(DiagramLocator), new PropertyMetadata(1.0, StrokeThicknessChanged));

        private static void StrokeThicknessChanged(DependencyObject sender, DependencyPropertyChangedEventArgs e) {
            if (sender is DiagramLocator locator) {
                locator.background.Pen.Thickness = (double)e.NewValue;
                locator.middleStroke.Pen.Thickness = (double)e.NewValue;
            }
        }


        public bool ShowMiddleLine {
            get { return (bool)GetValue(ShowMiddleLineProperty); }
            set { SetValue(ShowMiddleLineProperty, value); }
        }

        public static readonly DependencyProperty ShowMiddleLineProperty =
            DependencyProperty.Register("ShowMiddleLine", typeof(bool), typeof(DiagramLocator), new PropertyMetadata(false, ShowMiddleLineChanged));

        private static void ShowMiddleLineChanged(DependencyObject sender, DependencyPropertyChangedEventArgs e) {
            if (sender is DiagramLocator locator && e.NewValue is bool visibility)
                if (visibility)
                    locator.backGroup.Children.Add(locator.middleStroke);
                else
                    locator.backGroup.Children.Remove(locator.middleStroke);
        }
        #endregion

        #region Attached properties
        public static string GetLegend(DependencyObject obj) {
            return (string)obj.GetValue(LegendProperty);
        }

        public static void SetLegend(DependencyObject obj, string value) {
            obj.SetValue(LegendProperty, value);
        }

        public static readonly DependencyProperty LegendProperty =
            DependencyProperty.RegisterAttached("Legend", typeof(string), typeof(DiagramLocator), new PropertyMetadata(null));


        public static double GetAngle(DependencyObject obj) {
            return (double)obj.GetValue(AngleProperty);
        }

        public static readonly DependencyProperty AngleProperty =
            DependencyProperty.RegisterAttached("Angle", typeof(double), typeof(DiagramLocator), new PropertyMetadata(0.0, AngleChanged));

        private static void AngleChanged(DependencyObject sender, DependencyPropertyChangedEventArgs e) {
            double angle = (double)e.NewValue;

            sender.SetValue(ReverseAngleProperty, -angle);

            sender.SetValue(LessThan90Property, false);
            sender.SetValue(LessThan180Property, false);
            sender.SetValue(LessThan270Property, false);
            sender.SetValue(LessThan360Property, false);

            if (angle < 90)
                sender.SetValue(LessThan90Property, true);
            else if (angle < 180)
                sender.SetValue(LessThan180Property, true);
            else if (angle < 270)
                sender.SetValue(LessThan270Property, true);
            else if (angle < 360)
                sender.SetValue(LessThan360Property, true);
        }


        public static double GetReverseAngle(DependencyObject obj) {
            return (double)obj.GetValue(ReverseAngleProperty);
        }

        public static readonly DependencyProperty ReverseAngleProperty =
            DependencyProperty.RegisterAttached("ReverseAngle", typeof(double), typeof(DiagramLocator), new PropertyMetadata(0.0));


        public static bool GetLessThan90(DependencyObject obj) {
            return (bool)obj.GetValue(LessThan90Property);
        }

        private static void SetLessThan90(DependencyObject obj, bool value) {
            obj.SetValue(LessThan90Property, value);
        }

        public static readonly DependencyProperty LessThan90Property =
            DependencyProperty.RegisterAttached("LessThan90", typeof(bool), typeof(DiagramLocator), new PropertyMetadata(false));


        public static bool GetLessThan180(DependencyObject obj) {
            return (bool)obj.GetValue(LessThan180Property);
        }

        private static void SetLessThan180(DependencyObject obj, bool value) {
            obj.SetValue(LessThan180Property, value);
        }

        public static readonly DependencyProperty LessThan180Property =
            DependencyProperty.RegisterAttached("LessThan180", typeof(bool), typeof(DiagramLocator), new PropertyMetadata(false));


        public static bool GetLessThan270(DependencyObject obj) {
            return (bool)obj.GetValue(LessThan270Property);
        }

        private static void SetLessThan270(DependencyObject obj, bool value) {
            obj.SetValue(LessThan270Property, value);
        }

        public static readonly DependencyProperty LessThan270Property =
            DependencyProperty.RegisterAttached("LessThan270", typeof(bool), typeof(DiagramLocator), new PropertyMetadata(false));


        public static bool GetLessThan360(DependencyObject obj) {
            return (bool)obj.GetValue(LessThan360Property);
        }

        private static void SetLessThan360(DependencyObject obj, bool value) {
            obj.SetValue(LessThan360Property, value);
        }

        public static readonly DependencyProperty LessThan360Property =
            DependencyProperty.RegisterAttached("LessThan360", typeof(bool), typeof(DiagramLocator), new PropertyMetadata(false));


        public static double GetValue(DependencyObject obj) {
            return (double)obj.GetValue(ValueProperty);
        }

        public static void SetValue(DependencyObject obj, double value) {
            obj.SetValue(ValueProperty, value);
        }

        public static readonly DependencyProperty ValueProperty =
            DependencyProperty.RegisterAttached("Value", typeof(double), typeof(DiagramLocator), new PropertyMetadata(0.0, UpdateGeneralProperties));


        public static double GetMinimum(DependencyObject obj) {
            return (double)obj.GetValue(MinimumProperty);
        }

        public static void SetMinimum(DependencyObject obj, double value) {
            obj.SetValue(MinimumProperty, value);
        }

        public static readonly DependencyProperty MinimumProperty =
            DependencyProperty.RegisterAttached("Minimum", typeof(double), typeof(DiagramLocator), new PropertyMetadata(0.0, UpdateGeneralProperties));


        public static double GetMaximum(DependencyObject obj) {
            return (double)obj.GetValue(MaximumProperty);
        }

        public static void SetMaximum(DependencyObject obj, double value) {
            obj.SetValue(MaximumProperty, value);
        }

        public static readonly DependencyProperty MaximumProperty =
            DependencyProperty.RegisterAttached("Maximum", typeof(double), typeof(DiagramLocator), new PropertyMetadata(0.0, UpdateGeneralProperties));

        private static void UpdateGeneralProperties(DependencyObject sender, DependencyPropertyChangedEventArgs e) {
            if (sender is UIElement ui && GetRebuildArrangeChild(sender) is Action<UIElement> rebuild)
                rebuild(ui);
        }

        #endregion

        #region IDrawingPresenter
        DrawingGroup backDrawing = new DrawingGroup();
        public Drawing BackDrawing => backDrawing;

        public Drawing FrontDrawing { get; }
        #endregion

        #region IItemsLocator
        public override Size Measure(Size originalSize, params UIElement[] elements) {
            foreach (UIElement child in elements) {
                child.Measure(originalSize);
            }

            return originalSize;
        }

        public override Size Arrange(Size originalSize, Vector offset, Vector itemsOffset, out Size verifySize, bool checkSize = false, params UIElement[] elements) {

            verifySize = originalSize;

            size = originalSize;

            int number = 0;
            double angle = 360d / elements.Length;
            List<Point> outerPoints = new List<Point>();
            List<Point> innerPoints = new List<Point>();
            List<Point> middlegroundPoints = new List<Point>();

            // begin of Element Arrange Designers
            ExecuteFor<IArrangeDesigner>(designer => designer.BeginElementArrange(originalSize));

            foreach (UIElement element in elements) {
                element.Arrange(new Rect(new Point(originalSize.Width / 2, this.InnerRadius + this.OuterRadius - element.DesiredSize.Height / 2), new Size(this.OuterRadius, element.DesiredSize.Height)));

                #region set transform position of the element
                TransformGroup group = new TransformGroup();

                TranslateTransform translation = new TranslateTransform { X = this.InnerRadius };
                group.Children.Add(translation);
                #endregion

                element.SetValue(DiagramLocator.AngleProperty, angle * number);
                group.Children.Add(new RotateTransform(angle * number));
                element.RenderTransformOrigin = new Point(0, 0.5);
                element.RenderTransform = group;

                // elementArrange designers (setting)
                ExecuteFor<IElementArrangeDesigner>(designer => designer.AfterElementArrange(new Rect(GenerateElementPoint(element, originalSize, angle * number, DiagramLocator.GetValue(element)), new Size(1, 1)), originalSize, number, element));

                SetChildIndex(element, number);

                SetRebuildArrangeChild(element, ArrageChildWithCallingElementDesigners);

                // calculate maximum/middle point
                outerPoints.Add(GenerateElementPoint(element, originalSize, angle * number, DiagramLocator.GetMaximum(element)));
                innerPoints.Add(GenerateElementPoint(element, originalSize, angle * number, DiagramLocator.GetMinimum(element)));
                middlegroundPoints.Add(GenerateElementPoint(element, originalSize, angle * number, (DiagramLocator.GetMaximum(element) - DiagramLocator.GetMinimum(element)) / 2));

                number++;
            }

            // generate background
            SetBackgroundPoints(outerPoints, innerPoints);
            SetMiddlegroundPoints(middlegroundPoints);

            // end of Element Arrange Designers
            ExecuteFor<IArrangeDesigner>(designer => designer.EndElementArrange(originalSize));

            return originalSize;
        }

        public override Vector CalculateOffset(Size originalSize, Vector offset, UIElement element, bool asNext, params UIElement[] elements) {
            return default(Vector);
        }

        public override Rect GetOriginalBounds(UIElement element, Vector offset = default) {
            return default(Rect);
        }
        #endregion

        #region Helps
        private Point GenerateElementPoint(Visual visual, Size originalSize, double angle, double value) {
            if (visual == null) return default(Point);

            var minimum = DiagramLocator.GetMinimum(visual);
            var maximum = DiagramLocator.GetMaximum(visual);

            TransformGroup group = new TransformGroup();
            group.Children.Add(new TranslateTransform { X = this.InnerRadius });
            group.Children.Add(new RotateTransform(angle, originalSize.Width / 2, this.InnerRadius + this.OuterRadius));

            var distance = (value - minimum) * this.OuterRadius / (maximum - minimum) - this.InnerRadius;

            var linePoint = new Point(originalSize.Width / 2 + this.InnerRadius + distance, this.InnerRadius + this.OuterRadius);

            var result = group.Transform(linePoint);

            return result;
        }

        private void ArrageChildWithCallingElementDesigners(UIElement child) {

            Rect childBounds = new Rect(GenerateElementPoint(child, size, GetAngle(child), DiagramLocator.GetValue(child)), new Size(1, 1));

            // elementArrange designers (Updating)
            ExecuteFor<IElementArrangeDesigner>(designer => designer.UpdateElementArrage(childBounds, default(Size), GetChildIndex(child), child));
        }


        private void SetBackgroundPoints(IEnumerable<Point> outerPoints, IEnumerable<Point> innerPoints) {
            PathGeometry outerPathGeometry = GetGeometryByPoints(outerPoints);
            PathGeometry innerPathGeometry = GetGeometryByPoints(innerPoints);

            background.Geometry = CombinedGeometry.Combine(outerPathGeometry, innerPathGeometry, GeometryCombineMode.Exclude, Transform.Identity);
        }

        private void SetMiddlegroundPoints(IEnumerable<Point> points) {
            PathGeometry pathGeometry = new PathGeometry();
            PathFigure figure = new PathFigure { IsFilled = true, IsClosed = true };
            figure.StartPoint = points.FirstOrDefault();
            figure.Segments.Add(new PolyLineSegment { Points = new PointCollection(points) });
            pathGeometry.Figures.Add(figure);

            middleStroke.Geometry = pathGeometry;
        }

        private PathGeometry GetGeometryByPoints(IEnumerable<Point> points) {
            PathGeometry pathGeometry = new PathGeometry();
            PathFigure figure = new PathFigure { IsFilled = true, IsClosed = true };
            figure.StartPoint = points.FirstOrDefault();
            figure.Segments.Add(new PolyLineSegment { Points = new PointCollection(points) });
            pathGeometry.Figures.Add(figure);

            return pathGeometry;
        }
        #endregion

        protected override Freezable CreateInstanceCore() {
            return new DiagramLocator();
        }
    }
}
