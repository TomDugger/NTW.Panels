
namespace NTW.Panels {
    /// <summary>
    /// Simple scale transformation designer interface
    /// </summary>
    public interface IScaleTransformDesigner: ITransformDesigner {

        /// <summary>
        /// Allow to set/get value of Scale X
        /// </summary>
        double ScaleX { get; set; }

        /// <summary>
        /// Allow to set/get value of Scale Y
        /// </summary>
        double ScaleY { get; set; }

        /// <summary>
        /// Special method for set values of scale 
        /// </summary>
        /// <param name="x">Scale X</param>
        /// <param name="y">Scale Y</param>
        void SetScale(double x, double y);

        /// <summary>
        /// Special method for set center of scale
        /// </summary>
        /// <param name="x"></param>
        /// <param name="y"></param>
        void SetScaleCenter(double x, double y);
    }
}
