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
            DependencyProperty.Register("ShowLine", typeof(bool), typeof(ElementsPathLineDesigner), new FrameworkPropertyMetadata(true, FrameworkPropertyMetadataOptions.AffectsParentArrange));

        public bool ClosedLine {
            get { return (bool)GetValue(ClosedLineProperty); }
            set { SetValue(ClosedLineProperty, value); }
        }

        public static readonly DependencyProperty ClosedLineProperty =
            DependencyProperty.Register("ClosedLine", typeof(bool), typeof(ElementsPathLineDesigner), new FrameworkPropertyMetadata(false, FrameworkPropertyMetadataOptions.AffectsParentArrange));


        public bool FilledLine {
            get { return (bool)GetValue(FilledLineProperty); }
            set { SetValue(FilledLineProperty, value); }
        }

        public static readonly DependencyProperty FilledLineProperty =
            DependencyProperty.Register("FilledLine", typeof(bool), typeof(ElementsPathLineDesigner), new FrameworkPropertyMetadata(false, FrameworkPropertyMetadataOptions.AffectsParentArrange));

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
            if (ShowLine) {
                var x = elementRect.X + elementRect.Width / 2;
                var y = elementRect.Y + elementRect.Height / 2;

                linePoints.Add(new Point(x, y));
            }
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

            if (ShowLine) {
                lineFigure.Segments.Add(new PolyLineSegment { Points = new PointCollection(linePoints) });
                lineFigure.StartPoint = linePoints.FirstOrDefault();
                lineFigure.IsClosed = ClosedLine;
                lineFigure.IsFilled = FilledLine;
            }
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
