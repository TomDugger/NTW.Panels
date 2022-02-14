using Example.Locators;
using NTW.Panels;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;

namespace Example.Handlers {
    public class ShadowSelectingHandler : MouseHandler {

        public IChildrenContainer ChildrenContainer {
            get { return (IChildrenContainer)GetValue(ChildrenContainerProperty); }
            set { SetValue(ChildrenContainerProperty, value); }
        }

        public static readonly DependencyProperty ChildrenContainerProperty =
            DependencyProperty.Register("ChildrenContainer", typeof(IChildrenContainer), typeof(ShadowSelectingHandler), new PropertyMetadata(null));


        public override bool CanDownExecute(MouseEventArgs args, UIElementCollection elements) {
            return false;
        }

        public override bool CanMoveExecute(MouseEventArgs args, UIElementCollection elements) {
            return false;
        }

        public override bool CanUpExecution(MouseEventArgs args, UIElementCollection elements) {
            return true;
        }

        public override void DownExecute(UIElementCollection elements, Point mousePosition, IItemsLocator locator, Size place, Vector offset, out bool stopExecution) {
            stopExecution = false;
        }

        public override void MoveExecution(UIElementCollection elements, Point mousePosition, IItemsLocator locator, Size place, Vector offset, out bool stopExecution) {
            stopExecution = false;
        }

        public override void UpExecution(UIElementCollection elements, Point mousePosition, IItemsLocator locator, Size place, Vector offset, out bool stopExecution) {
            stopExecution = false;

            if (locator is ChartLocator chart 
                && ChildrenContainer is IChildrenContainer container && container.GetChild(mousePosition) is UIElement ui) {
                chart.SetMoveToChildPosition(ChartLocator.GetPosition(ui));
            }
        }

        protected override Freezable CreateInstanceCore() {
            return new ShadowSelectingHandler();
        }
    }
}
