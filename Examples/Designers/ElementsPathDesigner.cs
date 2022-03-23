using NTW.Panels;
using System.Collections.Generic;
using System.Linq;
using System.Windows;
using System.Windows.Media;
using Examples.Data;
using Examples.Expanse;
using System;

namespace Examples.Designers {
    public class ElementsPathDesigner : CustomDesigner, IElementArrangeDesigner, IArrangeDesigner, IDrawingPresenter {

        private PathFigure lineFigure;
        private GeometryDrawing lineDrawing;

        Dictionary<int, Point> linePoints;

        public ElementsPathDesigner() {
            PathGeometry pathGeometry = new PathGeometry();
            lineFigure = new PathFigure();
            pathGeometry.Figures.Add(lineFigure);

            lineDrawing = new GeometryDrawing { Brush = LineFill, Pen = new Pen(LineStroke, LineThickness), Geometry = pathGeometry };

            backDrawing.Children.Add(lineDrawing);

            linePoints = new Dictionary<int, Point>();
        }

        #region Properties
        public bool ShowLine {
            get { return (bool)GetValue(ShowLineProperty); }
            set { SetValue(ShowLineProperty, value); }
        }

        public static readonly DependencyProperty ShowLineProperty =
            DependencyProperty.Register("ShowLine", typeof(bool), typeof(ElementsPathDesigner), new PropertyMetadata(true, ShowLineChanged));

        private static void ShowLineChanged(DependencyObject sender, DependencyPropertyChangedEventArgs e) {
            if (sender is ElementsPathDesigner designer)
                if (e.NewValue is bool visiblity)
                    if (visiblity)
                        designer.backDrawing.Children.Add(designer.lineDrawing);
                    else
                        designer.backDrawing.Children.Remove(designer.lineDrawing);
        }

        public bool ClosedLine {
            get { return (bool)GetValue(ClosedLineProperty); }
            set { SetValue(ClosedLineProperty, value); }
        }

        public static readonly DependencyProperty ClosedLineProperty =
            DependencyProperty.Register("ClosedLine", typeof(bool), typeof(ElementsPathDesigner), new PropertyMetadata(false, ClosedLineChanged));

        private static void ClosedLineChanged(DependencyObject sender, DependencyPropertyChangedEventArgs e) {
            if (sender is ElementsPathDesigner designer) {
                if (e.NewValue is bool isClosed)
                    designer.lineFigure.IsClosed = isClosed;

                designer.RebuildPath();
            }
        }


        public bool FilledLine {
            get { return (bool)GetValue(FilledLineProperty); }
            set { SetValue(FilledLineProperty, value); }
        }

        public static readonly DependencyProperty FilledLineProperty =
            DependencyProperty.Register("FilledLine", typeof(bool), typeof(ElementsPathDesigner), new PropertyMetadata(false, FilledLineChanged));

        private static void FilledLineChanged(DependencyObject sender, DependencyPropertyChangedEventArgs e) {
            if (sender is ElementsPathDesigner designer)
                if (e.NewValue is bool isFilled)
                    designer.lineFigure.IsFilled = isFilled;
        }


        public SegmentSetting SegmentSetting {
            get { return (SegmentSetting)GetValue(SegmentSettingProperty); }
            set { SetValue(SegmentSettingProperty, value); }
        }

        public static readonly DependencyProperty SegmentSettingProperty =
            DependencyProperty.Register("SegmentSetting", typeof(SegmentSetting), typeof(ElementsPathDesigner), new PropertyMetadata(SegmentSetting.Line, SegmentSettingChanged));

        private static void SegmentSettingChanged(DependencyObject sender, DependencyPropertyChangedEventArgs e) {
            if (sender is ElementsPathDesigner designer)
                designer.RebuildPath();
        }

        #region Visual properties
        public Brush LineFill {
            get { return (Brush)GetValue(LineFillProperty); }
            set { SetValue(LineFillProperty, value); }
        }

        public static readonly DependencyProperty LineFillProperty =
            DependencyProperty.Register("LineFill", typeof(Brush), typeof(ElementsPathDesigner), new PropertyMetadata(Brushes.DodgerBlue, LineFillChanged));

        private static void LineFillChanged(DependencyObject sender, DependencyPropertyChangedEventArgs e) {
            if (sender is ElementsPathDesigner designer)
                designer.lineDrawing.Brush = (Brush)e.NewValue;
        }


        public Brush LineStroke {
            get { return (Brush)GetValue(LineStrokeProperty); }
            set { SetValue(LineStrokeProperty, value); }
        }

        public static readonly DependencyProperty LineStrokeProperty =
            DependencyProperty.Register("LineStroke", typeof(Brush), typeof(ElementsPathDesigner), new PropertyMetadata(Brushes.DarkBlue, LineStrokeChanged));

        private static void LineStrokeChanged(DependencyObject sender, DependencyPropertyChangedEventArgs e) {
            if (sender is ElementsPathDesigner designer)
                designer.lineDrawing.Pen.Brush = (Brush)e.NewValue;
        }


        public double LineThickness {
            get { return (double)GetValue(LineThicknessProperty); }
            set { SetValue(LineThicknessProperty, value); }
        }

        public static readonly DependencyProperty LineThicknessProperty =
            DependencyProperty.Register("LineThickness", typeof(double), typeof(ElementsPathDesigner), new PropertyMetadata(1.0, LineThicknessChanged));

        private static void LineThicknessChanged(DependencyObject sender, DependencyPropertyChangedEventArgs e) {
            if (sender is ElementsPathDesigner designer)
                designer.lineDrawing.Pen.Thickness = (double)e.NewValue;
        }
        #endregion

        #endregion

        #region Attached properties
        public static bool GetNotOnLine(DependencyObject obj) {
            return (bool)obj.GetValue(NotOnLineProperty);
        }

        public static void SetNotOnLine(DependencyObject obj, bool value) {
            obj.SetValue(NotOnLineProperty, value);
        }

        public static readonly DependencyProperty NotOnLineProperty =
            DependencyProperty.RegisterAttached("NotOnLine", typeof(bool), typeof(ElementsPathDesigner), new PropertyMetadata(false));


        #endregion

        #region IElementArrangeDesigner
        public void AfterElementArrange(Rect elementRect, Size containerSize, int index, UIElement element, Transform global = null) {

            if (GetNotOnLine(element)) return;

            // calculate the points
            var x = elementRect.X + elementRect.Width / 2;
            var y = elementRect.Y + elementRect.Height / 2;

            linePoints[index] = new Point(x, y);
        }

        public void UpdateElementArrage(Rect elementRect, Size containerSize, int index, UIElement element, Transform global = null) {

            if (GetNotOnLine(element)) return;

            var x = elementRect.X + elementRect.Width / 2;
            var y = elementRect.Y + elementRect.Height / 2;

            linePoints[index] = new Point(x, y);

            lineFigure.Segments.Clear();
            lineFigure.Segments.Add(GetPolyLine());
            lineFigure.StartPoint = linePoints.FirstOrDefault().Value;
        }
        #endregion

        #region IElementArrangeDesigner
        public void BeginElementArrange(Size containerSize, Transform global = null) {
            // clear the path
            linePoints.Clear();
        }

        public void EndElementArrange(Size containerSize, Transform global = null) {
            // build the line
            RebuildPath();
        }
        #endregion

        #region IDrawingPresenter
        private DrawingGroup backDrawing = new DrawingGroup();
        public Drawing BackDrawing => backDrawing;

        public Drawing FrontDrawing { get; }
        #endregion

        #region Helps
        private void RebuildPath() {
            lineFigure.Segments.Clear();

            lineFigure.Segments.Add(GetPolyLine());
            lineFigure.StartPoint = linePoints.FirstOrDefault().Value;
            lineFigure.IsClosed = ClosedLine;
            lineFigure.IsFilled = FilledLine;
        }

        private PathSegment GetPolyLine() {
            PathSegment result = null;

            switch (SegmentSetting.Type) {
                case SegmentTypes.Line:
                    result = new PolyLineSegment { Points = new PointCollection(linePoints.Values) };
                    break;
                case SegmentTypes.QuadraticBezier:

                    Point centerOfPoints = new Point(linePoints.Values.Sum(x => x.X) / linePoints.Values.Count, linePoints.Values.Sum(x => x.Y) / linePoints.Values.Count);

                    List<Point> quadraticBezierPoints = linePoints.Values.WithPrevious().SelectMany((x, i) => GetQuadraticBezierPointsByCenter(centerOfPoints, x.Current, x.Previous)).ToList();

                    // if closed, add two last points
                    if (ClosedLine)
                        quadraticBezierPoints.AddRange(GetQuadraticBezierPointsByCenter(centerOfPoints, linePoints.Values.FirstOrDefault(), linePoints.Values.LastOrDefault()));

                    result = new PolyQuadraticBezierSegment { Points = new PointCollection(quadraticBezierPoints) };
                    break;
                case SegmentTypes.Bezier:

                    List<Point> bezierPoints = linePoints.Values.WithPrevious().SelectMany((x, i) => GetBezierPointsByCenter(x.Current, x.Previous)).ToList();

                    if (ClosedLine)
                        bezierPoints.AddRange(GetBezierPointsByCenter(linePoints.Values.FirstOrDefault(), linePoints.Values.LastOrDefault()));

                    result = new PolyBezierSegment { Points = new PointCollection(bezierPoints) };
                    break;
            }

            return result;
        }

        private IEnumerable<Point> GetQuadraticBezierPointsByCenter(Point center, Point current, Point previous) {

            Point point = default(Point);

            var fPoint = new Point(current.X, previous.Y);
            var sPoint = new Point(previous.X, current.Y);

            double fDistance = GetDistance(center, fPoint);
            double sDistance = GetDistance(center, sPoint);

            if (SegmentSetting.ToCenter) {
                if (fDistance < sDistance)
                    point = fPoint;
                else
                    point = sPoint;
            } else {
                if (fDistance > sDistance)
                    point = fPoint;
                else
                    point = sPoint;
            }

            return new Point[] { point, current };
        }

        private IEnumerable<Point> GetBezierPointsByCenter(Point current, Point previous) {
            if (SegmentSetting.Horizontal)
                return new Point[] { new Point(current.X, previous.Y), new Point(previous.X, current.Y), current };
            else
                return new Point[] { new Point(previous.X, current.Y), new Point(current.X, previous.Y), current };
        }

        private double GetDistance(Point fPoint, Point sPoint) => Math.Abs(Math.Sqrt(Math.Pow(sPoint.X - fPoint.X, 2) + Math.Pow(sPoint.Y - fPoint.Y, 2)));
        #endregion

        protected override Freezable CreateInstanceCore() {
            return new ElementsPathDesigner();
        }
    }
}
