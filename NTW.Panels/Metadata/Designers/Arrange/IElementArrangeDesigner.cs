using System.Windows;
using System.Windows.Media;

namespace NTW.Panels {
    /// <summary>
    /// Simple designer interface. Allow to define logic for calculate something when the Element has arrange value
    /// </summary>
    public interface IElementArrangeDesigner: IDesigner {

        /// <summary>
        /// Special method, wetch points for the element. The element has arrange
        /// </summary>
        /// <param name="elementRect">Rect of element</param>
        /// <param name="containerSize">Original container size</param>
        /// <param name="element">UI element</param>
        void AfterElementArrange(Rect elementRect, Size containerSize, UIElement element, Transform global = null);

        /// <summary>
        /// Special method, wetch point and index of the element. The element has arrage
        /// </summary>
        /// <param name="elementRect"></param>
        /// <param name="containerSize"></param>
        /// <param name="index"></param>
        /// <param name="element"></param>
        /// <param name="global"></param>
        void UpdateElementArrage(Rect elementRect, Size containerSize, int index, UIElement element, Transform global = null);
    }
}
