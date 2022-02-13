using System.Windows;
using System.Windows.Media;

namespace NTW.Panels {
    /// <summary>
    /// Simple designer interface. Allow to calculate specific position
    /// </summary>
    public interface ICalculatePositionDesigner: IDesigner {

        /// <summary>
        /// Allow to calculate original point value to global point value
        /// </summary>
        /// <param name="value">Point value</param>
        /// <param name="center">Center of container</param>
        /// <param name="global">Global transformation</param>
        /// <returns>Global point value</returns>
        Point ToGlobal(Point value, Point center, Transform global = null);

        /// <summary>
        /// Allow to calculate global point value to original point value
        /// </summary>
        /// <param name="position">Position value</param>
        /// <param name="center">Center of container</param>
        /// <param name="global">Global transformation</param>
        /// <returns>Original point value</returns>
        Point FromGlobal(Point position, Point center, Transform global = null);

        /// <summary>
        /// Allow to calculate center of container
        /// </summary>
        /// <param name="containerSize">Container size</param>
        /// <param name="global">Global transformation</param>
        /// <returns>Calculated center value</returns>
        Point Center(Size containerSize, Transform global = null);
    }
}
