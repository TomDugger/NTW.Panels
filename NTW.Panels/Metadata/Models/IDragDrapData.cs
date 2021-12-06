using System.Windows;

namespace NTW.Panels {
    /// <summary>
    /// Drag and Drop interface
    /// </summary>
    public interface IDragDrapData {

        /// <summary>
        /// Data object
        /// </summary>
        IDataObject Data { get; }
        /// <summary>
        /// Keyboard states
        /// </summary>
        DragDropKeyStates KeyStates { get; }
        /// <summary>
        /// Allowed effects
        /// </summary>
        DragDropEffects AllowedEffects { get; }
        /// <summary>
        /// Current effects
        /// </summary>
        DragDropEffects Effects { get; }

        /// <summary>
        /// Mouse position
        /// </summary>
        Point MousePosition { get; }
    }
}
