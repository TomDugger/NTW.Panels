using NTW.Panels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows;
using System.Windows.Media;

namespace Example.Locators {
    public class ChartLocator : Freezable, IItemsLocator, IDrawingPresenter, IDesign {

        private Point center;
        private Size size;

        private TransformGroup transform;

        public ChartLocator() {
            this.Designers = new DesignersCollection();

            transform = new TransformGroup();

            transform.Children.Add(this.Designers.Transformation);
            backDrawing.Children.Add(this.Designers.BackDrawing);
            frontDrawing.Children.Add(this.Designers.FrontDrawing);
        }

        #region Attached properties
        public static Point GetPosition(DependencyObject obj) {
            return (Point)obj.GetValue(PositionProperty);
        }

        public static void SetPosition(DependencyObject obj, Point value) {
            obj.SetValue(PositionProperty, value);
        }

        public static readonly DependencyProperty PositionProperty =
            DependencyProperty.RegisterAttached("Position", typeof(Point), typeof(ChartLocator), new FrameworkPropertyMetadata(default(Point), FrameworkPropertyMetadataOptions.AffectsParentArrange));


        private static bool GetIsChartItem(DependencyObject obj) {
            return (bool)obj.GetValue(IsChartItemProperty);
        }

        private static void SetIsChartItem(DependencyObject obj, bool value) {
            obj.SetValue(IsChartItemProperty, value);
        }

        private static readonly DependencyProperty IsChartItemProperty =
            DependencyProperty.RegisterAttached("IsChartItem", typeof(bool), typeof(ChartLocator), new PropertyMetadata(false));
        #endregion

        #region IDrawingPresenter
        private DrawingGroup backDrawing = new DrawingGroup();
        public Drawing BackDrawing => backDrawing;

        private DrawingGroup frontDrawing = new DrawingGroup();
        public Drawing FrontDrawing => frontDrawing;
        #endregion

        #region IItemsLocator
        public Size Measure(Size originalSize, params UIElement[] elements) {

            foreach (UIElement child in elements) {
                child.Measure(originalSize);
            }

            return originalSize;
        }

        public Size Arrange(Size originalSize, Vector offset, Vector itemsOffset, out Size verifySize, bool checkSize = false, params UIElement[] elements) {

            verifySize = default(Size); // ignore ScrollView
            center = CalculateCenter(originalSize);
            size = originalSize;

            // begin of Element Arrange Designers
            ExecuteFor<IArrangeDesigner>(designer => designer.BeginElementArrange(originalSize, this.transform));

            foreach (UIElement child in elements) {
                Point position = ToGlobal(GetPosition(child));

                Point childPos = new Point(position.X - child.DesiredSize.Width / 2, position.Y - child.DesiredSize.Height / 2);

                Rect childRect = new Rect(childPos, child.DesiredSize);

                child.Arrange(childRect);
                SetIsChartItem(child, true);

                // elementArrange designers
                ExecuteFor<IElementArrangeDesigner>(designer => designer.AfterElementArrange(childRect, originalSize, child, this.transform));
            }

            // end of Element Arrange Designers
            ExecuteFor<IArrangeDesigner>(designer => designer.EndElementArrange(originalSize, this.transform));

            // set senter of scaling
            ExecuteFor<IScaleTransformDesigner>(scale => scale.SetScaleCenter(originalSize.Width / 2, originalSize.Height / 2));

            return originalSize;
        }

        public Vector CalculateOffset(Size originalSize, Vector offset, UIElement element, bool asNext, params UIElement[] elements) {
            return default(Vector);
        }

        public Rect GetOriginalBounds(UIElement element, Vector offset = default) {
            return default(Rect);
        }
        #endregion

        #region IDesign
        public DesignersCollection Designers {
            get { return (DesignersCollection)GetValue(DesignersProperty); }
            set { SetValue(DesignersProperty, value); }
        }

        public static readonly DependencyProperty DesignersProperty =
            DependencyProperty.Register("Designers", typeof(DesignersCollection), typeof(ChartLocator), new PropertyMetadata(null));

        public T GetDesigner<T>()
            where T : IDesigner {
            return (T)Designers.Where(x => x is T).Cast<IDesigner>().FirstOrDefault();
        }

        public IEnumerable<T> GetDesigners<T>() 
            where T : IDesigner {
            return Designers.Where(x => x is T).Cast<T>();
        }

        public void Execute<T>(IEnumerable<T> designers, Action<T> action) 
            where T : IDesigner {

            if (designers == null || action == null) return;

            foreach (T designer in designers)
                action(designer);
        }

        public void ExecuteFor<T>(Action<T> action) 
            where T : IDesigner {

            if (action == null) return;

            foreach (T designer in GetDesigners<T>())
                action(designer);
        }
        #endregion

        #region Helps
        public Point ToGlobal(Point position) {
            Point result = default(Point);

            if (GetDesigner<ICalculatePositionDesigner>() is ICalculatePositionDesigner designer) {
                result = designer.ToGlobal(position, center, transform);
            } else
                result = transform.Transform(new Point(center.X + position.X, center.Y + position.Y));

            return result;
        }

        public Point FromGlobal(Point position) {
            Point result = default(Point);

            if (GetDesigner<ICalculatePositionDesigner>() is ICalculatePositionDesigner designer)
                result = designer.FromGlobal(position, center, transform);
            else
                result = new Point(result.X - center.X, result.Y - center.Y);

            return result;
        }

        public void SetMoveToChildPosition(Point childPosition) {

            ExecuteFor<ITranslateTransformDesigner>(translate => translate.SetTranslation(default, true));

            Point position = ToGlobal(childPosition);

            Point renderCenter = new Point(size.Width / 2, size.Height / 2);

            // translate to move position
            var result = renderCenter - position;

            ExecuteFor<ITranslateTransformDesigner>(translate => translate.SetTranslation(result, true));
        }

        private Point CalculateCenter(Size originalSize) {
            Point result = default(Point);
            if (GetDesigner<ICalculatePositionDesigner>() is ICalculatePositionDesigner designer)
                result = designer.Center(originalSize, transform);
            else
                result = new Point(originalSize.Width / 2, originalSize.Height / 2);

            return result;
        }
        #endregion

        protected override Freezable CreateInstanceCore() {
            return new ChartLocator();
        }
    }
}
