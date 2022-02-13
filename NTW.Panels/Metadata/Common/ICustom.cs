namespace NTW.Panels {
    /// <summary>
    /// Standart custom interface
    /// </summary>
    public interface ICustom {

        /// <summary>
        /// Allow get the state
        /// </summary>
        bool IsActive { get; }

        /// <summary>
        /// Allow set the state
        /// </summary>
        /// <param name="isActive">New state</param>
        void SetState(bool isActive);
    }
}
