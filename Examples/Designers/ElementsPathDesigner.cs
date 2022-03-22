using NTW.Panels;
using System.Collections.Generic;
using System.Linq;
using System.Windows;
using System.Windows.Media;
using Examples.Data;

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
            if (sender is ElementsPathDesigner designer)
                if (e.NewValue is bool isClosed)
                    designer.lineFigure.IsClosed = isClosed;
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


        public SegmentTypes SegmentType {
            get { return (SegmentTypes)GetValue(SegmentTypeProperty); }
            set { SetValue(SegmentTypeProperty, value); }
        }

        public static readonly DependencyProperty SegmentTypeProperty =
            DependencyProperty.Register("SegmentType", typeof(SegmentTypes), typeof(ElementsPathDesigner), new PropertyMetadata(SegmentTypes.Line, SegmentTypeChanged));

        private static void SegmentTypeChanged(DependencyObject sender, DependencyPropertyChangedEventArgs e) {
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

            switch (SegmentType) {
                case SegmentTypes.Line:
                    result = new PolyLineSegment { Points = new PointCollection(linePoints.Values) };
                    break;
                case SegmentTypes.QuadraticBezier:


                    result = new PolyQuadraticBezierSegment { Points = new PointCollection(linePoints.Values.WithPrevious().SelectMany((x, i) => new Point[] { new Point(x.Current.X, x.Previous.Y), x.Current })) };
                    break;
                case SegmentTypes.Bezier:
                    result = new PolyBezierSegment { Points = new PointCollection(linePoints.Values.WithPrevious().SelectMany((x, i) => new Point[] { new Point(x.Current.X, x.Previous.Y), new Point(x.Previous.X, x.Current.Y), x.Current })) };
                    break;
            }

            return result;
        }
        #endregion

        protected override Freezable CreateInstanceCore() {
            return new ElementsPathDesigner();
        }
    }

    public static class EnumerableEx {
        public static IEnumerable<Pair<T>> WithPrevious<T>(this IEnumerable<T> source) {
            T previous = default;

            foreach (var item in source) {
                if (!previous.Equals(default(T)))
                    yield return new Pair<T>(item, previous);
                previous = item;
            }
        }
    }

    public struct Pair<T> {

        public Pair(T current, T previous) {
            this.Current = current;
            this.Previous = previous;
        }

        public T Current { get; }
        public T Previous { get; }
    }
}
