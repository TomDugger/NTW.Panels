using NTW.Panels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows;
using System.Windows.Media;

namespace Example.Locators {
    public class ChartLocator : Freezable, IItemsLocator, IDrawingPresenter {

        private Point center;

        private TransformGroup transform;
        private TranslateTransform translate;
        private ScaleTransform scale;

        private GeometryDrawing areaDrawing;
        private GeometryDrawing lineDrawing;
        private PathFigure lineFigure;

        public ChartLocator() {
            translate = new TranslateTransform();
            scale = new ScaleTransform();

            transform = new TransformGroup();
            transform.Children.Add(scale);
            transform.Children.Add(translate);

            areaDrawing = new GeometryDrawing { Brush = AreaFill, Pen = new Pen(AreaBorderBrush, AreaBorderThickness) };
            backDrawing.Children.Add(areaDrawing);

            PathGeometry pathGeometry = new PathGeometry();
            lineFigure = new PathFigure();
            pathGeometry.Figures.Add(lineFigure);

            lineDrawing = new GeometryDrawing { Brush = LineFill, Pen = new Pen(LineStroke, LineThickness), Geometry = pathGeometry };
            backDrawing.Children.Add(lineDrawing);
        }

        #region Properties
        #region Area properties
        public Size Area {
            get { return (Size)GetValue(AreaProperty); }
            set { SetValue(AreaProperty, value); }
        }

        public static readonly DependencyProperty AreaProperty =
            DependencyProperty.Register("Area", typeof(Size), typeof(ChartLocator), new PropertyMetadata(new Size(100, 100)));


        public Size ChildArea {
            get { return (Size)GetValue(ChildAreaProperty); }
            set { SetValue(ChildAreaProperty, value); }
        }

        public static readonly DependencyProperty ChildAreaProperty =
            DependencyProperty.Register("ChildArea", typeof(Size), typeof(ChartLocator), new PropertyMetadata(new Size(1, 1)));


        public Brush AreaFill {
            get { return (Brush)GetValue(AreaFillProperty); }
            set { SetValue(AreaFillProperty, value); }
        }

        public static readonly DependencyProperty AreaFillProperty =
            DependencyProperty.Register("AreaFill", typeof(Brush), typeof(ChartLocator), new PropertyMetadata(null, AreaFillChanged));

        private static void AreaFillChanged(DependencyObject sender, DependencyPropertyChangedEventArgs e) {
            var locator = (ChartLocator)sender;
            locator.areaDrawing.Brush = (Brush)e.NewValue;
        }


        public Brush AreaBorderBrush {
            get { return (Brush)GetValue(AreaBorderBrushProperty); }
            set { SetValue(AreaBorderBrushProperty, value); }
        }

        public static readonly DependencyProperty AreaBorderBrushProperty =
            DependencyProperty.Register("AreaBorderBrush", typeof(Brush), typeof(ChartLocator), new PropertyMetadata(Brushes.Black, AreaBorderBrushChanged));

        private static void AreaBorderBrushChanged(DependencyObject sender, DependencyPropertyChangedEventArgs e) {
            var locator = (ChartLocator)sender;
            locator.areaDrawing.Pen.Brush = (Brush)e.NewValue;
        }


        public double AreaBorderThickness {
            get { return (double)GetValue(AreaBorderThicknessProperty); }
            set { SetValue(AreaBorderThicknessProperty, value); }
        }

        public static readonly DependencyProperty AreaBorderThicknessProperty =
            DependencyProperty.Register("AreaBorderThickness", typeof(double), typeof(ChartLocator), new PropertyMetadata(1.0, AreaBorderThicknessChanged));

        private static void AreaBorderThicknessChanged(DependencyObject sender, DependencyPropertyChangedEventArgs e) {
            var locator = (ChartLocator)sender;
            locator.areaDrawing.Pen.Thickness = (double)e.NewValue;
        }


        public bool ShowArea {
            get { return (bool)GetValue(ShowAreaProperty); }
            set { SetValue(ShowAreaProperty, value); }
        }

        public static readonly DependencyProperty ShowAreaProperty =
            DependencyProperty.Register("ShowArea", typeof(bool), typeof(ChartLocator), new PropertyMetadata(true));


        public bool ShowAreaAsRuler {
            get { return (bool)GetValue(ShowAreaAsRulerProperty); }
            set { SetValue(ShowAreaAsRulerProperty, value); }
        }

        public static readonly DependencyProperty ShowAreaAsRulerProperty =
            DependencyProperty.Register("ShowAreaAsRuler", typeof(bool), typeof(ChartLocator), new PropertyMetadata(false));
        #endregion

        #region Line properties
        public bool ShowLine {
            get { return (bool)GetValue(ShowLineProperty); }
            set { SetValue(ShowLineProperty, value); }
        }

        public static readonly DependencyProperty ShowLineProperty =
            DependencyProperty.Register("ShowLine", typeof(bool), typeof(ChartLocator), new PropertyMetadata(false));


        public bool ClosedLine {
            get { return (bool)GetValue(ClosedLineProperty); }
            set { SetValue(ClosedLineProperty, value); }
        }

        public static readonly DependencyProperty ClosedLineProperty =
            DependencyProperty.Register("ClosedLine", typeof(bool), typeof(ChartLocator), new PropertyMetadata(false));


        public BindingLineTo BindLine {
            get { return (BindingLineTo)GetValue(BindLineProperty); }
            set { SetValue(BindLineProperty, value); }
        }

        public static readonly DependencyProperty BindLineProperty =
            DependencyProperty.Register("BindLine", typeof(BindingLineTo), typeof(ChartLocator), new PropertyMetadata(BindingLineTo.None));


        public bool FilledLine {
            get { return (bool)GetValue(FilledLineProperty); }
            set { SetValue(FilledLineProperty, value); }
        }

        public static readonly DependencyProperty FilledLineProperty =
            DependencyProperty.Register("FilledLine", typeof(bool), typeof(ChartLocator), new PropertyMetadata(false));



        public Brush LineFill {
            get { return (Brush)GetValue(LineFillProperty); }
            set { SetValue(LineFillProperty, value); }
        }

        public static readonly DependencyProperty LineFillProperty =
            DependencyProperty.Register("LineFill", typeof(Brush), typeof(ChartLocator), new PropertyMetadata(Brushes.DodgerBlue, LineFillChanged));

        private static void LineFillChanged(DependencyObject sender, DependencyPropertyChangedEventArgs e) {
            var locator = (ChartLocator)sender;
            locator.lineDrawing.Brush = (Brush)e.NewValue;
        }


        public Brush LineStroke {
            get { return (Brush)GetValue(LineStrokeProperty); }
            set { SetValue(LineStrokeProperty, value); }
        }

        public static readonly DependencyProperty LineStrokeProperty =
            DependencyProperty.Register("LineStroke", typeof(Brush), typeof(ChartLocator), new PropertyMetadata(Brushes.DarkBlue, LineStrokeChanged));

        private static void LineStrokeChanged(DependencyObject sender, DependencyPropertyChangedEventArgs e) {
            var locator = (ChartLocator)sender;
            locator.lineDrawing.Pen.Brush = (Brush)e.NewValue;
        }


        public double LineThickness {
            get { return (double)GetValue(LineThicknessProperty); }
            set { SetValue(LineThicknessProperty, value); }
        }

        public static readonly DependencyProperty LineThicknessProperty =
            DependencyProperty.Register("LineThickness", typeof(double), typeof(ChartLocator), new PropertyMetadata(1.0, LineThicknessChanged));

        private static void LineThicknessChanged(DependencyObject sender, DependencyPropertyChangedEventArgs e) {
            var locator = (ChartLocator)sender;
            locator.lineDrawing.Pen.Thickness = (double)e.NewValue;
        }

        #endregion

        #region TransformProperties
        public double TranslateX {
            get { return (double)GetValue(TranslateXProperty); }
            set { SetValue(TranslateXProperty, value); }
        }

        public static readonly DependencyProperty TranslateXProperty =
            DependencyProperty.Register("TranslateX", typeof(double), typeof(ChartLocator), new PropertyMetadata(0.0, TranslateXChanged));

        private static void TranslateXChanged(DependencyObject sender, DependencyPropertyChangedEventArgs e) {
            var locator = (ChartLocator)sender;
            locator.translate.X = (double)e.NewValue;
        }


        public double TranslateY {
            get { return (double)GetValue(TranslateYProperty); }
            set { SetValue(TranslateYProperty, value); }
        }

        public static readonly DependencyProperty TranslateYProperty =
            DependencyProperty.Register("TranslateY", typeof(double), typeof(ChartLocator), new PropertyMetadata(0.0, TranslateYChanged));

        private static void TranslateYChanged(DependencyObject sender, DependencyPropertyChangedEventArgs e) {
            var locator = (ChartLocator)sender;
            locator.translate.Y = (double)e.NewValue;
        }


        public double ScaleX {
            get { return (double)GetValue(ScaleXProperty); }
            set { SetValue(ScaleXProperty, value); }
        }

        public static readonly DependencyProperty ScaleXProperty =
            DependencyProperty.Register("ScaleX", typeof(double), typeof(ChartLocator), new FrameworkPropertyMetadata(1.0, ScaleXChanged));

        private static void ScaleXChanged(DependencyObject sender, DependencyPropertyChangedEventArgs e) {
            var locator = (ChartLocator)sender;
            locator.scale.ScaleX = (double)e.NewValue;
        }


        public double ScaleY {
            get { return (double)GetValue(ScaleYProperty); }
            set { SetValue(ScaleYProperty, value); }
        }

        public static readonly DependencyProperty ScaleYProperty =
            DependencyProperty.Register("ScaleY", typeof(double), typeof(ChartLocator), new PropertyMetadata(1.0, ScaleYChanged));

        private static void ScaleYChanged(DependencyObject sender, DependencyPropertyChangedEventArgs e) {
            var locator = (ChartLocator)sender;
            locator.scale.ScaleY = (double)e.NewValue;
        }


        public Size MinScale {
            get { return (Size)GetValue(MinScaleProperty); }
            set { SetValue(MinScaleProperty, value); }
        }

        public static readonly DependencyProperty MinScaleProperty =
            DependencyProperty.Register("MinScale", typeof(Size), typeof(ChartLocator), new PropertyMetadata(new Size(0.5, 0.5)));


        public Size MaxScale {
            get { return (Size)GetValue(MaxScaleProperty); }
            set { SetValue(MaxScaleProperty, value); }
        }

        public static readonly DependencyProperty MaxScaleProperty =
            DependencyProperty.Register("MaxScale", typeof(Size), typeof(ChartLocator), new PropertyMetadata(new Size(3, 3)));


        public Rect TranslateBounds {
            get { return (Rect)GetValue(TranslateBoundsProperty); }
            set { SetValue(TranslateBoundsProperty, value); }
        }

        public static readonly DependencyProperty TranslateBoundsProperty =
            DependencyProperty.Register("TranslateBounds", typeof(Rect), typeof(ChartLocator), new PropertyMetadata(new Rect(-100, -100, 100, 100)));
        #endregion

        public bool ClipMainArea {
            get { return (bool)GetValue(ClipMainAreaProperty); }
            set { SetValue(ClipMainAreaProperty, value); }
        }

        public static readonly DependencyProperty ClipMainAreaProperty =
            DependencyProperty.Register("ClipMainArea", typeof(bool), typeof(ChartLocator), new PropertyMetadata(true));
        #endregion

        #region Attached properties
        public static Point GetPosition(DependencyObject obj) {
            return (Point)obj.GetValue(PositionProperty);
        }

        public static void SetPosition(DependencyObject obj, Point value) {
            obj.SetValue(PositionProperty, value);
        }

        public static readonly DependencyProperty PositionProperty =
            DependencyProperty.RegisterAttached("Position", typeof(Point), typeof(ChartLocator), new FrameworkPropertyMetadata(default(Point), FrameworkPropertyMetadataOptions.AffectsParentArrange));


        private static bool GetIsChartItem(DependencyObject obj) {
            return (bool)obj.GetValue(IsChartItemProperty);
        }

        private static void SetIsChartItem(DependencyObject obj, bool value) {
            obj.SetValue(IsChartItemProperty, value);
        }

        private static readonly DependencyProperty IsChartItemProperty =
            DependencyProperty.RegisterAttached("IsChartItem", typeof(bool), typeof(ChartLocator), new PropertyMetadata(false));
        #endregion

        #region IDrawingPresenter
        private DrawingGroup backDrawing = new DrawingGroup();
        public Drawing BackDrawing => backDrawing;

        public Drawing FrontDrawing { get; } 
        #endregion

        #region IItemsLocator
        public Size Measure(Size originalSize, params UIElement[] elements) {

            foreach (UIElement child in elements) {
                child.Measure(originalSize);
            }

            return originalSize;
        }

        public Size Arrange(Size originalSize, Vector offset, Vector itemsOffset, out Size verifySize, bool checkSize = false, params UIElement[] elements) {

            verifySize = default(Size); // ignore ScrollView

            Action<Size, UIElement[]> display = DisplayWithoutLine;
            if (ShowLine)
                display = DisplayWithLine;

            display.Invoke(originalSize, elements);

            scale.CenterX = originalSize.Width / 2;
            scale.CenterY = originalSize.Height / 2;

            return originalSize;
        }

        public Vector CalculateOffset(Size originalSize, Vector offset, UIElement element, bool asNext, params UIElement[] elements) {
            return default(Vector);
        }

        public Rect GetOriginalBounds(UIElement element, Vector offset = default) {
            return default(Rect);
        }
        #endregion

        #region Helps
        public void SetTranslation(Vector deferent) {
            // Translate X
            if (TranslateX - deferent.X < TranslateBounds.Left)
                TranslateX = TranslateBounds.Left;
            else if (TranslateX - deferent.X > TranslateBounds.Width)
                TranslateX = TranslateBounds.Width;
            else
                TranslateX -= deferent.X;

            // Translate Y
            if (TranslateY - deferent.Y < TranslateBounds.Top)
                TranslateY = TranslateBounds.Top;
            else if (TranslateY - deferent.Y > TranslateBounds.Height)
                TranslateY = TranslateBounds.Height;
            else
                TranslateY -= deferent.Y;
        }

        public void SetScale(double tick) {
            if (ScaleX + tick < MinScale.Width)
                ScaleX = MinScale.Width;
            else if (ScaleX + tick > MaxScale.Width)
                ScaleX = MaxScale.Width;
            else
                ScaleX += tick;

            // Scale Y
            if (ScaleY + tick < MinScale.Height)
                ScaleY = MinScale.Height;
            else if (ScaleY + tick > MaxScale.Height)
                ScaleY = MaxScale.Height;
            else
                ScaleY += tick;
        }

        public Point Transformation(Point position) {
            Point result = default(Point);

            result = transform.Transform(new Point(center.X + Area.Width * position.X / ChildArea.Width, center.Y + Area.Height * position.Y / ChildArea.Height));

            return result;
        }

        public Point Translation(Point position) {
            Point result = default(Point);

            result = position;

            if (ClipMainArea) {
                Rect rect = transform.TransformBounds(new Rect(center, Area));

                double rX = result.X;
                double rY = result.Y;

                if (rX < rect.X)
                    rX = rect.X;
                else if (rX > rect.Right)
                    rX = rect.Right;

                if (rY < rect.Y)
                    rY = rect.Y;
                else if (rY > rect.Bottom)
                    rY = rect.Bottom;

                result = new Point(rX, rY);
            }

            result = transform.Inverse.Transform(result);

            result = new Point(ChildArea.Width * (result.X - center.X) / Area.Width, ChildArea.Height * (result.Y - center.Y) / Area.Height);

            return result;
        }

        public void SetMoveToChildPosition(Point childPosition) {

            TranslateX = TranslateY = 0;

            Point position = Transformation(childPosition);

            // translate to move position
            var result = center - position;

            TranslateX = result.X;
            TranslateY = result.Y;
        }

        private void UpdateArea(Size size) {
            if (!ShowArea) {
                areaDrawing.Geometry = null;
                return;
            }

            GeometryGroup areaGeometry = new GeometryGroup();

            int cw = (int)Math.Max(size.Width / Area.Width, size.Height / Area.Height);

            ScaleTransform s = new ScaleTransform { ScaleX = 1, ScaleY = 1, CenterX = size.Width / 2, CenterY = size.Height / 2 };

            for (int i = 0; i < cw; i++) {
                areaGeometry.Children.Add(new RectangleGeometry(transform.TransformBounds(s.TransformBounds(new Rect((size.Width - this.Area.Width) / 2, (size.Height - this.Area.Height) / 2, this.Area.Width, this.Area.Height)))));

                if (!ShowAreaAsRuler) break;

                s.ScaleX += 1;
                s.ScaleY += 1;
            }

            CombinedGeometry combine = new CombinedGeometry(GeometryCombineMode.Intersect, areaGeometry, new RectangleGeometry(new Rect(size)));

            areaDrawing.Geometry = combine;
        }


        private void DisplayWithoutLine(Size originalSize, params UIElement[] elements) {
            center = new Point((originalSize.Width - this.Area.Width) / 2, (originalSize.Height - this.Area.Height) / 2);

            foreach (UIElement child in elements) {
                Point position = Transformation(GetPosition(child));

                Point childPos = new Point(position.X - child.DesiredSize.Width / 2, position.Y - child.DesiredSize.Height / 2);

                child.Arrange(new Rect(childPos, child.DesiredSize));
                SetIsChartItem(child, true);
            }

            UpdateArea(originalSize);
        }

        private void DisplayWithLine(Size originalSize, params UIElement[] elements) {

            List<Point> linePoints = new List<Point>();

            center = new Point((originalSize.Width - this.Area.Width) / 2, (originalSize.Height - this.Area.Height) / 2);

            foreach (UIElement child in elements) {
                Point position = Transformation(GetPosition(child));

                linePoints.Add(position);

                Point childPos = new Point(position.X - child.DesiredSize.Width / 2, position.Y - child.DesiredSize.Height / 2);

                child.Arrange(new Rect(childPos, child.DesiredSize));
                SetIsChartItem(child, true);
            }

            UpdateArea(originalSize);

            if (ClosedLine) {
                var areaRect = transform.TransformBounds(new Rect((originalSize.Width - this.Area.Width) / 2, (originalSize.Height - this.Area.Height) / 2, this.Area.Width, this.Area.Height));

                switch (BindLine) {
                    case BindingLineTo.Left:
                        linePoints.Insert(0, areaRect.TopLeft);
                        linePoints.Add(areaRect.BottomLeft);
                        break;
                    case BindingLineTo.Top:
                        linePoints.Insert(0, areaRect.TopLeft);
                        linePoints.Add(areaRect.TopRight);
                        break;
                    case BindingLineTo.Right:
                        linePoints.Insert(0, areaRect.TopRight);
                        linePoints.Add(areaRect.BottomRight);
                        break;
                    case BindingLineTo.Bottom:
                        linePoints.Insert(0, areaRect.BottomLeft);
                        linePoints.Add(areaRect.BottomRight);
                        break;

                }
            }

            lineFigure.Segments.Clear();
            lineFigure.Segments.Add(new PolyLineSegment { Points = new PointCollection(linePoints) });
            lineFigure.StartPoint = linePoints.FirstOrDefault();
            lineFigure.IsClosed = ClosedLine;
            lineFigure.IsFilled = FilledLine;
        }
        #endregion

        protected override Freezable CreateInstanceCore() {
            return new ChartLocator();
        }
    }

    public enum BindingLineTo { 
        None,
        Left,
        Top,
        Right,
        Bottom,
    }
}
