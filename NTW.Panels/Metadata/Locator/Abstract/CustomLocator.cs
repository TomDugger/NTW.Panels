using System;
using System.Windows;

namespace NTW.Panels {
    /// <summary>
    /// Simple custom locator
    /// </summary>
    public abstract class CustomLocator : CustomObject, IItemsLocator {

        #region Properties
        #region Properties
        protected static Action<UIElement> GetRebuildArrangeChild(DependencyObject obj) {
            return (Action<UIElement>)obj.GetValue(RebuildArrangeChildProperty);
        }

        protected static void SetRebuildArrangeChild(DependencyObject obj, Action<UIElement> value) {
            obj.SetValue(RebuildArrangeChildProperty, value);
        }

        private static readonly DependencyProperty RebuildArrangeChildProperty =
            DependencyProperty.RegisterAttached("RebuildArrangeChild", typeof(Action<UIElement>), typeof(CustomObject), new PropertyMetadata(null));


        protected static int GetChildIndex(DependencyObject obj) {
            return (int)obj.GetValue(ChildIndexProperty);
        }

        protected static void SetChildIndex(DependencyObject obj, int value) {
            obj.SetValue(ChildIndexProperty, value);
        }

        private static readonly DependencyProperty ChildIndexProperty =
            DependencyProperty.RegisterAttached("ChildIndex", typeof(int), typeof(CustomObject), new PropertyMetadata(-1));
        #endregion
        #endregion

        #region IItemsLocator
        public abstract Size Measure(Size originalSize, params UIElement[] elements);

        public abstract Size Arrange(Size originalSize, Vector offset, Vector itemsOffset, out Size verifySize, bool checkSize = false, params UIElement[] elements);


        public abstract Rect GetOriginalBounds(UIElement element, Vector offset = default);

        public abstract Vector CalculateOffset(Size originalSize, Vector offset, UIElement element, bool asNext, params UIElement[] elements);
        #endregion
    }
}
