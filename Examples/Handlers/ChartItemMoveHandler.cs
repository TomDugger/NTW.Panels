using Examples.Locators;
using NTW.Panels;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;

namespace Examples.Handlers {
    public class ChartItemMoveHandler : MouseHandler {

        private UIElement moveElement;

        #region AttachedProperty
        public static MouseMoveType GetMoveType(DependencyObject obj) {
            return (MouseMoveType)obj.GetValue(MoveTypeProperty);
        }

        public static void SetMoveType(DependencyObject obj, MouseMoveType value) {
            obj.SetValue(MoveTypeProperty, value);
        }

        public static readonly DependencyProperty MoveTypeProperty =
            DependencyProperty.RegisterAttached("MoveType", typeof(MouseMoveType), typeof(ChartItemMoveHandler), new PropertyMetadata(MouseMoveType.XY));
        #endregion

        public override bool CanDownExecute(MouseEventArgs args, UIElementCollection elements) {
            return Mouse.LeftButton == MouseButtonState.Pressed;
        }

        public override bool CanMoveExecute(MouseEventArgs args, UIElementCollection elements) {
            return Mouse.LeftButton == MouseButtonState.Pressed && moveElement != null;
        }

        public override bool CanUpExecution(MouseEventArgs args, UIElementCollection elements) {
            return moveElement != null;
        }


        public override void DownExecute(UIElementCollection elements, Point mousePosition, IItemsLocator locator, Size place, Vector offset, out bool stopExecution) {

            stopExecution = false;

            moveElement = elements.Cast<UIElement>().FirstOrDefault(x => x.IsMouseOver);
            if (moveElement != null) {
                Mouse.Capture(moveElement);
                Panel.SetZIndex(moveElement, 1);
            }

        }

        public override void MoveExecution(UIElementCollection elements, Point mousePosition, IItemsLocator locator, Size place, Vector offset, out bool stopExecution) {
            stopExecution = false;
            if (locator is ChartLocator chart) {

                Point pos = chart.ToGlobal(ChartLocator.GetPosition(moveElement));
                switch (GetMoveType(moveElement)) {
                    case MouseMoveType.XY:
                        pos = mousePosition;
                        break;
                    case MouseMoveType.X:
                        pos = new Point(mousePosition.X, pos.Y);
                        break;
                    case MouseMoveType.Y:
                        pos = new Point(pos.X, mousePosition.Y);
                        break;
                }

                ChartLocator.SetPosition(moveElement, chart.FromGlobal(pos));
            }
        }

        public override void UpExecution(UIElementCollection elements, Point mousePosition, IItemsLocator locator, Size place, Vector offset, out bool stopExecution) {
            stopExecution = false;
            if (moveElement != null) {
                Panel.SetZIndex(moveElement, 0);
                Mouse.Capture(null);
                moveElement = null;
            }
        }

        protected override Freezable CreateInstanceCore() {
            return new ChartItemMoveHandler();
        }
    }

    public enum MouseMoveType {
        XY,
        X,
        Y
    }
}
