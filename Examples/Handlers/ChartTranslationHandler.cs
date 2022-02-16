using Examples.Designers;
using NTW.Panels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;

namespace Examples.Handlers {
    public class ChartTranslationHandler : MouseHandler, IMouseWheelHandler {

        private Point startTranslatePosition = default(Point);

        public override bool CanDownExecute(MouseEventArgs args, UIElementCollection elements) {
            return Mouse.MiddleButton == MouseButtonState.Pressed;
        }

        public override bool CanMoveExecute(MouseEventArgs args, UIElementCollection elements) {
            return Mouse.MiddleButton == MouseButtonState.Pressed;
        }

        public override bool CanUpExecution(MouseEventArgs args, UIElementCollection elements) {
            return startTranslatePosition != default(Point);
        }


        public override void DownExecute(UIElementCollection elements, Point mousePosition, IItemsLocator locator, Size place, Vector offset, out bool stopExecution) {
            if (Mouse.MiddleButton == MouseButtonState.Pressed) {
                stopExecution = true;
                startTranslatePosition = mousePosition;
            } else
                stopExecution = false;
        }

        public override void MoveExecution(UIElementCollection elements, Point mousePosition, IItemsLocator locator, Size place, Vector offset, out bool stopExecution) {

            stopExecution = false;

            if (locator is IDesign design) {

                if (startTranslatePosition == default(Point))
                    startTranslatePosition = mousePosition;

                var deferent = startTranslatePosition - mousePosition;

                design.ExecuteFor<CanvasOffsetDesigner>(control => control.SetTranslation(deferent));

                startTranslatePosition = mousePosition;
            }
        }

        public override void UpExecution(UIElementCollection elements, Point mousePosition, IItemsLocator locator, Size place, Vector offset, out bool stopExecution) {
            stopExecution = false;
            startTranslatePosition = default(Point);
        }


        public bool CanWheelExecution(MouseWheelEventArgs args, UIElementCollection elements) {
            return true;
        }

        public void WheelExecution(UIElementCollection elements, Point mousePosition, double delta, IItemsLocator locator, Size place, Vector offset, out bool stopExecution) {
            stopExecution = false;
            if (locator is IDesign design) {
                double tick = delta / 120.0 / 10.0;

                design.ExecuteFor<ZoomDesigner>(control => control.SetScale(tick, tick));
            }

        }

        protected override Freezable CreateInstanceCore() {
            return new ChartTranslationHandler();
        }
    }
}
