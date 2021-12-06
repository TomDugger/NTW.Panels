using System.Windows.Media;

namespace NTW.Panels {
    /// <summary>
    /// Drawing presenter handler interface
    /// </summary>
    public interface IDrawingPresenter {

        /// <summary>
        /// Background drawing
        /// </summary>
        Drawing BackDrawing { get; }

        /// <summary>
        /// Front drawing
        /// </summary>
        Drawing FrontDrawing { get; }
    }
}
