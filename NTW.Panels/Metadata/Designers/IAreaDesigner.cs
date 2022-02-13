using System.Windows;

namespace NTW.Panels {
    /// <summary>
    /// Standart designer interface. Allow to define area value
    /// </summary>
    public interface IAreaDesigner: IDesigner {

        /// <summary>
        /// Allow get/set Area value
        /// </summary>
        Size Area { get; set; }

        /// <summary>
        /// Allow to get/set special child area
        /// </summary>
        Size ChildArea { get; set; }
    }
}
