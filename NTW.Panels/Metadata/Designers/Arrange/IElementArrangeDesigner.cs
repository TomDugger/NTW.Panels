using System.Windows;
using System.Windows.Media;

namespace NTW.Panels {
    /// <summary>
    /// Simple designer interface. Allow to define logic for calculate something when the Element has arrange value
    /// </summary>
    public interface IElementArrangeDesigner: IDesigner {

        /// <summary>
        /// Special method, witch pointsfor the element has arrange
        /// </summary>
        /// <param name="elementRect">Rect of element</param>
        /// <param name="containerSize">Original container size</param>
        /// <param name="element">UI element</param>
        void AfterElementArrange(Rect elementRect, Size containerSize, UIElement element, Transform global = null);
    }
}
