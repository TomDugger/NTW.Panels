
namespace NTW.Panels {
    /// <summary>
    /// Standart handler interface
    /// </summary>
    public interface ICustomHandler {

        /// <summary>
        /// Allow get the state of handler
        /// </summary>
        bool IsActive { get; }

        /// <summary>
        /// Allow set the state of handler
        /// </summary>
        /// <param name="isActive">New state</param>
        void SetState(bool isActive);
    }
}
