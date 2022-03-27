using Examples.Expanse;
using NTW.Panels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows;
using System.Windows.Media;

namespace Examples.Data {
    public sealed class LineGroupSetting : CustomObject {

        #region Members
        private PathFigure lineFigure;
        private GeometryDrawing lineDrawing;
        private DrawingGroup drawing;

        Dictionary<int, Point> linePoints;
        #endregion

        public LineGroupSetting() {
            PathGeometry pathGeometry = new PathGeometry();
            lineFigure = new PathFigure();
            pathGeometry.Figures.Add(lineFigure);

            lineDrawing = new GeometryDrawing { Brush = LineFill, Pen = new Pen(LineStroke, LineThickness), Geometry = pathGeometry };

            drawing = new DrawingGroup();
            drawing.Children.Add(lineDrawing);

            linePoints = new Dictionary<int, Point>();
        }

        #region Dependency properties
        public string Name {
            get { return (string)GetValue(NameProperty); }
            set { SetValue(NameProperty, value); }
        }

        public static readonly DependencyProperty NameProperty =
            DependencyProperty.Register("Name", typeof(string), typeof(LineGroupSetting), new OptionPropertyMetadata(null, UpdateOptions.Arrange));


        public bool ShowLine {
            get { return (bool)GetValue(ShowLineProperty); }
            set { SetValue(ShowLineProperty, value); }
        }

        public static readonly DependencyProperty ShowLineProperty =
            DependencyProperty.Register("ShowLine", typeof(bool), typeof(LineGroupSetting), new PropertyMetadata(true, ShowLineChanged));

        private static void ShowLineChanged(DependencyObject sender, DependencyPropertyChangedEventArgs e) {
            if (sender is LineGroupSetting setting)
                if (e.NewValue is bool visiblity)
                    if (visiblity)
                        setting.drawing.Children.Add(setting.lineDrawing);
                    else
                        setting.drawing.Children.Remove(setting.lineDrawing);
        }

        public bool ClosedLine {
            get { return (bool)GetValue(ClosedLineProperty); }
            set { SetValue(ClosedLineProperty, value); }
        }

        public static readonly DependencyProperty ClosedLineProperty =
            DependencyProperty.Register("ClosedLine", typeof(bool), typeof(LineGroupSetting), new PropertyMetadata(false, ClosedLineChanged));

        private static void ClosedLineChanged(DependencyObject sender, DependencyPropertyChangedEventArgs e) {
            if (sender is LineGroupSetting setting) {
                if (e.NewValue is bool isClosed)
                    setting.lineFigure.IsClosed = isClosed;

                setting.RebuildPath();
            }
        }


        public bool FilledLine {
            get { return (bool)GetValue(FilledLineProperty); }
            set { SetValue(FilledLineProperty, value); }
        }

        public static readonly DependencyProperty FilledLineProperty =
            DependencyProperty.Register("FilledLine", typeof(bool), typeof(LineGroupSetting), new PropertyMetadata(false, FilledLineChanged));

        private static void FilledLineChanged(DependencyObject sender, DependencyPropertyChangedEventArgs e) {
            if (sender is LineGroupSetting setting)
                if (e.NewValue is bool isFilled)
                    setting.lineFigure.IsFilled = isFilled;
        }


        public SegmentSetting SegmentSetting {
            get { return (SegmentSetting)GetValue(SegmentSettingProperty); }
            set { SetValue(SegmentSettingProperty, value); }
        }

        public static readonly DependencyProperty SegmentSettingProperty =
            DependencyProperty.Register("SegmentSetting", typeof(SegmentSetting), typeof(LineGroupSetting), new PropertyMetadata(SegmentSetting.Line, SegmentSettingChanged));

        private static void SegmentSettingChanged(DependencyObject sender, DependencyPropertyChangedEventArgs e) {
            if (sender is LineGroupSetting setting)
                setting.RebuildPath();
        }

        #region Visual properties
        public Brush LineFill {
            get { return (Brush)GetValue(LineFillProperty); }
            set { SetValue(LineFillProperty, value); }
        }

        public static readonly DependencyProperty LineFillProperty =
            DependencyProperty.Register("LineFill", typeof(Brush), typeof(LineGroupSetting), new PropertyMetadata(Brushes.DodgerBlue, LineFillChanged));

        private static void LineFillChanged(DependencyObject sender, DependencyPropertyChangedEventArgs e) {
            if (sender is LineGroupSetting setting)
                setting.lineDrawing.Brush = (Brush)e.NewValue;
        }


        public Brush LineStroke {
            get { return (Brush)GetValue(LineStrokeProperty); }
            set { SetValue(LineStrokeProperty, value); }
        }

        public static readonly DependencyProperty LineStrokeProperty =
            DependencyProperty.Register("LineStroke", typeof(Brush), typeof(LineGroupSetting), new PropertyMetadata(Brushes.DarkBlue, LineStrokeChanged));

        private static void LineStrokeChanged(DependencyObject sender, DependencyPropertyChangedEventArgs e) {
            if (sender is LineGroupSetting setting)
                setting.lineDrawing.Pen.Brush = (Brush)e.NewValue;
        }


        public double LineThickness {
            get { return (double)GetValue(LineThicknessProperty); }
            set { SetValue(LineThicknessProperty, value); }
        }

        public static readonly DependencyProperty LineThicknessProperty =
            DependencyProperty.Register("LineThickness", typeof(double), typeof(LineGroupSetting), new PropertyMetadata(1.0, LineThicknessChanged));

        private static void LineThicknessChanged(DependencyObject sender, DependencyPropertyChangedEventArgs e) {
            if (sender is LineGroupSetting setting)
                setting.lineDrawing.Pen.Thickness = (double)e.NewValue;
        }
        #endregion
        #endregion

        #region Helps
        public Drawing GetDrawing() => drawing;

        public void Refresh() {
            this.RebuildPath();
        }

        public void Clear() {
            linePoints.Clear();
            lineFigure.Segments.Clear();
        }

        public void UpdateLinePoint(int index, Point point, bool rebuild = true) {
            linePoints[index] = point;

            if (rebuild)
                RebuildPath();
        }


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
            return new LineGroupSetting();
        }
    }
}
