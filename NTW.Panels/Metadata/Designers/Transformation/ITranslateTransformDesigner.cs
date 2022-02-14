using System.Windows;

namespace NTW.Panels {
    /// <summary>
    /// Simple translate transformation designer interface
    /// </summary>
    public interface ITranslateTransformDesigner: ITransformDesigner {

        /// <summary>
        /// Allow to set/get value of Translate X
        /// </summary>
        double TranslateX { get; set; }

        /// <summary>
        /// Allow to set/get value of Translate Y
        /// </summary>
        double TranslateY { get; set; }

        /// <summary>
        /// Special method for set values of translate 
        /// </summary>
        /// <param name="deferent">Vector of translate</param>
        /// /// <param name="set">Set values</param>
        void SetTranslation(Vector deferent, bool set = false);
    }
}
