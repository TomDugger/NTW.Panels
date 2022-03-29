using Examples.Expanse;
using NTW.Panels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows;
using System.Windows.Data;
using System.Windows.Media;

namespace Examples.Data {
    public sealed class LineGroupSetting : CustomObject {

        #region Members
        private GeometryDrawing lineDrawing;
        #endregion

        public LineGroupSetting() {

            lineDrawing = new GeometryDrawing { Brush = Fill, Pen = new Pen()};
            SetDepending(lineDrawing, GeometryDrawing.BrushProperty, this, FillProperty);
            SetDepending(lineDrawing.Pen, Pen.BrushProperty, this, StrokeProperty);
            SetDepending(lineDrawing.Pen, Pen.ThicknessProperty, this, ThicknessProperty);
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
            //if (sender is LineGroupSetting setting)
            //    if (e.NewValue is bool visiblity)
            //        if (visiblity)
            //            setting.drawing.Children.Add(setting.lineDrawing);
            //        else
            //            setting.drawing.Children.Remove(setting.lineDrawing);
        }

        public bool ClosedLine {
            get { return (bool)GetValue(ClosedLineProperty); }
            set { SetValue(ClosedLineProperty, value); }
        }

        public static readonly DependencyProperty ClosedLineProperty =
            DependencyProperty.Register("ClosedLine", typeof(bool), typeof(LineGroupSetting), new PropertyMetadata(false));


        public bool FilledLine {
            get { return (bool)GetValue(FilledLineProperty); }
            set { SetValue(FilledLineProperty, value); }
        }

        public static readonly DependencyProperty FilledLineProperty =
            DependencyProperty.Register("FilledLine", typeof(bool), typeof(LineGroupSetting), new PropertyMetadata(false));


        public SegmentSetting SegmentSetting {
            get { return (SegmentSetting)GetValue(SegmentSettingProperty); }
            set { SetValue(SegmentSettingProperty, value); }
        }

        public static readonly DependencyProperty SegmentSettingProperty =
            DependencyProperty.Register("SegmentSetting", typeof(SegmentSetting), typeof(LineGroupSetting), new PropertyMetadata(SegmentSetting.Line));

        #region Visual properties
        public Brush Fill {
            get { return (Brush)GetValue(FillProperty); }
            set { SetValue(FillProperty, value); }
        }

        public static readonly DependencyProperty FillProperty =
            DependencyProperty.Register("Fill", typeof(Brush), typeof(LineGroupSetting), new PropertyMetadata(Brushes.DodgerBlue));


        public Brush Stroke {
            get { return (Brush)GetValue(StrokeProperty); }
            set { SetValue(StrokeProperty, value); }
        }

        public static readonly DependencyProperty StrokeProperty =
            DependencyProperty.Register("Stroke", typeof(Brush), typeof(LineGroupSetting), new PropertyMetadata(Brushes.DarkBlue));


        public double Thickness {
            get { return (double)GetValue(ThicknessProperty); }
            set { SetValue(ThicknessProperty, value); }
        }

        public static readonly DependencyProperty ThicknessProperty =
            DependencyProperty.Register("Thickness", typeof(double), typeof(LineGroupSetting), new PropertyMetadata(1.0));
        #endregion
        #endregion

        #region Helps
        public Drawing GetDrawing(IEnumerable<Point> points) {
            var copy = this.lineDrawing.Clone();

            PathGeometry pathGeometry = new PathGeometry();
            pathGeometry.Figures.Add(GetLineFigure(points));
            copy.Geometry = pathGeometry;

            return copy;
        }

        private PathFigure GetLineFigure(IEnumerable<Point> points) {
            PathFigure figure = new PathFigure();

            figure.Segments.Add(GetPolyLine(points));
            figure.StartPoint = points.FirstOrDefault();
            SetDepending(figure, PathFigure.IsClosedProperty, this, ClosedLineProperty);
            SetDepending(figure, PathFigure.IsFilledProperty, this, FilledLineProperty);

            return figure;
        }

        private PathSegment GetPolyLine(IEnumerable<Point> points) {
            PathSegment result = null;

            switch (SegmentSetting.Type) {
                case SegmentTypes.Line:
                    result = new PolyLineSegment { Points = new PointCollection(points) };
                    break;
                case SegmentTypes.QuadraticBezier:

                    Point centerOfPoints = new Point(points.Sum(x => x.X) / points.Count(), points.Sum(x => x.Y) / points.Count());

                    List<Point> quadraticBezierPoints = points.WithPrevious().SelectMany((x, i) => GetQuadraticBezierPointsByCenter(centerOfPoints, x.Current, x.Previous)).ToList();

                    // if closed, add two last points
                    if (ClosedLine)
                        quadraticBezierPoints.AddRange(GetQuadraticBezierPointsByCenter(centerOfPoints, points.FirstOrDefault(), points.LastOrDefault()));

                    result = new PolyQuadraticBezierSegment { Points = new PointCollection(quadraticBezierPoints) };
                    break;
                case SegmentTypes.Bezier:

                    List<Point> bezierPoints = points.WithPrevious().SelectMany((x, i) => GetBezierPointsByCenter(x.Current, x.Previous)).ToList();

                    if (ClosedLine)
                        bezierPoints.AddRange(GetBezierPointsByCenter(points.FirstOrDefault(), points.LastOrDefault()));

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
