using NTW.Panels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows;
using System.Windows.Media;

namespace Example.Locators {
    public class DiagramLocator : Freezable, IItemsLocator, IDrawingPresenter, IDiagramLocator {

        private Size place;
        private IEnumerable<UIElement> elements;

        #region Depdendency properties

        public double InnerRadius {
            get { return (double)GetValue(InnerRadiusProperty); }
            set { SetValue(InnerRadiusProperty, value); }
        }

        public static readonly DependencyProperty InnerRadiusProperty =
            DependencyProperty.Register("InnerRadius", typeof(double), typeof(DiagramLocator), new PropertyMetadata(0.0));


        public double OuterRadius {
            get { return (double)GetValue(OuterRadiusProperty); }
            set { SetValue(OuterRadiusProperty, value); }
        }

        public static readonly DependencyProperty OuterRadiusProperty =
            DependencyProperty.Register("OuterRadius", typeof(double), typeof(DiagramLocator), new PropertyMetadata(50.0));


        public Brush Fill
        {
            get { return (Brush)GetValue(FillProperty); }
            set { SetValue(FillProperty, value); }
        }

        public static readonly DependencyProperty FillProperty =
            DependencyProperty.Register("Fill", typeof(Brush), typeof(DiagramLocator), new PropertyMetadata(null));


        public Brush Stroke {
            get { return (Brush)GetValue(StrokeProperty); }
            set { SetValue(StrokeProperty, value); }
        }

        public static readonly DependencyProperty StrokeProperty =
            DependencyProperty.Register("Stroke", typeof(Brush), typeof(DiagramLocator), new PropertyMetadata(Brushes.Black));


        public double StrokeThickness {
            get { return (double)GetValue(StrokeThicknessProperty); }
            set { SetValue(StrokeThicknessProperty, value); }
        }

        public static readonly DependencyProperty StrokeThicknessProperty =
            DependencyProperty.Register("StrokeThickness", typeof(double), typeof(DiagramLocator), new PropertyMetadata(1.0));



        public bool ShowMiddleLine
        {
            get { return (bool)GetValue(ShowMiddleLineProperty); }
            set { SetValue(ShowMiddleLineProperty, value); }
        }

        public static readonly DependencyProperty ShowMiddleLineProperty =
            DependencyProperty.Register("ShowMiddleLine", typeof(bool), typeof(DiagramLocator), new PropertyMetadata(false));




        public Brush DiagramFill {
            get { return (Brush)GetValue(DiagramFillProperty); }
            set { SetValue(DiagramFillProperty, value); }
        }

        public static readonly DependencyProperty DiagramFillProperty =
            DependencyProperty.Register("DiagramFill", typeof(Brush), typeof(DiagramLocator), new PropertyMetadata(Brushes.DodgerBlue));


        public Brush DiagramStroke {
            get { return (Brush)GetValue(DiagramStrokeProperty); }
            set { SetValue(DiagramStrokeProperty, value); }
        }

        public static readonly DependencyProperty DiagramStrokeProperty =
            DependencyProperty.Register("DiagramStroke", typeof(Brush), typeof(DiagramLocator), new PropertyMetadata(Brushes.Violet));


        public double DiagramStrokeThickness {
            get { return (double)GetValue(DiagramStrokeThicknessProperty); }
            set { SetValue(DiagramStrokeThicknessProperty, value); }
        }

        public static readonly DependencyProperty DiagramStrokeThicknessProperty =
            DependencyProperty.Register("DiagramStrokeThickness", typeof(double), typeof(DiagramLocator), new PropertyMetadata(1.0));


        public double DiagramOpacity {
            get { return (double)GetValue(DiagramOpacityProperty); }
            set { SetValue(DiagramOpacityProperty, value); }
        }

        public static readonly DependencyProperty DiagramOpacityProperty =
            DependencyProperty.Register("DiagramOpacity", typeof(double), typeof(DiagramLocator), new PropertyMetadata(1.0));
        #endregion

        #region IDrawingPresenter
        DrawingGroup backDrawing = new DrawingGroup();
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

            verifySize = originalSize;

            int number = 0;
            double angle = 360d / elements.Length;
            foreach (UIElement element in elements) {
                element.Arrange(new Rect(new Point(originalSize.Width / 2, this.InnerRadius * 2 + this.OuterRadius - element.DesiredSize.Height / 2), new Size(this.OuterRadius, element.DesiredSize.Height)));

                #region set transform position of the element
                TransformGroup group = new TransformGroup();

                TranslateTransform translation = new TranslateTransform() { X = this.InnerRadius };
                #endregion

                group.Children.Add(translation);

                element.SetValue(DiagramHelper.AngleProperty, angle * number);
                group.Children.Add(new RotateTransform(angle * number));
                element.RenderTransformOrigin = new Point(0, 0.5);
                element.RenderTransform = group;

                number++;
            }

            this.place = originalSize;
            this.elements = elements;

            // first update back drawing
            this.RebuildDiagram();

            return originalSize;
        }

        public Vector CalculateOffset(Size originalSize, Vector offset, UIElement element, bool asNext, params UIElement[] elements) {
            return default(Vector);
        }

        public Rect GetOriginalBounds(UIElement element, Vector offset = default) {
            return default(Rect);
        }
        #endregion

        #region IDiagramLocator
        public void RebuildDiagram() {
            backDrawing.Children.Clear();
            backDrawing.Children.Add(GetBackground());
            backDrawing.Children.Add(GetDiagram());
        }
        #endregion

        #region Helps
        private Drawing GetBackground()
        {
            DrawingGroup drawing = new DrawingGroup();

            if (elements == null) return drawing;

            double angleOne = 360d / elements.Count();
            TransformGroup group = new TransformGroup();
            RotateTransform rotate = new RotateTransform(0, place.Width / 2, this.InnerRadius * 2 + this.OuterRadius);
            group.Children.Add(new TranslateTransform { X = this.InnerRadius });
            group.Children.Add(rotate);

            var background = new PathGeometry();

            #region Outher ring
            Point start = new Point(place.Width / 2 + this.OuterRadius, this.InnerRadius * 2 + this.OuterRadius);

            background.Figures.Add(GetFigureByStartPoint(x => { rotate.Angle = x * angleOne; return group.Transform(start); }));
            #endregion

            #region Inner ring
            start = new Point(place.Width / 2 + this.InnerRadius, this.InnerRadius * 2 + this.OuterRadius);

            background.Figures.Add(GetFigureByStartPoint(x => { rotate.Angle = x * angleOne; return group.Transform(start); }));

            drawing.Children.Add(new GeometryDrawing(this.Fill, new Pen(this.Stroke, this.StrokeThickness), background));
            #endregion

            #region  meddle line
            if (ShowMiddleLine)
            {
                var middleLine = new PathGeometry();

                start = new Point(place.Width / 2 + this.OuterRadius / 2, this.InnerRadius * 2 + this.OuterRadius);

                middleLine.Figures.Add(GetFigureByStartPoint(x => { rotate.Angle = x * angleOne; return group.Transform(start); }));

                drawing.Children.Add(new GeometryDrawing(null, new Pen(this.Stroke, this.StrokeThickness), middleLine));
            }
            #endregion

            return drawing;
        }

        private Drawing GetDiagram()
        {
            DrawingGroup drawing = new DrawingGroup();
            drawing.Opacity = this.DiagramOpacity;

            if (elements == null) return drawing;

            double angleOne = 360d / elements.Count();
            TransformGroup group = new TransformGroup();
            RotateTransform rotate = new RotateTransform(0, place.Width / 2, this.InnerRadius * 2 + this.OuterRadius);
            group.Children.Add(new TranslateTransform { X = this.InnerRadius });
            group.Children.Add(rotate);

            var line = new PathGeometry();

            #region Outher ring
            line.Figures.Add(GetFigureByStartPoint(x => { rotate.Angle = x * angleOne; return group.Transform(GenerateElementPoint(x)); }));
            #endregion

            #region Inner ring
            Point start = new Point(place.Width / 2 + this.InnerRadius, this.InnerRadius * 2 + this.OuterRadius);

            line.Figures.Add(GetFigureByStartPoint(x => { rotate.Angle = x * angleOne; return group.Transform(start); }));
            #endregion

            drawing.Children.Add(new GeometryDrawing(this.DiagramFill, new Pen(this.DiagramStroke, this.DiagramStrokeThickness), line));
            return drawing;
        }

        private PathFigure GetFigureByStartPoint(Func<int, Point> func)
        {
            if (elements == null) return new PathFigure();

            var points = Enumerable.Range(0, elements.Count()).Select(x => func(x));
            var pathFigure = new PathFigure
            {
                IsClosed = true,
                StartPoint = points.First()
            };

            var polyline = new PolyLineSegment();
            polyline.Points = new PointCollection(points.Skip(1));

            pathFigure.Segments.Add(polyline);

            return pathFigure;
        }

        private Point GenerateElementPoint(int index)
        {
            if (elements == null) return default(Point);

            Visual visual = elements.ToList()[index];

            var value = DiagramHelper.GetValue(visual);
            var minimum = DiagramHelper.GetMinimum(visual);
            var maximum = DiagramHelper.GetMaximum(visual);

            return new Point(place.Width / 2 + this.InnerRadius + (value - minimum) * (this.OuterRadius - this.InnerRadius) / (maximum - minimum), this.InnerRadius * 2 + this.OuterRadius);
        }
        #endregion

        protected override Freezable CreateInstanceCore() {
            return new DiagramLocator();
        }
    }
}
