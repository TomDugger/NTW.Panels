using System.Windows;

namespace NTW.Panels {
    /// <summary>
    /// Simple Common interface. Allow to define logic for working with children
    /// </summary>
    public interface IChildrenContainer {

        /// <summary>
        /// Allow to get child by the point
        /// </summary>
        /// <param name="position">Point value</param>
        /// <returns>UI element</returns>
        UIElement GetChild(Point position);
    }
}
