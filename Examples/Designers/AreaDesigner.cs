using NTW.Panels;
using System.Windows;
using System.Windows.Media;

namespace Examples.Designers {
    public class AreaDesigner : CustomDesigner
        , IArrangeDesigner
        , IDrawingPresenter
        , ICalculatePositionDesigner
        , IAreaDesigner {

        private GeometryDrawing areaDrawing;

        public AreaDesigner() {
            areaDrawing = new GeometryDrawing { Brush = AreaFill, Pen = new Pen(AreaBorderBrush, AreaBorderThickness) };
            backDrawing.Children.Add(areaDrawing);
        }

        #region Properties
        public Brush AreaFill {
            get { return (Brush)GetValue(AreaFillProperty); }
            set { SetValue(AreaFillProperty, value); }
        }

        public static readonly DependencyProperty AreaFillProperty =
            DependencyProperty.Register("AreaFill", typeof(Brush), typeof(AreaDesigner), new PropertyMetadata(null, AreaFillChanged));

        private static void AreaFillChanged(DependencyObject sender, DependencyPropertyChangedEventArgs e) {

            if (sender is AreaDesigner designer)
                designer.areaDrawing.Brush = (Brush)e.NewValue;
        }


        public Brush AreaBorderBrush {
            get { return (Brush)GetValue(AreaBorderBrushProperty); }
            set { SetValue(AreaBorderBrushProperty, value); }
        }

        public static readonly DependencyProperty AreaBorderBrushProperty =
            DependencyProperty.Register("AreaBorderBrush", typeof(Brush), typeof(AreaDesigner), new PropertyMetadata(Brushes.Black, AreaBorderBrushChanged));

        private static void AreaBorderBrushChanged(DependencyObject sender, DependencyPropertyChangedEventArgs e) {

            if (sender is AreaDesigner designer)
                designer.areaDrawing.Pen.Brush = (Brush)e.NewValue;
        }


        public double AreaBorderThickness {
            get { return (double)GetValue(AreaBorderThicknessProperty); }
            set { SetValue(AreaBorderThicknessProperty, value); }
        }

        public static readonly DependencyProperty AreaBorderThicknessProperty =
            DependencyProperty.Register("AreaBorderThickness", typeof(double), typeof(AreaDesigner), new PropertyMetadata(1.0, AreaBorderThicknessChanged));

        private static void AreaBorderThicknessChanged(DependencyObject sender, DependencyPropertyChangedEventArgs e) {

            if (sender is AreaDesigner designer)
                designer.areaDrawing.Pen.Thickness = (double)e.NewValue;
        }


        public bool ShowArea {
            get { return (bool)GetValue(ShowAreaProperty); }
            set { SetValue(ShowAreaProperty, value); }
        }

        public static readonly DependencyProperty ShowAreaProperty =
            DependencyProperty.Register("ShowArea", typeof(bool), typeof(AreaDesigner), new PropertyMetadata(true));


        public bool ClipMainArea {
            get { return (bool)GetValue(ClipMainAreaProperty); }
            set { SetValue(ClipMainAreaProperty, value); }
        }

        public static readonly DependencyProperty ClipMainAreaProperty =
            DependencyProperty.Register("ClipMainArea", typeof(bool), typeof(AreaDesigner), new PropertyMetadata(true));
        #endregion

        #region IArrangeDesigner
        public void BeginElementArrange(Size containerSize, Transform global = null) {

        }

        public void EndElementArrange(Size containerSize, Transform global = null) {

            backDrawing.ClipGeometry = new RectangleGeometry(new Rect(containerSize));

            BuildArea(containerSize, global);
        }
        #endregion

        #region IDrawingPresenter
        private DrawingGroup backDrawing = new DrawingGroup();
        public Drawing BackDrawing => backDrawing;

        public Drawing FrontDrawing { get; }
        #endregion

        #region ICalculatePositionDesigner
        public Point ToGlobal(Point value, Point center, Transform global = null) {
            Point result = default(Point);

            global = global ?? Transform.Identity;

            result = global.Transform(new Point(center.X + Area.Width * value.X / ChildArea.Width, center.Y + Area.Height * value.Y / ChildArea.Height));

            return result;
        }

        public Point FromGlobal(Point position, Point center, Transform global = null) {
            Point result = default(Point);

            global = global ?? Transform.Identity;

            result = position;

            if (ClipMainArea) {
                Rect rect = global.TransformBounds(new Rect(center, Area));

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

            result = global.Inverse.Transform(result);

            result = new Point(ChildArea.Width * (result.X - center.X) / Area.Width, ChildArea.Height * (result.Y - center.Y) / Area.Height);

            return result;
        }

        public Point Center(Size containerSize, Transform global = null) {
            return new Point((containerSize.Width - this.Area.Width) / 2, (containerSize.Height - this.Area.Height) / 2);
        }
        #endregion

        #region IAreaDesigner
        public Size Area {
            get { return (Size)GetValue(AreaProperty); }
            set { SetValue(AreaProperty, value); }
        }

        public static readonly DependencyProperty AreaProperty =
            DependencyProperty.Register("Area", typeof(Size), typeof(AreaDesigner), new PropertyMetadata(new Size(100, 100)));


        public Size ChildArea {
            get { return (Size)GetValue(ChildAreaProperty); }
            set { SetValue(ChildAreaProperty, value); }
        }

        public static readonly DependencyProperty ChildAreaProperty =
            DependencyProperty.Register("ChildArea", typeof(Size), typeof(AreaDesigner), new PropertyMetadata(new Size(1, 1)));
        #endregion

        #region Helps
        private void BuildArea(Size containerSize, Transform global) {

            global = global ?? Transform.Identity;

            if (!ShowArea) {
                areaDrawing.Geometry = null;
                return;
            }

            Rect result = new Rect((containerSize.Width - this.Area.Width) / 2, (containerSize.Height - this.Area.Height) / 2, this.Area.Width, this.Area.Height);

            GeometryGroup areaGeometry = new GeometryGroup();

            areaGeometry.Children.Add(new RectangleGeometry(global.TransformBounds(result)));
            areaDrawing.Geometry = areaGeometry;
        }
        #endregion

        protected override Freezable CreateInstanceCore() {
            return new AreaDesigner();
        }
    }
}
