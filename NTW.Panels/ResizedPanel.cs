using System;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Interop;

namespace NTW.Panels {
    /// <summary>
    /// Base panel with resize Implementation of base window 
    /// </summary>
    public abstract class ResizedPanel: Panel {

        /// <summary>
        /// last window parameter
        /// </summary>
        private IntPtr lastWParam;

        private const int WmEnterSizeMove = 0x0231;
        private const int WmExitSizeMove = 0x232;

        private IntPtr Sc_Move = new IntPtr(0xF012);

        /// <summary>
        /// Allow to get the state of resizing
        /// </summary>
        protected bool IsResizingFinished = true;


        public ResizedPanel(): base() {
            this.Loaded += ResizedPanelLoaded;
        }

        private void ResizedPanelLoaded(object sender, RoutedEventArgs e) {
            BeginToCheckResizingChanged();
            this.Loaded -= ResizedPanelLoaded;
            this.Unloaded += ResizedPanelUnloaded;
        }

        private void ResizedPanelUnloaded(object sender, RoutedEventArgs e) {
            StopToCheckResizingChanged();
            this.Unloaded -= ResizedPanelUnloaded;
        }

        /// <summary>
        /// Allow to begin chacking the resize state
        /// </summary>
        private void BeginToCheckResizingChanged() {
            if (Application.Current.MainWindow == null) return;

            var helper = new WindowInteropHelper(Application.Current.MainWindow);
            if (helper.Handle != null) {
                var source = HwndSource.FromHwnd(helper.Handle);
                if (source != null)
                    source.AddHook(HwndMessageHook);
            }
        }

        /// <summary>
        /// Allow to stop chacking the rsize state
        /// </summary>
        private void StopToCheckResizingChanged() {
            if (Application.Current.MainWindow == null) return;

            var helper = new WindowInteropHelper(Application.Current.MainWindow);
            if (helper.Handle != null) {
                var source = HwndSource.FromHwnd(helper.Handle);
                if (source != null)
                    source.RemoveHook(HwndMessageHook);
            }
        }


        private IntPtr HwndMessageHook(IntPtr wnd, int msg, IntPtr wParam, IntPtr lParam, ref bool handled) {

            if (wParam != IntPtr.Zero)
                lastWParam = wParam;

            switch (msg) {
                case WmEnterSizeMove: {

                        if (lastWParam != Sc_Move) {

                            var lastState = IsResizingFinished;
                            IsResizingFinished = false;

                            if (lastState)
                                ResizingStart();
                        }
                        break;
                    }
                case WmExitSizeMove: {

                        lastWParam = IntPtr.Zero;
                        var lastState = IsResizingFinished;
                        IsResizingFinished = true;

                        if (!lastState)
                            ResizingFinished();
                        break;
                    }
            }

            return IntPtr.Zero;
        }

        /// <summary>
        /// Special virtual method witch execution of resizing size on start
        /// </summary>
        protected virtual void ResizingStart() { }

        /// <summary>
        /// Special virtual method witch execution of resizing size on finish
        /// </summary>
        protected virtual void ResizingFinished() { }
    }
}
