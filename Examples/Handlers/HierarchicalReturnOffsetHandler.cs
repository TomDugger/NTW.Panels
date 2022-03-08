using Examples.Locators;
using NTW.Panels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;

namespace Examples.Handlers {
    public class HierarchicalReturnOffsetHandler : MouseHandler {

        private UIElement selected;



        public object SelectedContext {
            get { return (object)GetValue(SelectedContextProperty); }
            set { SetValue(SelectedContextProperty, value); }
        }

        public static readonly DependencyProperty SelectedContextProperty =
            DependencyProperty.Register("SelectedContext", typeof(object), typeof(HierarchicalReturnOffsetHandler), new PropertyMetadata(null));



        #region MouseHandler
        public override bool CanDownExecute(MouseEventArgs args, UIElementCollection elements) {
            bool result = args.OriginalSource.GetType().Name != "TextBoxView" && args.LeftButton == MouseButtonState.Pressed && elements.Count > 1;

            return result && selected == null;
        }

        public override bool CanMoveExecute(MouseEventArgs args, UIElementCollection elements) {

            return selected != null;
        }

        public override bool CanUpExecution(MouseEventArgs args, UIElementCollection elements) {
            return selected != null;
        }

        public override void DownExecute(UIElementCollection elements, Point mousePosition, IItemsLocator locator, Size place, Vector offset, out bool stopExecution) {

            stopExecution = false;

            selected = null;

            if (elements.Cast<UIElement>().FirstOrDefault(x => x.IsMouseOver) is UIElement ui) {
                selected = ui;
                Mouse.Capture(selected);

                if (selected is FrameworkElement fe)
                    SelectedContext = fe.DataContext;
            }
        }

        public override void MoveExecution(UIElementCollection elements, Point mousePosition, IItemsLocator locator, Size place, Vector offset, out bool stopExecution) {

            var size = locator.CalculateOffset(place, offset, selected, false);
            var bounds = locator.GetOriginalBounds(selected, offset);

            var result = mousePosition.X / place.Width;

            HierarchicalLocator.SetOffset(selected, result);

            stopExecution = true;

            Console.WriteLine(" - > vt");
        }

        public override void UpExecution(UIElementCollection elements, Point mousePosition, IItemsLocator locator, Size place, Vector offset, out bool stopExecution) {

            stopExecution = false;

            Mouse.Capture(null);
            selected = null;
        }
        #endregion

        protected override Freezable CreateInstanceCore() {
            return new HierarchicalReturnOffsetHandler();
        }
    }
}
