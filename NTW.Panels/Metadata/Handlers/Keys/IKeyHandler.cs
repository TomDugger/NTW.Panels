using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;

namespace NTW.Panels {
    /// <summary>
    /// Keyboard handler interface
    /// </summary>
    public interface IKeyHandler {

        /// <summary>
        /// Allow to check the system can execute KeyDownExecution method.
        /// </summary>
        /// <param name="args">key event args</param>
        /// <param name="elements">Elements</param>
        /// <returns>Result value. True - can execute, false - cannot execute</returns>
        bool CanKeyDown(KeyEventArgs args, UIElementCollection elements);

        /// <summary>
        /// Allow to do something during key down event
        /// </summary>
        /// <param name="elements">Elements</param>
        /// <param name="Key">used key</param>
        /// <param name="locator">Panel locator</param>
        /// <param name="place">Original size</param>
        /// <param name="offset">Scroll offset</param>
        /// <param name="stopExecution">Stop pointer. If needs to check next handlers after this, needs to set true</param>
        void KeyDownExecution(UIElementCollection elements, Key Key, IItemsLocator locator, Size place, Vector offset, out bool stopExecution);

        /// <summary>
        /// Allow to check the system can execute KeyUpExecution method.
        /// </summary>
        /// <param name="args">key event args</param>
        /// <param name="elements">Elements</param>
        /// <returns>Result value. True - can execute, false - cannot execute</returns>
        bool CanKeyUp(KeyEventArgs args, UIElementCollection elements);

        /// <summary>
        /// Allow to do something during key up event
        /// </summary>
        /// <param name="elements">Elements</param>
        /// <param name="Key">used key</param>
        /// <param name="locator">Panel locator</param>
        /// <param name="place">Original size</param>
        /// <param name="offset">Scroll offset</param>
        /// <param name="stopExecution">Stop pointer. If needs to check next handlers after this, needs to set true</param>
        void KeyUpExecution(UIElementCollection elements, Key Key, IItemsLocator locator, Size place, Vector offset, out bool stopExecution);
    }
}
