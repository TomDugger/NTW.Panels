using System;
using System.Collections.Generic;

namespace NTW.Panels {
    /// <summary>
    /// Standart Design interface. Allow to use Designers in own logic
    /// </summary>
    public interface IDesign {

        DesignersCollection Designers { get; set; }

        T GetDesigner<T>() where T : IDesigner;

        IEnumerable<T> GetDesigners<T>() where T : IDesigner;

        void Execute<T>(IEnumerable<T> designers, Action<T> action) where T: IDesigner;

        void ExecuteFor<T>(Action<T> action) where T : IDesigner;
    }
}
