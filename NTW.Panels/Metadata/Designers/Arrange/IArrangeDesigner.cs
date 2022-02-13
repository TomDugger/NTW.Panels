using System.Windows;
using System.Windows.Media;

namespace NTW.Panels {
    /// <summary>
    /// Simple designer interface. Allow to define logic when container begin/end Arrange for Elements
    /// </summary>
    public interface IArrangeDesigner: IDesigner {

        /// <summary>
        /// Special method, witch points for begining the logic
        /// </summary>
        /// <param name="containerSize">Original container size</param>
        void BeginElementArrange(Size containerSize, Transform global = null);

        /// <summary>
        /// Special method, witch points for ending the logic
        /// </summary>
        /// <param name="containerSize">Original container size</param>
        void EndElementArrange(Size containerSize, Transform global = null);
    }
}
