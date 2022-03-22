using Examples.Data;
using NTW.Panels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows;
using System.Windows.Media;

namespace Examples.Designers {
    public class RulersDesigner : CustomDesigner, IArrangeDesigner, IDrawingPresenter {

        private readonly RulerSetting DefaultRulerSettings;

        private Size containerSize;
        private Transform global;

        private DrawingGroup verticalRulerDrawing;
        private DrawingGroup horizontalRulerDrawing;

        private GeometryDrawing borderDrawing;

        private List<GeometryDrawing> horizontalRulersGroup;
        private List<GeometryDrawing> horizontalAreaGroup;

        private List<GeometryDrawing> verticalRulersGroup;
        private List<GeometryDrawing> verticalAreaGroup;

        public RulersDesigner() {
            DefaultRulerSettings = new RulerSetting();

            horizontalRulersGroup = new List<GeometryDrawing>();
            horizontalAreaGroup = new List<GeometryDrawing>();

            verticalRulersGroup = new List<GeometryDrawing>();
            verticalAreaGroup = new List<GeometryDrawing>();

            verticalRulerDrawing = new DrawingGroup();
            horizontalRulerDrawing = new DrawingGroup();

            borderDrawing = new GeometryDrawing(null, new Pen(this.RulerFill, this.BorderThickness), Geometry.Empty);
            this.frontDrawing.Children.Add(borderDrawing);
        }

        #region Properties
        public bool ShowVerticalRuler {
            get { return (bool)GetValue(ShowVerticalRulerProperty); }
            set { SetValue(ShowVerticalRulerProperty, value); }
        }

        public static readonly DependencyProperty ShowVerticalRulerProperty =
            DependencyProperty.Register("ShowVerticalRuler", typeof(bool), typeof(RulersDesigner), new PropertyMetadata(false, ShowVerticalRulerChanged));

        private static void ShowVerticalRulerChanged(DependencyObject sender, DependencyPropertyChangedEventArgs e) {
            if (sender is RulersDesigner designer)
                if (e.NewValue is bool visibility)
                    if (visibility) {
                        designer.frontDrawing.Children.Add(designer.verticalRulerDrawing);
                        designer.BuildVerticalRuler();
                        designer.BuildHorizontalRuler();
                    } else
                        designer.frontDrawing.Children.Remove(designer.verticalRulerDrawing);
        }


        public bool ShowHorizontalRuler {
            get { return (bool)GetValue(ShowHorizontalRulerProperty); }
            set { SetValue(ShowHorizontalRulerProperty, value); }
        }

        public static readonly DependencyProperty ShowHorizontalRulerProperty =
            DependencyProperty.Register("ShowHorizontalRuler", typeof(bool), typeof(RulersDesigner), new PropertyMetadata(false, ShowHorizontalRulerChanged));

        private static void ShowHorizontalRulerChanged(DependencyObject sender, DependencyPropertyChangedEventArgs e) {
            if (sender is RulersDesigner designer)
                if (e.NewValue is bool visibility)
                    if (visibility) {
                        designer.frontDrawing.Children.Add(designer.horizontalRulerDrawing);
                        designer.BuildHorizontalRuler();
                        designer.BuildVerticalRuler();
                    } else
                        designer.frontDrawing.Children.Remove(designer.horizontalRulerDrawing);
        }


        public RulerStretch VerticalRulerStretch {
            get { return (RulerStretch)GetValue(VerticalRulerStretchProperty); }
            set { SetValue(VerticalRulerStretchProperty, value); }
        }

        public static readonly DependencyProperty VerticalRulerStretchProperty =
            DependencyProperty.Register("VerticalRulerStretch", typeof(RulerStretch), typeof(RulersDesigner), new PropertyMetadata(RulerStretch.OnlyOnStart, VerticalRulerStretchChanged));

        private static void VerticalRulerStretchChanged(DependencyObject sender, DependencyPropertyChangedEventArgs e) {
            if (sender is RulersDesigner designer) {
                designer.verticalRulerDrawing.Children.Clear();
                designer.BuildVerticalRuler();
                designer.BuildHorizontalRuler();
            }
        }


        public RulerStretch HorizontalRulerStretch {
            get { return (RulerStretch)GetValue(HorizontalRulerStretchProperty); }
            set { SetValue(HorizontalRulerStretchProperty, value); }
        }

        public static readonly DependencyProperty HorizontalRulerStretchProperty =
            DependencyProperty.Register("HorizontalRulerStretch", typeof(RulerStretch), typeof(RulersDesigner), new PropertyMetadata(RulerStretch.OnlyOnStart, HorizontalRulerStretchChanged));

        private static void HorizontalRulerStretchChanged(DependencyObject sender, DependencyPropertyChangedEventArgs e) {
            if (sender is RulersDesigner designer) {
                designer.horizontalRulerDrawing.Children.Clear();
                designer.BuildHorizontalRuler();
                designer.BuildVerticalRuler();
            }
        }


        public RulerSetting VerticalRuler {
            get { return (RulerSetting)GetValue(VerticalRulerProperty); }
            set { SetValue(VerticalRulerProperty, value); }
        }

        public static readonly DependencyProperty VerticalRulerProperty =
            DependencyProperty.Register("VerticalRuler", typeof(RulerSetting), typeof(RulersDesigner), new PropertyMetadata(null, VerticalRulerChanged));

        private static void VerticalRulerChanged(DependencyObject sender, DependencyPropertyChangedEventArgs e) {
            if (sender is RulersDesigner designer) {
                designer.verticalRulerDrawing.Children.Clear();
                designer.BuildVerticalRuler();

                if (e.OldValue is RulerSetting oldSetting)
                    oldSetting.OptionCalling -= designer.VerticalUpdateOptionCalling;

                if(e.NewValue is RulerSetting newSetting)
                    newSetting.OptionCalling += designer.VerticalUpdateOptionCalling;
            }
        }

        private void VerticalUpdateOptionCalling(CustomObject sender, UpdateOptions option) {
            if (option == UpdateOptions.ParentUpdate) {
                verticalRulerDrawing.Children.Clear();
                BuildVerticalRuler();
            }
        }


        public RulerSetting HorizaontalRuler {
            get { return (RulerSetting)GetValue(HorizaontalRulerProperty); }
            set { SetValue(HorizaontalRulerProperty, value); }
        }

        public static readonly DependencyProperty HorizaontalRulerProperty =
            DependencyProperty.Register("HorizaontalRuler", typeof(RulerSetting), typeof(RulersDesigner), new PropertyMetadata(null, HorizaontalRulerChanged));

        private static void HorizaontalRulerChanged(DependencyObject sender, DependencyPropertyChangedEventArgs e) {
            if (sender is RulersDesigner designer) {
                designer.horizontalRulerDrawing.Children.Clear();
                designer.BuildHorizontalRuler();

                if (e.OldValue is RulerSetting oldSetting)
                    oldSetting.OptionCalling -= designer.HorizaontalUpdateOptionCalling;

                if (e.NewValue is RulerSetting newSetting)
                    newSetting.OptionCalling += designer.HorizaontalUpdateOptionCalling;
            }
        }

        private void HorizaontalUpdateOptionCalling(CustomObject sender, UpdateOptions option) {
            if (option == UpdateOptions.ParentUpdate) {
                horizontalRulerDrawing.Children.Clear();
                BuildHorizontalRuler();
            }
        }


        public StartPosition StartPosition {
            get { return (StartPosition)GetValue(StartPositionProperty); }
            set { SetValue(StartPositionProperty, value); }
        }

        public static readonly DependencyProperty StartPositionProperty =
            DependencyProperty.Register("StartPosition", typeof(StartPosition), typeof(RulersDesigner), new PropertyMetadata(StartPosition.OnCenter, StartPositionChanged));

        private static void StartPositionChanged(DependencyObject sender, DependencyPropertyChangedEventArgs e) {
            if (sender is RulersDesigner designer) {
                // update only brush and thickness
                designer.verticalRulerDrawing.Children.Clear();
                designer.horizontalRulerDrawing.Children.Clear();

                designer.BuildVerticalRuler();
                designer.BuildHorizontalRuler();
            }
        }


        public double BorderThickness {
            get { return (double)GetValue(BorderThicknessProperty); }
            set { SetValue(BorderThicknessProperty, value); }
        }

        public static readonly DependencyProperty BorderThicknessProperty =
            DependencyProperty.Register("BorderThickness", typeof(double), typeof(RulersDesigner), new PropertyMetadata(0.0, BorderThicknessChanged));

        private static void BorderThicknessChanged(DependencyObject sender, DependencyPropertyChangedEventArgs e) {
            if (sender is RulersDesigner designer)
                designer.borderDrawing.Pen.Thickness = (double)e.NewValue;
        }


        public Brush RulerFill {
            get { return (Brush)GetValue(RulerFillProperty); }
            set { SetValue(RulerFillProperty, value); }
        }

        public static readonly DependencyProperty RulerFillProperty =
            DependencyProperty.Register("RulerFill", typeof(Brush), typeof(RulersDesigner), new PropertyMetadata(Brushes.Black, RulerFillUpdate));

        private static void RulerFillUpdate(DependencyObject sender, DependencyPropertyChangedEventArgs e) {
            if (sender is RulersDesigner designer) {

                foreach (var drawing in designer.horizontalRulersGroup)
                    drawing.Pen.Brush = (Brush)e.NewValue;

                foreach (var drawing in designer.verticalRulersGroup)
                    drawing.Pen.Brush = (Brush)e.NewValue;

                designer.borderDrawing.Pen.Brush = (Brush)e.NewValue;
            }
        }


        public double RulerWidth {
            get { return (double)GetValue(RulerWidthProperty); }
            set { SetValue(RulerWidthProperty, value); }
        }

        public static readonly DependencyProperty RulerWidthProperty =
            DependencyProperty.Register("RulerWidth", typeof(double), typeof(RulersDesigner), new PropertyMetadata(1.0, RulerWidthChanged));

        private static void RulerWidthChanged(DependencyObject sender, DependencyPropertyChangedEventArgs e) {
            if (sender is RulersDesigner designer) {

                foreach (var drawing in designer.horizontalRulersGroup)
                    drawing.Pen.Thickness = (double)e.NewValue;

                foreach (var drawing in designer.verticalRulersGroup)
                    drawing.Pen.Thickness = (double)e.NewValue;

                foreach (var drawing in designer.horizontalAreaGroup)
                    drawing.Pen.Thickness = (double)e.NewValue;

                foreach (var drawing in designer.verticalAreaGroup)
                    drawing.Pen.Thickness = (double)e.NewValue;
            }
        }


        public bool RulersByArea {
            get { return (bool)GetValue(RulersByAreaProperty); }
            set { SetValue(RulersByAreaProperty, value); }
        }

        public static readonly DependencyProperty RulersByAreaProperty =
            DependencyProperty.Register("RulersByArea", typeof(bool), typeof(RulersDesigner), new PropertyMetadata(false, RulersByAreaChanged));

        private static void RulersByAreaChanged(DependencyObject sender, DependencyPropertyChangedEventArgs e) {
            if (sender is RulersDesigner designer) {
                designer.verticalRulerDrawing.Children.Clear();
                designer.horizontalRulerDrawing.Children.Clear();

                designer.BuildVerticalRuler();
                designer.BuildHorizontalRuler();
            }
        }

        public IAreaDesigner AreaDesigner {
            get { return (IAreaDesigner)GetValue(AreaDesignerProperty); }
            set { SetValue(AreaDesignerProperty, value); }
        }

        public static readonly DependencyProperty AreaDesignerProperty =
            DependencyProperty.Register("AreaDesigner", typeof(IAreaDesigner), typeof(RulersDesigner), new PropertyMetadata(null, AreaDesignerChanged));

        private static void AreaDesignerChanged(DependencyObject sender, DependencyPropertyChangedEventArgs e) {
            if (sender is RulersDesigner designer) {
                designer.verticalRulerDrawing.Children.Clear();
                designer.horizontalRulerDrawing.Children.Clear();

                designer.BuildVerticalRuler();
                designer.BuildHorizontalRuler();
            }
        }


        public Brush AreaSegmentFill {
            get { return (Brush)GetValue(AreaSegmentFillProperty); }
            set { SetValue(AreaSegmentFillProperty, value); }
        }

        public static readonly DependencyProperty AreaSegmentFillProperty =
            DependencyProperty.Register("AreaSegmentFill", typeof(Brush), typeof(RulersDesigner), new PropertyMetadata(Brushes.Violet, AreaBorderBrushChanged));

        private static void AreaBorderBrushChanged(DependencyObject sender, DependencyPropertyChangedEventArgs e) {
            if (sender is RulersDesigner designer) {
                foreach (var drawing in designer.horizontalAreaGroup)
                    drawing.Pen.Brush = (Brush)e.NewValue;

                foreach (var drawing in designer.verticalAreaGroup)
                    drawing.Pen.Brush = (Brush)e.NewValue;
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
            horizontalRulersGroup.Clear();
            horizontalAreaGroup.Clear();

            verticalRulersGroup.Clear();
            verticalAreaGroup.Clear();
        }

        public void EndElementArrange(Size containerSize, Transform global = null) {
            this.containerSize = containerSize;
            this.global = global;

            borderDrawing.Geometry = new RectangleGeometry(new Rect(containerSize));
            this.frontDrawing.ClipGeometry = borderDrawing.Geometry;

            this.BuildVerticalRuler();
            this.BuildHorizontalRuler();
        }
        #endregion

        #region Helps
        private void BuildVerticalRuler() {

            verticalRulersGroup.Clear();
            verticalAreaGroup.Clear();

            if (containerSize == default(Size)) return;

            verticalRulerDrawing.Children.Clear();

            Rect areaRect = default(Rect);

            // get indent by other ruler (it we have them) 
            Vector indent = GetIndentOfRuller(ShowHorizontalRuler, HorizaontalRuler, HorizontalRulerStretch, VerticalRulerStretch);

            double indentation = this.RulerWidth / 2;

            // get transform parameters
            double scaleY = GetScale().Height;
            double translate = GetTranslate().Y;
            double start = translate + GetStartPosition(indent, HorizontalRulerStretch, translate).Y - indentation;

            // generate ruler brush
            var brush = GenerateVerticalBrush(this.VerticalRuler, scaleY, start, this.RulerFill, this.RulerWidth);

            // get height of ruler
            double height = (this.VerticalRuler ?? DefaultRulerSettings).TickHeight;

            // check if need area presenting
            if (RulersByArea && AreaDesigner is IAreaDesigner areaDesigner)
                areaRect = new Rect(0, areaDesigner.Bounds.Y - indentation, height, areaDesigner.Bounds.Height + this.RulerWidth + indentation);

            AddRuler(verticalRulerDrawing, VerticalRulerStretch,
                new Rect(0, indent.X, height, this.containerSize.Height - indent.Y),
                brush,
                verticalRulersGroup,
                0, this.containerSize.Width,
                areaRect);

            // vertical clip geometry
            verticalRulerDrawing.ClipGeometry = new RectangleGeometry(new Rect(0, indent.X, this.containerSize.Width, this.containerSize.Height - indent.Y));

            if (areaRect != default(Rect)) {
                var areaBrush = GenerateVerticalBrush(this.VerticalRuler, scaleY, start, this.AreaSegmentFill, this.RulerWidth);

                AddRuler(verticalRulerDrawing, VerticalRulerStretch,
                    areaRect,
                    areaBrush,
                    verticalAreaGroup,
                    0, this.containerSize.Width);
            }
        }

        private void BuildHorizontalRuler() {

            horizontalRulersGroup.Clear();
            horizontalAreaGroup.Clear();

            if (containerSize == default(Size)) return;

            horizontalRulerDrawing.Children.Clear();

            Rect areaRect = default(Rect);

            // get indent
            Vector indent = GetIndentOfRuller(ShowVerticalRuler, VerticalRuler, VerticalRulerStretch, HorizontalRulerStretch);

            double indentation = this.RulerWidth / 2;

            // get transform parameters
            double scaleX = GetScale().Width;
            double translate = GetTranslate().X;
            double start = translate + GetStartPosition(indent, VerticalRulerStretch, translate).X - indentation;

            // generate ruler brush
            var brush = GenerateHorizontalBrush(this.HorizaontalRuler, scaleX, start, this.RulerFill, this.RulerWidth);

            // get height of ruler
            double height = (this.HorizaontalRuler ?? DefaultRulerSettings).TickHeight;

            // check if need area presenting
            if (RulersByArea && AreaDesigner is IAreaDesigner areaDesigner) 
                areaRect = new Rect(areaDesigner.Bounds.X - indentation, 0, areaDesigner.Bounds.Width + this.RulerWidth + indentation, height);

            AddRuler(horizontalRulerDrawing, HorizontalRulerStretch,
                new Rect(indent.X, 0, this.containerSize.Width - indent.Y, height),
                brush,
                horizontalRulersGroup,
                this.containerSize.Height, 0, 
                areaRect);

            // horizontal clip geometry
            horizontalRulerDrawing.ClipGeometry = new RectangleGeometry(new Rect(indent.X, 0, this.containerSize.Width - indent.Y, this.containerSize.Height));

            if (areaRect != default(Rect)) {
                var areaBrush = GenerateHorizontalBrush(this.HorizaontalRuler, scaleX, start, this.AreaSegmentFill, this.RulerWidth);

                AddRuler(horizontalRulerDrawing, HorizontalRulerStretch,
                    areaRect,
                    areaBrush,
                    horizontalAreaGroup,
                    this.containerSize.Height);
            }
        }

        private DrawingBrush GenerateHorizontalBrush(RulerSetting setting, double scale, double indent, Brush brush, double thickness) {
            return (setting ?? DefaultRulerSettings).GetAsHorizontalBrush(scale, indent, brush, thickness);
        }

        private DrawingBrush GenerateVerticalBrush(RulerSetting setting, double scale, double indent, Brush brush, double thickness) {
            return (setting ?? DefaultRulerSettings).GetAsVerticalBrush(scale, indent, brush, thickness);
        }

        private void AddRuler(DrawingGroup group, 
            RulerStretch stretch,
            Rect place,
            Brush brush,
            IList<GeometryDrawing> rulerGeometriesGroup,
            double indentY = 0, double indentX = 0, params Rect[] masks) {

            if (stretch == RulerStretch.OnlyOnStart || stretch == RulerStretch.Duplicate)
                group.Children.Add(GetRulerDrawing(place, brush, rulerGeometriesGroup,0, 0, masks));

            if(stretch == RulerStretch.OnlyOnEnd || stretch == RulerStretch.Duplicate)
                group.Children.Add(GetRulerDrawing(GetInversBounds(place), brush, rulerGeometriesGroup, indentY, indentX, masks.Select(mask => GetInversBounds(mask)).ToArray()));
        }

        private Drawing GetRulerDrawing(Rect place,
            Brush brush, 
            IList<GeometryDrawing> rulerGeometriesGroup,
            double indentY = 0, double indentX = 0, params Rect[] masks) {

            DrawingGroup result = new DrawingGroup();

            // all the time clone main brush
            var resultBrush = (DrawingBrush)brush.Clone();
            
            // add copied drawing
            rulerGeometriesGroup.Add((GeometryDrawing)resultBrush.Drawing);

            // duplicate
            resultBrush.Viewport = Rect.Offset(resultBrush.Viewport, indentX, indentY);

            Geometry resultGeometry = new RectangleGeometry(place);

            foreach(var mask in masks)
                resultGeometry = Geometry.Combine(resultGeometry, new RectangleGeometry(mask), GeometryCombineMode.Exclude, null);

            result.Children.Add(new GeometryDrawing(resultBrush, null, resultGeometry));

            return result;
        }

        private Rect GetInversBounds(Rect bounds) {
            var transform = new ScaleTransform(bounds.X == 0 ? -1 : 1, bounds.Y == 0 ? -1: 1, this.containerSize.Width / 2, this.containerSize.Height / 2);

            return transform.TransformBounds(bounds);
        }


        private Vector GetStartPosition(Vector indent, RulerStretch otherStretch, double translate) {
            switch (this.StartPosition) {
                default:
                case StartPosition.OnCenter:
                    return new Vector(this.containerSize.Width / 2, this.containerSize.Height / 2);
                case StartPosition.OnStart:
                    return RulersByArea && AreaDesigner is IAreaDesigner areaStart ?
                        new Vector(areaStart.Bounds.Left - translate, areaStart.Bounds.Top - translate)
                        : new Vector(indent.X, indent.X);
                case StartPosition.OnEnd:
                    double spec = 0;
                    switch (otherStretch) {
                        case RulerStretch.OnlyOnEnd:
                            spec = indent.Y;
                            break;
                        case RulerStretch.Duplicate:
                            spec = indent.Y;
                            break;
                    }

                    return RulersByArea && AreaDesigner is IAreaDesigner areaEnd ?
                        new Vector(areaEnd.Bounds.Right - translate, areaEnd.Bounds.Bottom - translate)
                        : new Vector(this.containerSize.Width - spec, this.containerSize.Height - spec);
            }
        }

        private Vector GetIndentOfRuller(bool otherVisibility, RulerSetting otherRuler, RulerStretch otherStretch, RulerStretch currentStretch) {
            if (otherVisibility && otherRuler is RulerSetting setting) {
                double start = 0;
                double end = 0;

                switch (otherStretch) {
                    case RulerStretch.OnlyOnStart:
                        start = setting.TickHeight;
                        end = setting.TickHeight;
                        break;
                    case RulerStretch.OnlyOnEnd:
                        start = 0.0000001; // needs for system
                        end = setting.TickHeight;
                        break;
                    case RulerStretch.Duplicate:
                        start = setting.TickHeight;
                        end = setting.TickHeight * 2;
                        break;
                }

                return new Vector(start, end);
            } else return default(Vector);
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
        #endregion

        protected override Freezable CreateInstanceCore() {
            return new RulersDesigner();
        }
    }
}
