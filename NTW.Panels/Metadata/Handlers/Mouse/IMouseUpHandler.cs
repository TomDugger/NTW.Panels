using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;

namespace NTW.Panels {
    /// <summary>
    /// Mouse up handler interface
    /// </summary>
    public interface IMouseUpHandler: ICustomHandler {

        /// <summary>
        /// Allow to check the system can execute UpExecution method.
        /// </summary>
        /// <param name="args">Mouse up event args</param>
        /// <param name="elements">Elements</param>
        /// <returns>Result value. True - can execute, false - cannot execute</returns>
        bool CanUpExecution(MouseEventArgs args, UIElementCollection elements);

        /// <summary>
        /// Allow to do something during mouse up event
        /// </summary>
        /// <param name="elements">Elements</param>
        /// <param name="mousePosition">Mouse position</param>
        /// <param name="locator">Panel locator</param>
        /// <param name="place">Original size</param>
        /// <param name="offset">Scroll offset</param>
        /// <param name="stopExecution">Stop pointer. If needs to check next handlers after this, needs to set true</param>
        void UpExecution(UIElementCollection elements, Point mousePosition, IItemsLocator locator, Size place, Vector offset, out bool stopExecution);
    }
}
