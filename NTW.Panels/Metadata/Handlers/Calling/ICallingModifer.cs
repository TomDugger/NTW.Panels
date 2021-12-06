using System;
namespace NTW.Panels {
    /// <summary>
    /// Event handler. Used to call the parent panel and do something with it 
    /// </summary>
    /// <param name="action"></param>
    public delegate void PanelCallingHandler(Action<CustomPanel> action);

    /// <summary>
    /// Calling modifer the panel handler interface
    /// </summary>
    public interface ICallingModifer {

        /// <summary>
        /// Panel calling event
        /// </summary>
        event PanelCallingHandler PanelCalling;
    }
}
