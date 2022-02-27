using NTW.Panels;
using System.Collections.Generic;
using System.Linq;
using System.Windows;
using System.Windows.Media;

namespace Examples.Designers {
    public class ShadowsDesigner : CustomDesigner
        , IElementArrangeDesigner
        , IArrangeDesigner
        , IDrawingPresenter
        , IChildrenContainer {

        private DrawingGroup shadowsDrawing;
        private readonly Geometry defaultShadowItemGeometry;
        private readonly Dictionary<UIElement, Rect> shadowsPositions;
        private readonly Dictionary<UIElement, GeometryDrawing> shadowsDrawings;

        public ShadowsDesigner() {
            shadowsPositions = new Dictionary<UIElement, Rect>();
            shadowsDrawings = new Dictionary<UIElement, GeometryDrawing>();

            defaultShadowItemGeometry = new EllipseGeometry(new Rect(0, 0, 12, 12));

            shadowsDrawing = new DrawingGroup();
        }

        #region Properties
        public bool UseShadows {
            get { return (bool)GetValue(UseShadowsProperty); }
            set { SetValue(UseShadowsProperty, value); }
        }

        public static readonly DependencyProperty UseShadowsProperty =
            DependencyProperty.Register("UseShadows", typeof(bool), typeof(ShadowsDesigner), new PropertyMetadata(false, UseShadowsChanged));

        private static void UseShadowsChanged(DependencyObject sender, DependencyPropertyChangedEventArgs e) {
            if (sender is ShadowsDesigner designer)
                if (e.NewValue is bool visibility)
                    if (visibility)
                        designer.frontDrawing.Children.Add(designer.shadowsDrawing);
                    else
                        designer.frontDrawing.Children.Remove(designer.shadowsDrawing);
        }


        public Geometry ShadowItemGeometry {
            get { return (Geometry)GetValue(ShadowItemGeometryProperty); }
            set { SetValue(ShadowItemGeometryProperty, value); }
        }

        public static readonly DependencyProperty ShadowItemGeometryProperty =
            DependencyProperty.Register("ShadowItemGeometry", typeof(Geometry), typeof(ShadowsDesigner), new PropertyMetadata(null, ShadowItemGeometryChanged));

        private static void ShadowItemGeometryChanged(DependencyObject sender, DependencyPropertyChangedEventArgs e) {
            if (sender is ShadowsDesigner designer)
                designer.UpdateShadowsGeometry();
        }


        public Brush ShadowFill {
            get { return (Brush)GetValue(ShadowFillProperty); }
            set { SetValue(ShadowFillProperty, value); }
        }
        public static readonly DependencyProperty ShadowFillProperty =
            DependencyProperty.Register("ShadowFill", typeof(Brush), typeof(ShadowsDesigner), new PropertyMetadata(Brushes.Red, ShadowFillChanged));

        private static void ShadowFillChanged(DependencyObject sender, DependencyPropertyChangedEventArgs e) {
            if (sender is ShadowsDesigner designer)
                designer.UpdateShadowsFill();
        }

        public Brush ShadowStroke {
            get { return (Brush)GetValue(ShadowStrokeProperty); }
            set { SetValue(ShadowStrokeProperty, value); }
        }

        public static readonly DependencyProperty ShadowStrokeProperty =
            DependencyProperty.Register("ShadowStroke", typeof(Brush), typeof(ShadowsDesigner), new PropertyMetadata(null, ShadowStrokeChanged));

        private static void ShadowStrokeChanged(DependencyObject sender, DependencyPropertyChangedEventArgs e) {
            if (sender is ShadowsDesigner designer)
                designer.UpdateShadowsStroke();
        }


        public double ShadowStrokeThickness {
            get { return (double)GetValue(ShadowStrokeThicknessProperty); }
            set { SetValue(ShadowStrokeThicknessProperty, value); }
        }

        public static readonly DependencyProperty ShadowStrokeThicknessProperty =
            DependencyProperty.Register("ShadowStrokeThickness", typeof(double), typeof(ShadowsDesigner), new PropertyMetadata(0.0, ShadowStrokeThicknessChanged));

        private static void ShadowStrokeThicknessChanged(DependencyObject sender, DependencyPropertyChangedEventArgs e) {
            if (sender is ShadowsDesigner designer)
                designer.UpdateShadowsStrokeThickness();
        }

        #endregion

        #region Attached properties
        private static ShadowsDesigner GetParentShadowDesigner(DependencyObject obj) {
            return (ShadowsDesigner)obj.GetValue(ParentShadowDesignerProperty);
        }

        private static void SetParentShadowDesigner(DependencyObject obj, ShadowsDesigner value) {
            obj.SetValue(ParentShadowDesignerProperty, value);
        }

        private static readonly DependencyProperty ParentShadowDesignerProperty =
            DependencyProperty.RegisterAttached("ParentShadowDesigner", typeof(ShadowsDesigner), typeof(ShadowsDesigner), new PropertyMetadata(null));


        public static Geometry GetItemGeometry(DependencyObject obj) {
            return (Geometry)obj.GetValue(ItemGeometryProperty);
        }

        public static void SetItemGeometry(DependencyObject obj, Geometry value) {
            obj.SetValue(ItemGeometryProperty, value);
        }

        public static readonly DependencyProperty ItemGeometryProperty =
            DependencyProperty.RegisterAttached("ItemGeometry", typeof(Geometry), typeof(ShadowsDesigner), new PropertyMetadata(null, ItemGeometryChanged));

        private static void ItemGeometryChanged(DependencyObject sender, DependencyPropertyChangedEventArgs e) {
            if (GetParentShadowDesigner(sender) is ShadowsDesigner designer)
                designer.UpdateShadowsGeometry(sender as UIElement);
        }


        public static Brush GetFill(DependencyObject obj) {
            return (Brush)obj.GetValue(FillProperty);
        }

        public static void SetFill(DependencyObject obj, Brush value) {
            obj.SetValue(FillProperty, value);
        }

        public static readonly DependencyProperty FillProperty =
            DependencyProperty.RegisterAttached("Fill", typeof(Brush), typeof(ShadowsDesigner), new PropertyMetadata(null, FillChanged));

        private static void FillChanged(DependencyObject sender, DependencyPropertyChangedEventArgs e) {
            if (GetParentShadowDesigner(sender) is ShadowsDesigner designer)
                designer.UpdateShadowsFill(sender as UIElement);
        }


        public static Brush GetStroke(DependencyObject obj) {
            return (Brush)obj.GetValue(StrokeProperty);
        }

        public static void SetStroke(DependencyObject obj, Brush value) {
            obj.SetValue(StrokeProperty, value);
        }

        public static readonly DependencyProperty StrokeProperty =
            DependencyProperty.RegisterAttached("Stroke", typeof(Brush), typeof(ShadowsDesigner), new PropertyMetadata(null, StrokeChanged));

        private static void StrokeChanged(DependencyObject sender, DependencyPropertyChangedEventArgs e) {
            if (GetParentShadowDesigner(sender) is ShadowsDesigner designer)
                designer.UpdateShadowsStroke(sender as UIElement);
        }


        public static double? GetStrokeThickness(DependencyObject obj) {
            return (double?)obj.GetValue(StrokeThicknessProperty);
        }

        public static void SetStrokeThickness(DependencyObject obj, double? value) {
            obj.SetValue(StrokeThicknessProperty, value);
        }

        public static readonly DependencyProperty StrokeThicknessProperty =
            DependencyProperty.RegisterAttached("StrokeThickness", typeof(double?), typeof(ShadowsDesigner), new PropertyMetadata(null, StrokeThicknessChanged));

        private static void StrokeThicknessChanged(DependencyObject sender, DependencyPropertyChangedEventArgs e) {
            if (GetParentShadowDesigner(sender) is ShadowsDesigner designer)
                designer.UpdateShadowsStrokeThickness(sender as UIElement);
        }
        #endregion

        #region IElementArrangeDesigner
        public void AfterElementArrange(Rect elementRect, Size containerSize, UIElement element, Transform global = null) {
            BuildShadowOfElement(containerSize, elementRect, element);
        }

        public void UpdateElementArrage(Rect elementRect, Size containerSize, int index, UIElement element, Transform global = null) { }
        #endregion

        #region IArrangeDesigner
        public void BeginElementArrange(Size containerSize, Transform global = null) {
            ClearShadowsOfElements();
            frontDrawing.ClipGeometry = new RectangleGeometry(new Rect(containerSize));
        }

        public void EndElementArrange(Size containerSize, Transform global = null) { }
        #endregion

        #region IDrawingPresenter
        public Drawing BackDrawing { get; }

        private DrawingGroup frontDrawing = new DrawingGroup();
        public Drawing FrontDrawing => frontDrawing;
        #endregion

        #region IChildrenContainer
        public UIElement GetChild(Point position) {

            var shadow = shadowsPositions.FirstOrDefault(x => x.Value.Contains(position));

            if (shadow.Key != null)
                return shadow.Key;
            else
                return null;
        }
        #endregion

        #region Helps
        private void ClearShadowsOfElements() {
            // clear ShadowsDesigner for all elements in the collections
            foreach (var it in shadowsPositions.Keys)
                SetParentShadowDesigner(it, null);

            // clear other collections and dictionaries
            shadowsDrawing.Children.Clear();
            shadowsPositions.Clear();
            shadowsDrawings.Clear();
        }

        private void BuildShadowOfElement(Size originalSize, Rect arrange, UIElement element) {

            if (!arrange.IntersectsWith(new Rect(originalSize)) & originalSize != default) {

                Geometry shadowItemGeometry = (GetItemGeometry(element) ?? this.ShadowItemGeometry ?? defaultShadowItemGeometry).Clone();

                Transform transform = Transform.Identity;

                var geoRect = shadowItemGeometry.Bounds;

                Point centerGeometry = new Point(geoRect.Width / 2, geoRect.Height / 2);

                bool onLeft = arrange.Right < 0;
                bool onRight = arrange.Left > originalSize.Width;
                bool onTop = arrange.Bottom < 0;
                bool onBottom = arrange.Top > originalSize.Height;

                // corners
                if (onLeft & onTop) transform = new TranslateTransform(-centerGeometry.X, -centerGeometry.Y);
                else if (onRight & onTop) transform = new TranslateTransform(originalSize.Width - centerGeometry.X, -centerGeometry.Y);
                else if (onLeft & onBottom) transform = new TranslateTransform(-centerGeometry.X, originalSize.Height - centerGeometry.Y);
                else if (onRight & onBottom) transform = new TranslateTransform(originalSize.Width - centerGeometry.X, originalSize.Height - centerGeometry.Y);
                // lines
                else if (onTop) transform = new TranslateTransform(arrange.X, -centerGeometry.Y);
                else if (onBottom) transform = new TranslateTransform(arrange.X, originalSize.Height - centerGeometry.Y);
                else if (onLeft) transform = new TranslateTransform(-centerGeometry.X, arrange.Y);
                else if (onRight) transform = new TranslateTransform(originalSize.Width - centerGeometry.X, arrange.Y);

                if (transform != Transform.Identity) {
                    shadowItemGeometry.Transform = transform;

                    shadowsDrawings[element] = new GeometryDrawing(GetFill(element) ?? ShadowFill, new Pen(GetStroke(element) ?? ShadowStroke, GetStrokeThickness(element) ?? ShadowStrokeThickness), shadowItemGeometry);
                    shadowsDrawing.Children.Add(shadowsDrawings[element]);
                    shadowsPositions[element] = transform.TransformBounds(geoRect);
                    SetParentShadowDesigner(element, this);
                }
            }
        }

        private void UpdateShadowsGeometry(UIElement element = null) {

            if (element != null) {
                if (shadowsDrawings.ContainsKey(element)) {
                    Geometry shadowItemGeometry = (GetItemGeometry(element) ?? this.ShadowItemGeometry ?? defaultShadowItemGeometry).Clone();

                    shadowsDrawings[element].Geometry = shadowItemGeometry;
                }
            } else
                foreach (var it in shadowsDrawings) {
                    Geometry shadowItemGeometry = (GetItemGeometry(it.Key) ?? this.ShadowItemGeometry ?? defaultShadowItemGeometry).Clone();

                    it.Value.Geometry = shadowItemGeometry;
                }
        }

        private void UpdateShadowsFill(UIElement element = null) {
            if (element != null) {
                if (shadowsDrawings.ContainsKey(element))
                    shadowsDrawings[element].Brush = GetFill(element) ?? ShadowFill;
            } else
                foreach (var it in shadowsDrawings) 
                    it.Value.Brush = GetFill(it.Key) ?? ShadowFill;
        }

        private void UpdateShadowsStroke(UIElement element = null) {
            if (element != null) {
                if (shadowsDrawings.ContainsKey(element))
                    shadowsDrawings[element].Pen.Brush = GetStroke(element) ?? ShadowStroke;
            } else
                foreach (var it in shadowsDrawings)
                    it.Value.Pen.Brush = GetStroke(it.Key) ?? ShadowStroke;
        }

        private void UpdateShadowsStrokeThickness(UIElement element = null) {
            if (element != null) {
                if (shadowsDrawings.ContainsKey(element))
                    shadowsDrawings[element].Pen.Thickness = GetStrokeThickness(element) ?? ShadowStrokeThickness;
            } else
                foreach (var it in shadowsDrawings)
                    it.Value.Pen.Thickness = GetStrokeThickness(it.Key) ?? ShadowStrokeThickness;
        }
        #endregion

        protected override Freezable CreateInstanceCore() {
            return new ShadowsDesigner();
        }
    }
}
