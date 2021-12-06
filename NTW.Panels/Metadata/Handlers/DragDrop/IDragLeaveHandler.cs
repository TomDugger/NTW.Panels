using System.Windows;
using System.Windows.Controls;

namespace NTW.Panels {
    /// <summary>
    /// Drag leave handler interface
    /// </summary>
    public interface IDragLeaveHandler : ICustomHandler {

        /// <summary>
        /// Allow to check the system can execute DragLeaveExecute method.
        /// </summary>
        /// <param name="args">Drag event args</param>
        /// <param name="elements">Elements</param>
        /// <returns>Result value. True - can execute, false - cannot execute</returns>
        bool CanDragLeaveExecute(DragEventArgs args, UIElementCollection elements);

        /// <summary>
        /// Allow to do something during Drag leave event
        /// </summary>
        /// <param name="elements">Elements</param>
        /// <param name="data">Data object</param>
        /// <param name="locator">Panel locator</param>
        /// <param name="place">Original size</param>
        /// <param name="offset">Scroll offset</param>
        /// <param name="stopExecution">Stop pointer. If needs to check next handlers after this, needs to set true</param>
        void DragLeaveExecute(UIElementCollection elements, IDragDrapData data, IItemsLocator locator, Size place, Vector offset, out bool stopExecution);
    }
}
