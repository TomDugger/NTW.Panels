using NTW.Panels;
using System.Windows;
using System.Windows.Media;

namespace Examples.Data {
    public sealed class RulerSetting : CustomObject {

        private GeometryGroup horizontalGeometry;
        private GeometryGroup HorizontalGeometry => horizontalGeometry ?? (horizontalGeometry =  GenerateGeometry());

        private GeometryGroup verticalGeometry;
        private GeometryGroup VerticalGeometry => verticalGeometry ?? (verticalGeometry = GenerateGeometry(false));

        #region Properties
        public double TickHeight {
            get { return (double)GetValue(TickHeightProperty); }
            set { SetValue(TickHeightProperty, value); }
        }

        public static readonly DependencyProperty TickHeightProperty =
            DependencyProperty.Register("TickHeight", typeof(double), typeof(RulerSetting), new OptionPropertyMetadata(10.0, UpdateOptions.ParentUpdate, TickHeightChanged));

        private static void TickHeightChanged(DependencyObject sender, DependencyPropertyChangedEventArgs e) {
            if (sender is RulerSetting setting) {
                setting.Rebuild();
            }
        }


        public double TickStep {
            get { return (double)GetValue(TickStepProperty); }
            set { SetValue(TickStepProperty, value); }
        }

        public static readonly DependencyProperty TickStepProperty =
            DependencyProperty.Register("TickStep", typeof(double), typeof(RulerSetting), new OptionPropertyMetadata(50.0, UpdateOptions.ParentUpdate, TickStepChanged));

        private static void TickStepChanged(DependencyObject sender, DependencyPropertyChangedEventArgs e) {
            if (sender is RulerSetting setting) {
                setting.Rebuild();
            }
        }


        public int MiddleTickQuantity {
            get { return (int)GetValue(MiddleTickQuantityProperty); }
            set { SetValue(MiddleTickQuantityProperty, value); }
        }

        public static readonly DependencyProperty MiddleTickQuantityProperty =
            DependencyProperty.Register("MiddleTickQuantity", typeof(int), typeof(RulerSetting), new OptionPropertyMetadata(5, UpdateOptions.ParentUpdate, MiddleTickQuantityChanged));

        private static void MiddleTickQuantityChanged(DependencyObject sender, DependencyPropertyChangedEventArgs e) {
            if (sender is RulerSetting setting) setting.Rebuild();
        }


        public double MiddleTickHeight {
            get { return (double)GetValue(MiddleTickHeightProperty); }
            set { SetValue(MiddleTickHeightProperty, value); }
        }

        public static readonly DependencyProperty MiddleTickHeightProperty =
            DependencyProperty.Register("MiddleTickHeight", typeof(double), typeof(RulerSetting), new OptionPropertyMetadata(5.0, UpdateOptions.ParentUpdate, MiddleTickHeightChanged));

        private static void MiddleTickHeightChanged(DependencyObject sender, DependencyPropertyChangedEventArgs e) {
            if (sender is RulerSetting setting) setting.Rebuild();
        }
        #endregion

        #region Helps
        public DrawingBrush GetAsHorizontalBrush(double scale, double indent, Brush brush, double thickness) {

            // prepare geometry
            var geometry = HorizontalGeometry.Clone();
            geometry.Transform = new ScaleTransform(scale, 1);

            // generate geometry drawing
            var drawing = new GeometryDrawing(brush, new Pen(brush, thickness), geometry);

            // generate drawingBrush
            DrawingBrush result = new DrawingBrush(drawing) {
                Stretch = Stretch.None,
                ViewportUnits = BrushMappingMode.Absolute,
                Viewport = new Rect(indent, 0, this.TickStep * scale, this.TickHeight),
                AlignmentX = AlignmentX.Left,
                AlignmentY = AlignmentY.Top,
                TileMode = TileMode.FlipY,
            };

            return result;
        }

        public DrawingBrush GetAsVerticalBrush(double scale, double indent, Brush brush, double thickness) {
            // prepare geometry
            var geometry = VerticalGeometry.Clone();
            geometry.Transform = new ScaleTransform(1, scale);

            // generate geometry drawing
            var drawing = new GeometryDrawing(brush, new Pen(brush, thickness), geometry);

            // generate drawingBrush
            DrawingBrush result = new DrawingBrush(drawing) {
                Stretch = Stretch.None,
                ViewportUnits = BrushMappingMode.Absolute,
                Viewport = new Rect(0, indent, this.TickHeight, this.TickStep * scale),
                AlignmentX = AlignmentX.Left,
                AlignmentY = AlignmentY.Top,
                TileMode = TileMode.FlipX,
            };

            return result;
        }


        private RulerSetting ClearGeometry() {
            HorizontalGeometry.Children.Clear();
            VerticalGeometry.Children.Clear();
            return this;
        }

        private GeometryGroup GenerateGeometry(bool horizontal = true) {
            var rulerGeometry = new GeometryGroup();

            CreateGeometry(rulerGeometry, horizontal);

            return rulerGeometry;
        }

        private RulerSetting CreateGeometry(GeometryGroup geometry, bool horizontal = true) {

            // create lines
            geometry.Children.Add(CreateLine(0, TickHeight, horizontal));
            // calculate midlleLines
            var middleStep = TickStep / (MiddleTickQuantity + 1);
            for (int i = 0; i < MiddleTickQuantity; i++) {
                geometry.Children.Add(CreateLine(middleStep + i * middleStep, MiddleTickHeight, horizontal));
            }
            return this;
        }

        private LineGeometry CreateLine(double start, double height, bool horizontal = true) {
            if (horizontal)
                return new LineGeometry(new Point(start, 0), new Point(start, height));
            else
                return new LineGeometry(new Point(0, start), new Point(height, start));
        }

        private void Rebuild() {
            ClearGeometry().CreateGeometry(horizontalGeometry).CreateGeometry(verticalGeometry, false);
        }
        #endregion

        protected override Freezable CreateInstanceCore() {
            return new RulerSetting();
        }
    }
}
