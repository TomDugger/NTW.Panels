using NTW.Panels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows;
using System.Windows.Media;

namespace Examples.Designers {
    public class RulersDesigner : CustomDesigner, IArrangeDesigner, IDrawingPresenter {

        private Size containerSize;
        private Transform global;

        private DrawingGroup verticalRulerDrawing;
        private DrawingGroup horizontalRulerDrawing;

        public RulersDesigner() {
            verticalRulerDrawing = new DrawingGroup();
            frontDrawing.Children.Add(verticalRulerDrawing); verticalRulerDrawing.Transform = Transform.Identity;

            horizontalRulerDrawing = new DrawingGroup();
            frontDrawing.Children.Add(horizontalRulerDrawing);
        }

        #region Properties
        public bool ShowVerticalRuler {
            get { return (bool)GetValue(ShowVerticalRulerProperty); }
            set { SetValue(ShowVerticalRulerProperty, value); }
        }

        public static readonly DependencyProperty ShowVerticalRulerProperty =
            DependencyProperty.Register("ShowVerticalRuler", typeof(bool), typeof(RulersDesigner), new PropertyMetadata(false, UpdateVerticalRuler));


        public double MinimumVerticalRuler {
            get { return (double)GetValue(MinimumVerticalRulerProperty); }
            set { SetValue(MinimumVerticalRulerProperty, value); }
        }

        public static readonly DependencyProperty MinimumVerticalRulerProperty =
            DependencyProperty.Register("MinimumVerticalRuler", typeof(double), typeof(RulersDesigner), new PropertyMetadata(0.0, UpdateVerticalRuler));


        public double MaximumVerticalRuler {
            get { return (double)GetValue(MaximumVerticalRulerProperty); }
            set { SetValue(MaximumVerticalRulerProperty, value); }
        }

        public static readonly DependencyProperty MaximumVerticalRulerProperty =
            DependencyProperty.Register("MaximumVerticalRuler", typeof(double), typeof(RulersDesigner), new PropertyMetadata(10.0, UpdateVerticalRuler));


        public double FrequencyVerticalRuler {
            get { return (double)GetValue(FrequencyVerticalRulerProperty); }
            set { SetValue(FrequencyVerticalRulerProperty, value); }
        }

        public static readonly DependencyProperty FrequencyVerticalRulerProperty =
            DependencyProperty.Register("FrequencyVerticalRuler", typeof(double), typeof(RulersDesigner), new PropertyMetadata(1.0, UpdateVerticalRuler));

        private static void UpdateVerticalRuler(DependencyObject a, DependencyPropertyChangedEventArgs e) {
            if (a is RulersDesigner designer)
                designer.BuildVerticalRuler();
        }


        public bool ShowHorizontalRuler {
            get { return (bool)GetValue(ShowHorizontalRulerProperty); }
            set { SetValue(ShowHorizontalRulerProperty, value); }
        }

        public static readonly DependencyProperty ShowHorizontalRulerProperty =
            DependencyProperty.Register("ShowHorizontalRuler", typeof(bool), typeof(RulersDesigner), new PropertyMetadata(false));


        public double MinimumHorizontalRuler {
            get { return (double)GetValue(MinimumHorizontalRulerProperty); }
            set { SetValue(MinimumHorizontalRulerProperty, value); }
        }

        public static readonly DependencyProperty MinimumHorizontalRulerProperty =
            DependencyProperty.Register("MinimumHorizontalRuler", typeof(double), typeof(RulersDesigner), new PropertyMetadata(0.0, UpdateHorizontalRuler));


        public double MaximumHorizontalRuler {
            get { return (double)GetValue(MaximumHorizontalRulerProperty); }
            set { SetValue(MaximumHorizontalRulerProperty, value); }
        }

        public static readonly DependencyProperty MaximumHorizontalRulerProperty =
            DependencyProperty.Register("MaximumHorizontalRuler", typeof(double), typeof(RulersDesigner), new PropertyMetadata(10.0, UpdateHorizontalRuler));


        public double FrequencyHorizontalRuler {
            get { return (double)GetValue(FrequencyHorizontalRulerProperty); }
            set { SetValue(FrequencyHorizontalRulerProperty, value); }
        }

        public static readonly DependencyProperty FrequencyHorizontalRulerProperty =
            DependencyProperty.Register("FrequencyHorizontalRuler", typeof(double), typeof(RulersDesigner), new PropertyMetadata(1.0, UpdateHorizontalRuler));

        private static void UpdateHorizontalRuler(DependencyObject a, DependencyPropertyChangedEventArgs e) {
            if (a is RulersDesigner designer)
                designer.BuildHorizontalRuler();
        }


        public Brush RulerFill {
            get { return (Brush)GetValue(RulerFillProperty); }
            set { SetValue(RulerFillProperty, value); }
        }

        public static readonly DependencyProperty RulerFillProperty =
            DependencyProperty.Register("RulerFill", typeof(Brush), typeof(RulersDesigner), new PropertyMetadata(Brushes.Black, UpdateRulers));


        public double RulerWidth {
            get { return (double)GetValue(RulerWidthProperty); }
            set { SetValue(RulerWidthProperty, value); }
        }

        public static readonly DependencyProperty RulerWidthProperty =
            DependencyProperty.Register("RulerWidth", typeof(double), typeof(RulersDesigner), new PropertyMetadata(2.0, UpdateRulers));

        private static void UpdateRulers(DependencyObject a, DependencyPropertyChangedEventArgs e) {
            if (a is RulersDesigner designer) {
                // update onlu brush and thickness
                designer.BuildVerticalRuler();
                designer.BuildHorizontalRuler();
            }
        }

        public double RulerSmallHeight {
            get { return (double)GetValue(RulerSmallHeightProperty); }
            set { SetValue(RulerSmallHeightProperty, value); }
        }

        public static readonly DependencyProperty RulerSmallHeightProperty =
            DependencyProperty.Register("RulerSmallHeight", typeof(double), typeof(RulersDesigner), new PropertyMetadata(5.0, UpdateArea));


        public double RulerHeight {
            get { return (double)GetValue(RulerHeightProperty); }
            set { SetValue(RulerHeightProperty, value); }
        }

        public static readonly DependencyProperty RulerHeightProperty =
            DependencyProperty.Register("RulerHeight", typeof(double), typeof(RulersDesigner), new PropertyMetadata(10.0, UpdateArea));


        public int RulerBigFrequency {
            get { return (int)GetValue(RulerBigFrequencyProperty); }
            set { SetValue(RulerBigFrequencyProperty, value); }
        }

        public static readonly DependencyProperty RulerBigFrequencyProperty =
            DependencyProperty.Register("RulerBigFrequency", typeof(int), typeof(RulersDesigner), new PropertyMetadata(2, UpdateArea));


        public bool RulerDependsOnArea {
            get { return (bool)GetValue(RulerDependsOnAreaProperty); }
            set { SetValue(RulerDependsOnAreaProperty, value); }
        }

        public static readonly DependencyProperty RulerDependsOnAreaProperty =
            DependencyProperty.Register("RulerDependsOnArea", typeof(bool), typeof(RulersDesigner), new PropertyMetadata(false, UpdateArea));


        public IAreaDesigner AreaDesigner {
            get { return (IAreaDesigner)GetValue(AreaDesignerProperty); }
            set { SetValue(AreaDesignerProperty, value); }
        }

        public static readonly DependencyProperty AreaDesignerProperty =
            DependencyProperty.Register("AreaDesigner", typeof(IAreaDesigner), typeof(RulersDesigner), new PropertyMetadata(null, UpdateArea));


        public Brush AreaBorderBrush {
            get { return (Brush)GetValue(AreaBorderBrushProperty); }
            set { SetValue(AreaBorderBrushProperty, value); }
        }

        public static readonly DependencyProperty AreaBorderBrushProperty =
            DependencyProperty.Register("AreaBorderBrush", typeof(Brush), typeof(RulersDesigner), new PropertyMetadata(Brushes.Black, UpdateArea));

        private static void UpdateArea(DependencyObject sender, DependencyPropertyChangedEventArgs e) {
            if (sender is RulersDesigner designer) {

                designer.BuildVerticalRuler();
                designer.BuildHorizontalRuler();
            }
        }
        #endregion

        #region IDrawingPresenter
        public Drawing BackDrawing { get; }

        private DrawingGroup frontDrawing = new DrawingGroup();
        public Drawing FrontDrawing => frontDrawing;
        #endregion


        #region IArrangeDesigner
        public void BeginElementArrange(Size containerSize, Transform global = null) {
            verticalRulerDrawing.Children.Clear();
            horizontalRulerDrawing.Children.Clear();
        }

        public void EndElementArrange(Size containerSize, Transform global = null) {
            this.containerSize = containerSize;
            this.global = global;

            this.BuildVerticalRuler();
            this.BuildHorizontalRuler();
        }
        #endregion

        #region Helps
        private void BuildVerticalRuler() {

            if (ShowVerticalRuler) {

                if (RulerDependsOnArea && AreaDesigner != null)
                    BuildVerticalRulerByArea(containerSize);
                else
                    BuildVerticalRulerStandart(containerSize);
            }
        }

        private void BuildVerticalRulerStandart(Size originalSize) {

            double ScaleY = GetScale().Height;
            double TranslateY = GetTranslate().Y;

            var ticksCount = (Math.Abs(MaximumVerticalRuler) + Math.Abs(MinimumVerticalRuler)) / Math.Abs(FrequencyVerticalRuler) / ScaleY;

            var height = originalSize.Height - FrequencyVerticalRuler * 2;

            var step = height / ticksCount;

            double i = TranslateY;
            int index = 0;

            if (TranslateY > 0) {
                i = TranslateY % step;
                index = -(int)(TranslateY / step);
            }

            while (i <= height) {
                if (i >= 0) {

                    var endPoint = new Point(RulerHeight, FrequencyVerticalRuler + i);
                    if (index % RulerBigFrequency != 0)
                        endPoint = new Point(RulerSmallHeight, FrequencyVerticalRuler + i);

                    verticalRulerDrawing.Children.Add(CreateLine(new Point(0, FrequencyVerticalRuler + i), endPoint, RulerFill, RulerWidth));
                }

                i += step;
                index++;
            }
        }

        private void BuildVerticalRulerByArea(Size originalSize) {

            TransformGroup scale = new TransformGroup();
            foreach (var sc in RecursiveFinder<ScaleTransform>(global))
                scale.Children.Add(sc);

            double ScaleY = GetScale().Height;
            double TranslateY = GetTranslate().Y;

            var area = new Rect((originalSize.Width - this.AreaDesigner.Area.Width) / 2, (originalSize.Height - this.AreaDesigner.Area.Height) / 2, this.AreaDesigner.Area.Width, this.AreaDesigner.Area.Height);

            var scaledArea = scale.TransformBounds(area);

            var ticksCount = (Math.Abs(MaximumVerticalRuler) + Math.Abs(MinimumVerticalRuler)) / Math.Abs(FrequencyVerticalRuler) / ScaleY;

            var height = originalSize.Height - FrequencyVerticalRuler * 2;

            var step = area.Height / ticksCount;

            double i = TranslateY;
            int index = 0;

            if (TranslateY > 0) {
                i = TranslateY % step;
                index = -(int)(TranslateY / step);
            }

            if (area.Y > 0) {
                i += area.Y % step;
                index += -(int)(area.Y / step);
            }

            while (i <= height) {
                if (i >= 0) {

                    var endPoint = new Point(RulerHeight, FrequencyVerticalRuler + i);
                    if (index % RulerBigFrequency != 0)
                        endPoint = new Point(RulerSmallHeight, FrequencyVerticalRuler + i);

                    Brush stroke = RulerFill;
                    if (i >= scaledArea.Y + TranslateY && i <= scaledArea.Y + scaledArea.Height + TranslateY)
                        stroke = AreaBorderBrush;

                    verticalRulerDrawing.Children.Add(CreateLine(new Point(0, FrequencyVerticalRuler + i), endPoint, stroke, RulerWidth));
                }

                i += step;
                index++;
            }
        }


        private void BuildHorizontalRuler() {

            if (ShowHorizontalRuler) {

                if (RulerDependsOnArea && AreaDesigner != null)
                    BuildHorizontalRulerByArea(containerSize);
                else
                    BuildHorizontalRulerStandart(containerSize);
            }
        }

        private void BuildHorizontalRulerStandart(Size originalSize) {

            double ScaleX = GetScale().Width;
            double TranslateX = GetTranslate().X;

            var ticksCount = (Math.Abs(MaximumHorizontalRuler) + Math.Abs(MinimumHorizontalRuler)) / Math.Abs(FrequencyHorizontalRuler) / ScaleX;

            var width = originalSize.Width - FrequencyHorizontalRuler * 2;

            var step = width / ticksCount;

            double i = TranslateX;
            int index = 0;

            if (TranslateX > 0) {
                i = TranslateX % step;
                index = -(int)(TranslateX / step);
            }

            while (i <= width) {
                if (i >= 0) {

                    var endPoint = new Point(FrequencyHorizontalRuler + i, RulerHeight);
                    if (index % RulerBigFrequency != 0)
                        endPoint = new Point(FrequencyHorizontalRuler + i, RulerSmallHeight);

                    horizontalRulerDrawing.Children.Add(CreateLine(new Point(FrequencyHorizontalRuler + i, 0), endPoint, RulerFill, RulerWidth));
                }

                i += step;
                index++;
            }
        }

        private void BuildHorizontalRulerByArea(Size originalSize) {

            TransformGroup scale = new TransformGroup();
            foreach (var sc in RecursiveFinder<ScaleTransform>(global))
                scale.Children.Add(sc);

            double ScaleX = GetScale().Width;
            double TranslateX = GetTranslate().X;

            var area = new Rect((originalSize.Width - this.AreaDesigner.Area.Width) / 2, (originalSize.Height - this.AreaDesigner.Area.Height) / 2, this.AreaDesigner.Area.Width, this.AreaDesigner.Area.Height);

            var scaledArea = scale.TransformBounds(area);

            var ticksCount = (Math.Abs(MaximumHorizontalRuler) + Math.Abs(MinimumHorizontalRuler)) / Math.Abs(FrequencyHorizontalRuler) / ScaleX;

            var width = originalSize.Width - FrequencyHorizontalRuler * 2;

            var step = area.Width / ticksCount;

            double i = TranslateX;
            int index = 0;

            if (TranslateX > 0) {
                i = TranslateX % step;
                index = -(int)(TranslateX / step);
            }

            if (area.X > 0) {
                i += area.X % step;
                index += -(int)(area.X / step);
            }

            while (i <= width) {
                if (i >= 0) {

                    var endPoint = new Point(FrequencyHorizontalRuler + i, RulerHeight);
                    if (index % RulerBigFrequency != 0)
                        endPoint = new Point(FrequencyHorizontalRuler + i, RulerSmallHeight);

                    Brush stroke = RulerFill;
                    if (i >= scaledArea.X + TranslateX && i <= scaledArea.X + scaledArea.Width + TranslateX)
                        stroke = AreaBorderBrush;

                    horizontalRulerDrawing.Children.Add(CreateLine(new Point(FrequencyHorizontalRuler + i, 0), endPoint, stroke, RulerWidth));
                }

                i += step;
                index++;
            }
        }

        private Point GetTranslate() {
            if (global == null || global == Transform.Identity) return default(Point);

            Point result = default(Point);

            var all = RecursiveFinder<TranslateTransform>(global);
            if (all.Any())
                result = new Point(all.Sum(x => x.X), all.Sum(y => y.Y));

            return result;
        }

        private Size GetScale() {
            if (global == null || global == Transform.Identity) return new Size(1, 1);

            Size result = new Size(1, 1);

            var all = RecursiveFinder<ScaleTransform>(global);
            if (all.Any())
                result = new Size(all.Sum(x => x.ScaleX), all.Sum(y => y.ScaleY));

            return result;
        }

        private IEnumerable<T> RecursiveFinder<T>(Transform transform) where T : Transform {

            List<T> result = new List<T>();

            if (transform is T t)
                result.Add(t);
            else if (transform is TransformGroup group) {
                foreach (Transform inner in group.Children)
                    result.AddRange(RecursiveFinder<T>(inner));
            }

            return result;
        }

        private GeometryDrawing CreateLine(Point start, Point end, Brush stroke, double thickness) {
            GeometryDrawing result = new GeometryDrawing { Pen = new Pen(stroke, thickness) };
            result.Geometry = new LineGeometry(start, end);
            return result;
        }
        #endregion

        protected override Freezable CreateInstanceCore() {
            return new RulersDesigner();
        }
    }
}
