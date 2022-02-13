using System.Windows.Media;

namespace NTW.Panels {
    /// <summary>
    /// Simple transform designer inerface
    /// </summary>
    public interface ITransformDesigner: IDesigner {

        /// <summary>
        /// Allow to get Transformation
        /// </summary>
        /// <returns></returns>
        Transform GetTransform();
    }
}
