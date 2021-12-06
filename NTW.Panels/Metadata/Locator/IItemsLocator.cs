using System.Windows;

namespace NTW.Panels {

    /// <summary>
    /// Special Interface for define items locator
    /// </summary>
    public interface IItemsLocator {

        /// <summary>
        /// Try to define the size of panel
        /// </summary>
        /// <param name="originalSize">Original size</param>
        /// <param name="elements">UI elements</param>
        /// <returns>Result size</returns>
        Size Measure(Size originalSize, params UIElement[] elements);

        /// <summary>
        /// Try to set arrange for UI elements
        /// </summary>
        /// <param name="originalSize">Original size of the panel</param>
        /// <param name="offset">ScrollView offset</param>
        /// <param name="itemsOffset">Special ofsset for elements</param>
        /// <param name="verifySize">Result size for verify size in ScrollView</param>
        /// <param name="checkSize">True - always returne from custom Arrange method. You can set other value in handlers</param>
        /// <param name="elements">UI elements</param>
        /// <returns>Result size</returns>
        Size Arrange(Size originalSize, Vector offset, Vector itemsOffset, out Size verifySize, bool checkSize = false, params UIElement[] elements);

        /// <summary>
        /// Allow to calculate rect for element
        /// </summary>
        /// <param name="originalSize">Original size of the panel</param>
        /// <param name="offset">ScrollView offset</param>
        /// <param name="element">Defined UI elements</param>
        /// <param name="asNext">Speacial pointer for define needs to add something to rect rect</param>
        /// <param name="elements">UI elements</param>
        /// <returns>Result size</returns>
        Vector CalculateOffset(Size originalSize, Vector offset, UIElement element, bool asNext, params UIElement[] elements);

        /// <summary>
        /// Allow to get original rect for element
        /// </summary>
        /// <param name="element">UIe element</param>
        /// <param name="offset">Global offset</param>
        /// <returns>Result rect</returns>
        Rect GetOriginalBounds(UIElement element, Vector offset = default(Vector));
    }
}
