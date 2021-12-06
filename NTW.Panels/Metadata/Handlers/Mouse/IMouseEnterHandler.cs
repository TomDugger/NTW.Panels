using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;

namespace NTW.Panels {
    /// <summary>
    /// Mouse enter handler interface
    /// </summary>
    public interface IMouseEnterHandler: ICustomHandler {

        /// <summary>
        /// Allow to check the system can execute EnterExecute method.
        /// </summary>
        /// <param name="args">Mouse enter event args<</param>
        /// <param name="elements">Elements</param>
        /// <returns>Result value. True - can execute, false - cannot execute</returns>
        bool CanEnterExecute(MouseEventArgs args, UIElementCollection elements);

        /// <summary>
        /// Allow to do something during mouse enter event
        /// </summary>
        /// <param name="elements">Elements</param>
        /// <param name="mousePosition">Mouse position</param>
        /// <param name="locator">Panel locator</param>
        /// <param name="place">Original size</param>
        /// <param name="offset">Scroll offset</param>
        /// <param name="stopExecution">Stop pointer. If needs to check next handlers after this, needs to set true</param>
        void EnterExecute(UIElementCollection elements, Point mousePosition, IItemsLocator locator, Size place, Vector offset, out bool stopExecution);
    }
}
