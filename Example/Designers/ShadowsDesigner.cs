using NTW.Panels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows;
using System.Windows.Media;

namespace Example.Designers {
    public class ShadowsDesigner : CustomDesigner, IElementArrangeDesigner, IArrangeDesigner, IDrawingPresenter {

        private DrawingGroup shadowsDrawing;
        private readonly Geometry defaultShadowItemGeometry;
        private readonly Dictionary<UIElement, Rect> shadowsPositions;

        public ShadowsDesigner() {
            shadowsPositions = new Dictionary<UIElement, Rect>();

            defaultShadowItemGeometry = new EllipseGeometry(new Rect(0, 0, 12, 12));

            shadowsDrawing = new DrawingGroup();
            frontDrawing.Children.Add(shadowsDrawing);
        }

        #region Properties
        public bool UseShadows {
            get { return (bool)GetValue(UseShadowsProperty); }
            set { SetValue(UseShadowsProperty, value); }
        }

        public static readonly DependencyProperty UseShadowsProperty =
            DependencyProperty.Register("UseShadows", typeof(bool), typeof(ShadowsDesigner), new FrameworkPropertyMetadata(false, FrameworkPropertyMetadataOptions.AffectsParentArrange));


        public Geometry ShadowItemGeometry {
            get { return (Geometry)GetValue(ShadowItemGeometryProperty); }
            set { SetValue(ShadowItemGeometryProperty, value); }
        }

        public static readonly DependencyProperty ShadowItemGeometryProperty =
            DependencyProperty.Register("ShadowItemGeometry", typeof(Geometry), typeof(ShadowsDesigner), new PropertyMetadata(null));


        public Brush ShadowFill {
            get { return (Brush)GetValue(ShadowFillProperty); }
            set { SetValue(ShadowFillProperty, value); }
        }
        public static readonly DependencyProperty ShadowFillProperty =
            DependencyProperty.Register("ShadowFill", typeof(Brush), typeof(ShadowsDesigner), new PropertyMetadata(Brushes.Red));


        public Brush ShadowStroke {
            get { return (Brush)GetValue(ShadowStrokeProperty); }
            set { SetValue(ShadowStrokeProperty, value); }
        }

        public static readonly DependencyProperty ShadowStrokeProperty =
            DependencyProperty.Register("ShadowStroke", typeof(Brush), typeof(ShadowsDesigner), new PropertyMetadata(null));


        public double ShadowStrokeThickness {
            get { return (double)GetValue(ShadowStrokeThicknessProperty); }
            set { SetValue(ShadowStrokeThicknessProperty, value); }
        }

        public static readonly DependencyProperty ShadowStrokeThicknessProperty =
            DependencyProperty.Register("ShadowStrokeThickness", typeof(double), typeof(ShadowsDesigner), new PropertyMetadata(0.0));
        #endregion

        #region IElementArrangeDesigner
        public void AfterElementArrange(Rect elementRect, Size containerSize, UIElement element, Transform global = null) {
            if (UseShadows)
                BuildShadowOfElement(containerSize, elementRect, element);
        }
        #endregion

        #region IArrangeDesigner
        public void BeginElementArrange(Size containerSize, Transform global = null) {
            ClearShadowsOfElements();
            frontDrawing.ClipGeometry = new RectangleGeometry(new Rect(containerSize));
        }

        public void EndElementArrange(Size containerSize, Transform global = null) {
            
        }
        #endregion

        #region IDrawingPresenter
        public Drawing BackDrawing { get; }

        private DrawingGroup frontDrawing = new DrawingGroup();
        public Drawing FrontDrawing => frontDrawing;
        #endregion

        #region Helps
        private void ClearShadowsOfElements() {
            shadowsDrawing.Children.Clear();
            shadowsPositions.Clear();
        }

        private void BuildShadowOfElement(Size originalSize, Rect arrange, UIElement element) {

            if (!arrange.IntersectsWith(new Rect(originalSize)) & originalSize != default) {

                Geometry shadowItemGeometry = (this.ShadowItemGeometry ?? defaultShadowItemGeometry).Clone();

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

                    shadowsDrawing.Children.Add(new GeometryDrawing(ShadowFill, new Pen(ShadowStroke, ShadowStrokeThickness), shadowItemGeometry));
                    shadowsPositions.Add(element, transform.TransformBounds(geoRect));
                }
            }
        }

        public UIElement GetChild(Point mousePosition) {

            var shadow = shadowsPositions.FirstOrDefault(x => x.Value.Contains(mousePosition));
            
            if (shadow.Key != null) 
                return shadow.Key;
             else
                return null;
        }
        #endregion

        protected override Freezable CreateInstanceCore() {
            return new ShadowsDesigner();
        }
    }
}
