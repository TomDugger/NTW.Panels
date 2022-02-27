using System;

namespace NTW.Panels {

    public delegate void OptionCallingHandler(CustomObject sender, UpdateOptions option);

    public interface INotifyOption {

        event OptionCallingHandler OptionCalling;
    }
}
