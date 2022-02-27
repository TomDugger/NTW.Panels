using System.Windows;

namespace NTW.Panels {
    /// <summary>
    /// Simple custom locator
    /// </summary>
    public abstract class CustomLocator : CustomObject, IItemsLocator {

        #region IItemsLocator
        public abstract Size Measure(Size originalSize, params UIElement[] elements);

        public abstract Size Arrange(Size originalSize, Vector offset, Vector itemsOffset, out Size verifySize, bool checkSize = false, params UIElement[] elements);


        public abstract Rect GetOriginalBounds(UIElement element, Vector offset = default);

        public abstract Vector CalculateOffset(Size originalSize, Vector offset, UIElement element, bool asNext, params UIElement[] elements);
        #endregion
    }
}
