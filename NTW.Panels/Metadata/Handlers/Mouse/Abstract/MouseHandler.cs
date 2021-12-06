using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;

namespace NTW.Panels {
    /// <summary>
    /// Simple abstract mouse handler
    /// </summary>
    public abstract class MouseHandler : CustomHandler
        , IMouseDownHandler, IMouseMoveHandler, IMouseUpHandler {

        #region IMouseDawnHandler
        /// <summary>
        /// Allow to check the system can execute DownExecute method.
        /// </summary>
        /// <param name="args">Mouse down event args</param>
        /// <param name="elements">Elements</param>
        /// <returns>Result value. True - can execute, false - cannot execute</returns>
        public abstract bool CanDownExecute(MouseEventArgs args, UIElementCollection elements);

        /// <summary>
        /// Allow to do something during mouse down event
        /// </summary>
        /// <param name="elements">Elements</param>
        /// <param name="mousePosition">Mouse position</param>
        /// <param name="locator">Panel locator</param>
        /// <param name="place">Original size</param>
        /// <param name="offset">Scroll offset</param>
        /// <param name="stopExecution">Stop pointer. If needs to check next handlers after this, needs to set true</param>
        public abstract void DownExecute(UIElementCollection elements, Point mousePosition, IItemsLocator locator, Size place, Vector offset, out bool stopExecution);
        #endregion

        #region IMouseMoveHandler
        /// <summary>
        /// Allow to check the system can execute MoveExecution method.
        /// </summary>
        /// <param name="args">Mouse move event args</param>
        /// <param name="elements">Elements</param>
        /// <returns>Result value. True - can execute, false - cannot execute</returns>
        public abstract bool CanMoveExecute(MouseEventArgs args, UIElementCollection elements);

        /// <summary>
        /// Allow to do something during move event
        /// </summary>
        /// <param name="elements">Elements</param>
        /// <param name="mousePosition">Mouse position</param>
        /// <param name="locator">Panel locator</param>
        /// <param name="place">Original size</param>
        /// <param name="offset">Scroll offset</param>
        /// <param name="stopExecution">Stop pointer. If needs to check next handlers after this, needs to set true</param>
        public abstract void MoveExecution(UIElementCollection elements, Point mousePosition, IItemsLocator locator, Size place, Vector offset, out bool stopExecution);
        #endregion

        #region IMouseUpHandler
        /// <summary>
        /// Allow to check the system can execute UpExecution method.
        /// </summary>
        /// <param name="args">Mouse up event args</param>
        /// <param name="elements">Elements</param>
        /// <returns>Result value. True - can execute, false - cannot execute</returns>
        public abstract bool CanUpExecution(MouseEventArgs args, UIElementCollection elements);

        /// <summary>
        /// Allow to do something during mouse up event
        /// </summary>
        /// <param name="elements">Elements</param>
        /// <param name="mousePosition">Mouse position</param>
        /// <param name="locator">Panel locator</param>
        /// <param name="place">Original size</param>
        /// <param name="offset">Scroll offset</param>
        /// <param name="stopExecution">Stop pointer. If needs to check next handlers after this, needs to set true</param>
        public abstract void UpExecution(UIElementCollection elements, Point mousePosition, IItemsLocator locator, Size place, Vector offset, out bool stopExecution);
        #endregion
    }
}
