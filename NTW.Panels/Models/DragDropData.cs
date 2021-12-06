using System.Windows;

namespace NTW.Panels {
    /// <summary>
    /// Struct with Drag & Drop Data
    /// </summary>
    internal struct DragDropData: IDragDrapData {

        /// <summary>
        /// Drag and drop constructor
        /// </summary>
        /// <param name="data">Data object</param>
        /// <param name="keyStates">Keyboard states</param>
        /// <param name="allowedEffects">Allowed effects</param>
        /// <param name="effects">Current effects</param>
        /// <param name="mousePosition">Mouse position</param>
        public DragDropData(IDataObject data, DragDropKeyStates keyStates, DragDropEffects allowedEffects, DragDropEffects effects, Point mousePosition) {
            this.Data = data;
            this.KeyStates = keyStates;
            this.AllowedEffects = allowedEffects;
            this.Effects = effects;
            this.MousePosition = mousePosition;
        }

        /// <summary>
        /// Drag and drop constructor
        /// </summary>
        /// <param name="args">Drag and Drop event args</param>
        /// <param name="mousePosition">Mouse position</param>
        public DragDropData(DragEventArgs args, Point mousePosition) {
            this.Data = args.Data;
            this.KeyStates = args.KeyStates;
            this.AllowedEffects = args.AllowedEffects;
            this.Effects = args.Effects;
            this.MousePosition = mousePosition;
        }

        /// <summary>
        /// Data object
        /// </summary>
        public IDataObject Data { get; }
        /// <summary>
        /// Keyboard states
        /// </summary>
        public DragDropKeyStates KeyStates { get; }
        /// <summary>
        /// Allowed effects
        /// </summary>
        public DragDropEffects AllowedEffects { get; }
        /// <summary>
        /// Current effects
        /// </summary>
        public DragDropEffects Effects { get; }

        /// <summary>
        /// Mouse position
        /// </summary>
        public Point MousePosition { get; }
    }
}
