using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;

namespace NTW.Panels {
    /// <summary>
    /// Mouse wheel handler interface
    /// </summary>
    public interface IMouseWheelHandler: ICustomHandler {

        /// <summary>
        /// Allow to check the system can execute WheelExecution method.
        /// </summary>
        /// <param name="args">Mouse wheel event args</param>
        /// <param name="elements">Elements</param>
        /// <returns>Result value. True - can execute, false - cannot execute</returns>
        bool CanWheelExecution(MouseWheelEventArgs args, UIElementCollection elements);

        /// <summary>
        /// Allow to do something during wheel event
        /// </summary>
        /// <param name="elements">Elements</param>
        /// <param name="mousePosition">Mouse position</param>
        /// <param name="delta">Scroll wheel delta</param>
        /// <param name="locator">Panel locator</param>
        /// <param name="place">Original size</param>
        /// <param name="offset">Scroll offset</param>
        /// <param name="stopExecution">Stop pointer. If needs to check next handlers after this, needs to set true</param>
        void WheelExecution(UIElementCollection elements, Point mousePosition, double delta, IItemsLocator locator, Size place, Vector offset, out bool stopExecution);
    }
}
