using NTW.Panels;
using System.Collections.Generic;
using System.Linq;
using System.Windows;
using System.Windows.Media;

namespace Examples.Designers {
    public class ElementsPathLineDesigner : CustomDesigner, IElementArrangeDesigner, IArrangeDesigner, IDrawingPresenter {

        private PathFigure lineFigure;
        private GeometryDrawing lineDrawing;

        List<Point> linePoints = new List<Point>();

        public ElementsPathLineDesigner() {
            PathGeometry pathGeometry = new PathGeometry();
            lineFigure = new PathFigure();
            pathGeometry.Figures.Add(lineFigure);

            lineDrawing = new GeometryDrawing { Brush = LineFill, Pen = new Pen(LineStroke, LineThickness), Geometry = pathGeometry };

            backDrawing.Children.Add(lineDrawing);
        }

        #region Properties
        public bool ShowLine {
            get { return (bool)GetValue(ShowLineProperty); }
            set { SetValue(ShowLineProperty, value); }
        }

        public static readonly DependencyProperty ShowLineProperty =
            DependencyProperty.Register("ShowLine", typeof(bool), typeof(ElementsPathLineDesigner), new PropertyMetadata(true, ShowLineChanged));

        private static void ShowLineChanged(DependencyObject sender, DependencyPropertyChangedEventArgs e) {
            if (sender is ElementsPathLineDesigner designer)
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
            DependencyProperty.Register("ClosedLine", typeof(bool), typeof(ElementsPathLineDesigner), new PropertyMetadata(false, ClosedLineChanged));

        private static void ClosedLineChanged(DependencyObject sender, DependencyPropertyChangedEventArgs e) {
            if (sender is ElementsPathLineDesigner designer)
                if(e.NewValue is bool isClosed)
                    designer.lineFigure.IsClosed = isClosed;
        }


        public bool FilledLine {
            get { return (bool)GetValue(FilledLineProperty); }
            set { SetValue(FilledLineProperty, value); }
        }

        public static readonly DependencyProperty FilledLineProperty =
            DependencyProperty.Register("FilledLine", typeof(bool), typeof(ElementsPathLineDesigner), new PropertyMetadata(false, FilledLineChanged));

        private static void FilledLineChanged(DependencyObject sender, DependencyPropertyChangedEventArgs e) {
            if (sender is ElementsPathLineDesigner designer)
                if (e.NewValue is bool isFilled)
                    designer.lineFigure.IsFilled = isFilled;
        }

        #region Visual properties
        public Brush LineFill {
            get { return (Brush)GetValue(LineFillProperty); }
            set { SetValue(LineFillProperty, value); }
        }

        public static readonly DependencyProperty LineFillProperty =
            DependencyProperty.Register("LineFill", typeof(Brush), typeof(ElementsPathLineDesigner), new PropertyMetadata(Brushes.DodgerBlue, LineFillChanged));

        private static void LineFillChanged(DependencyObject sender, DependencyPropertyChangedEventArgs e) {
            if (sender is ElementsPathLineDesigner designer)
                designer.lineDrawing.Brush = (Brush)e.NewValue;
        }


        public Brush LineStroke {
            get { return (Brush)GetValue(LineStrokeProperty); }
            set { SetValue(LineStrokeProperty, value); }
        }

        public static readonly DependencyProperty LineStrokeProperty =
            DependencyProperty.Register("LineStroke", typeof(Brush), typeof(ElementsPathLineDesigner), new PropertyMetadata(Brushes.DarkBlue, LineStrokeChanged));

        private static void LineStrokeChanged(DependencyObject sender, DependencyPropertyChangedEventArgs e) {
            if (sender is ElementsPathLineDesigner designer)
                designer.lineDrawing.Pen.Brush = (Brush)e.NewValue;
        }


        public double LineThickness {
            get { return (double)GetValue(LineThicknessProperty); }
            set { SetValue(LineThicknessProperty, value); }
        }

        public static readonly DependencyProperty LineThicknessProperty =
            DependencyProperty.Register("LineThickness", typeof(double), typeof(ElementsPathLineDesigner), new PropertyMetadata(1.0, LineThicknessChanged));

        private static void LineThicknessChanged(DependencyObject sender, DependencyPropertyChangedEventArgs e) {
            if (sender is ElementsPathLineDesigner designer)
                designer.lineDrawing.Pen.Thickness = (double)e.NewValue;
        }
        #endregion

        #endregion

        #region IElementArrangeDesigner
        public void AfterElementArrange(Rect elementRect, Size containerSize, UIElement element, Transform global = null) {
            // calculate the points
            var x = elementRect.X + elementRect.Width / 2;
            var y = elementRect.Y + elementRect.Height / 2;

            linePoints.Add(new Point(x, y));
        }

        public void UpdateElementArrage(Rect elementRect, Size containerSize, int index, UIElement element, Transform global = null) {
            var x = elementRect.X + elementRect.Width / 2;
            var y = elementRect.Y + elementRect.Height / 2;

            if (index >= 0 && index < linePoints.Count)
                linePoints[index] = new Point(x, y);

            lineFigure.Segments.Clear();
            lineFigure.Segments.Add(new PolyLineSegment { Points = new PointCollection(linePoints) });
            lineFigure.StartPoint = linePoints.FirstOrDefault();
        }
        #endregion

        #region IElementArrangeDesigner
        public void BeginElementArrange(Size containerSize, Transform global = null) {
            // clear the path
            linePoints.Clear();
        }

        public void EndElementArrange(Size containerSize, Transform global = null) {
            // build the line
            lineFigure.Segments.Clear();

            lineFigure.Segments.Add(new PolyLineSegment { Points = new PointCollection(linePoints) });
            lineFigure.StartPoint = linePoints.FirstOrDefault();
            lineFigure.IsClosed = ClosedLine;
            lineFigure.IsFilled = FilledLine;
        }
        #endregion

        #region IDrawingPresenter
        private DrawingGroup backDrawing = new DrawingGroup();
        public Drawing BackDrawing => backDrawing;

        public Drawing FrontDrawing { get; }
        #endregion

        protected override Freezable CreateInstanceCore() {
            return new ElementsPathLineDesigner();
        }
    }
}
