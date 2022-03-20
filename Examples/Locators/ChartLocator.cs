using NTW.Panels;
using System;
using System.Windows;
using System.Windows.Media;

namespace Examples.Locators {
    public class ChartLocator : DesignedLocator, IDrawingPresenter {

        private Point center;
        private Size size;

        private TransformGroup transform;

        public ChartLocator():base() {

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
            DependencyProperty.RegisterAttached("Position", typeof(Point), typeof(ChartLocator), new PropertyMetadata(default(Point), PositionChanged));

        private static void PositionChanged(DependencyObject sender, DependencyPropertyChangedEventArgs e) {
            if (sender is UIElement ui && GetRebuildArrangeChild(sender) is Action<UIElement> rebuild)
                rebuild(ui);
        }
        #endregion

        #region IDrawingPresenter
        private DrawingGroup backDrawing = new DrawingGroup();
        public Drawing BackDrawing => backDrawing;

        private DrawingGroup frontDrawing = new DrawingGroup();
        public Drawing FrontDrawing => frontDrawing;
        #endregion

        #region IItemsLocator
        public override Size Measure(Size originalSize, params UIElement[] elements) {

            foreach (UIElement child in elements) {
                child.Measure(originalSize);
            }

            return originalSize;
        }

        public override Size Arrange(Size originalSize, Vector offset, Vector itemsOffset, out Size verifySize, bool checkSize = false, params UIElement[] elements) {

            verifySize = default(Size); // ignore ScrollView
            center = CalculateCenter(originalSize);
            size = originalSize;

            // begin of Element Arrange Designers
            ExecuteFor<IArrangeDesigner>(designer => designer.BeginElementArrange(originalSize, this.transform));

            int index = -1;
            foreach (UIElement child in elements) {

                SetChildIndex(child, ++index);

                ArrangeChild(child);

                SetRebuildArrangeChild(child, ArrageChildWithCallingElementDesigners);
            }

            // end of Element Arrange Designers
            ExecuteFor<IArrangeDesigner>(designer => designer.EndElementArrange(originalSize, this.transform));

            // set senter of scaling
            ExecuteFor<IScaleTransformDesigner>(scale => scale.SetScaleCenter(originalSize.Width / 2, originalSize.Height / 2));

            return originalSize;
        }

        public override Vector CalculateOffset(Size originalSize, Vector offset, UIElement element, bool asNext, params UIElement[] elements) {
            return default(Vector);
        }

        public override Rect GetOriginalBounds(UIElement element, Vector offset = default) {
            return default(Rect);
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
                result = new Point(position.X - center.X, position.Y - center.Y);

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

        private void ArrangeChild(UIElement child) {
            Point position = ToGlobal(GetPosition(child));

            Point childPos = new Point(position.X - child.DesiredSize.Width / 2, position.Y - child.DesiredSize.Height / 2);

            Rect childRect = new Rect(childPos, child.DesiredSize);

            child.Arrange(childRect);

            // elementArrange designers (setting)
            ExecuteFor<IElementArrangeDesigner>(designer => designer.AfterElementArrange(childRect, size, GetChildIndex(child), child, this.transform));
        }

        private void ArrageChildWithCallingElementDesigners(UIElement child) {

            Point position = ToGlobal(GetPosition(child));

            Point childPos = new Point(position.X - child.DesiredSize.Width / 2, position.Y - child.DesiredSize.Height / 2);

            Rect childRect = new Rect(childPos, child.DesiredSize);

            child.Arrange(childRect);

            // elementArrange designers (Updating)
            ExecuteFor<IElementArrangeDesigner>(designer => designer.UpdateElementArrage(childRect, size, GetChildIndex(child), child, this.transform));
        }
        #endregion

        protected override Freezable CreateInstanceCore() {
            return new ChartLocator();
        }
    }
}
