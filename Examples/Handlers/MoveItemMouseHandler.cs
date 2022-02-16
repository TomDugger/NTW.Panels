using NTW.Panels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media.Effects;

namespace Examples.Handlers {
    public class MoveItemMouseHandler : MouseHandler, IMouseLeaveHandler, IKeyHandler {

        private Effect storeMoveElementEffet = null;
        private double storeMoveElementOpacity = 1;

        private UIElement moveElement;
        private UIElement placeElement;

        #region Properties
        public Action<object, object> MoveUp {
            get { return (Action<object, object>)GetValue(MoveUpProperty); }
            set { SetValue(MoveUpProperty, value); }
        }

        public static readonly DependencyProperty MoveUpProperty =
            DependencyProperty.Register("MoveUp", typeof(Action<object, object>), typeof(MoveItemMouseHandler), new PropertyMetadata(null));


        public Effect MoveEffect {
            get { return (Effect)GetValue(MoveEffectProperty); }
            set { SetValue(MoveEffectProperty, value); }
        }

        public static readonly DependencyProperty MoveEffectProperty =
            DependencyProperty.Register("MoveEffect", typeof(Effect), typeof(MoveItemMouseHandler), new PropertyMetadata(null));


        public bool StopOnMouseLeave {
            get { return (bool)GetValue(StopOnMouseLeaveProperty); }
            set { SetValue(StopOnMouseLeaveProperty, value); }
        }

        public static readonly DependencyProperty StopOnMouseLeaveProperty =
            DependencyProperty.Register("StopOnMouseLeave", typeof(bool), typeof(MoveItemMouseHandler), new PropertyMetadata(false));
        #endregion

        #region MouseHandler
        public override bool CanDownExecute(MouseEventArgs args, UIElementCollection elements) {
            return args.LeftButton == MouseButtonState.Pressed;
        }

        public override bool CanMoveExecute(MouseEventArgs args, UIElementCollection elements) {
            return args.LeftButton == MouseButtonState.Pressed && moveElement != null;
        }

        public override bool CanUpExecution(MouseEventArgs args, UIElementCollection elements) {
            return moveElement != null;
        }


        public override void DownExecute(UIElementCollection elements, Point mousePosition, IItemsLocator locator, Size place, Vector offset, out bool stopExecution) {

            stopExecution = false;

            if (Mouse.RightButton == MouseButtonState.Pressed) {
                StopExecution(elements, locator, place, offset);
                return;
            }

            moveElement = elements.Cast<UIElement>().FirstOrDefault(x => x.IsMouseOver);
            if (moveElement != null) {
                storeMoveElementEffet = this.moveElement.Effect;
                moveElement.Effect = this.MoveEffect;

                storeMoveElementOpacity = moveElement.Opacity;
                moveElement.Opacity = 0.5;

                Panel.SetZIndex(moveElement, 1);
            }
        }

        public override void MoveExecution(UIElementCollection elements, Point mousePosition, IItemsLocator locator, Size place, Vector offset, out bool stopExecution) {

            stopExecution = false;

            moveElement.Arrange(new Rect(new Point(mousePosition.X - moveElement.RenderSize.Width / 2, mousePosition.Y - moveElement.RenderSize.Height / 2), moveElement.RenderSize));

            moveElement.Visibility = Visibility.Visible;

            UIElement next = elements.Cast<UIElement>().FirstOrDefault(x => {
                return x != moveElement && x.Visibility == Visibility.Visible && locator.GetOriginalBounds(x, offset).Contains(mousePosition);
            });

            if (next != null)
                placeElement = next;

            if (placeElement == null)
                return;

            int indexMove = elements.IndexOf(moveElement);
            int indexPlace = elements.IndexOf(placeElement);

            int modifer = -1;
            if (indexPlace < indexMove) modifer = 1;

            var withoutMovelement = elements.Cast<UIElement>().Where(x => x != moveElement);

            // set correct position for all elements without moveElement
            locator.Arrange(place, offset, default(Vector), out Size fVerifySize, false, withoutMovelement.ToArray());

            // array of elements between moveElement and placeElement
            var skip = Math.Min(indexMove, indexPlace);
            var take = Math.Max(indexMove, indexPlace) - skip;
            var placeArray = elements.Cast<UIElement>().Skip(skip).Take(take).Where(x => x != moveElement && x != placeElement).ToArray();
            var moveOffset = locator.CalculateOffset(place, offset, moveElement, true);

            locator.Arrange(place, offset, moveOffset * modifer, out Size verifySize, false, placeArray);

            // set coorect position for placeElement
            if (locator.GetOriginalBounds(placeElement, offset).Contains(mousePosition))
                locator.Arrange(place, offset, moveOffset * modifer, out verifySize, false, placeElement);
            else {
                locator.Arrange(place, offset, default(Vector), out verifySize, false, placeElement);
                placeElement = null;
            }
        }

        public override void UpExecution(UIElementCollection elements, Point mousePosition, IItemsLocator locator, Size place, Vector offset, out bool stopExecution) {

            stopExecution = false;
            MoveUp?.Invoke(((FrameworkElement)moveElement)?.DataContext, ((FrameworkElement)placeElement)?.DataContext);
            StopExecution(elements, locator, place, offset);
        }
        #endregion

        #region IMouseLeaveHandler
        public bool CanLeaveExecute(MouseEventArgs args, UIElementCollection elements) {
            return StopOnMouseLeave && args.LeftButton == MouseButtonState.Pressed && moveElement != null;
        }

        public void LeaveExecute(UIElementCollection elements, Point mousePosition, IItemsLocator locator, Size place, Vector offset, out bool stopExecution) {
            stopExecution = true;
            StopExecution(elements, locator, place, offset);
        }
        #endregion

        #region IkeyHandler
        public bool CanKeyDown(KeyEventArgs args, UIElementCollection elements) => false;

        public void KeyDownExecution(UIElementCollection elements, Key Key, IItemsLocator locator, Size place, Vector offset, out bool stopExecution) { stopExecution = false; }

        public bool CanKeyUp(KeyEventArgs args, UIElementCollection elements) => Mouse.LeftButton == MouseButtonState.Pressed && moveElement != null;

        public void KeyUpExecution(UIElementCollection elements, Key Key, IItemsLocator locator, Size place, Vector offset, out bool stopExecution) {

            stopExecution = false;

            if (Key == Key.Escape)
                StopExecution(elements, locator, place, offset);
        }
        #endregion

        private void StopExecution(UIElementCollection elements, IItemsLocator locator, Size place, Vector offset) {
            Panel.SetZIndex(moveElement, 0);

            moveElement.Effect = storeMoveElementEffet;
            moveElement.Opacity = storeMoveElementOpacity;

            storeMoveElementEffet = null;
            storeMoveElementOpacity = 1;

            locator.Arrange(place, offset, default(Vector), out Size fVerifySize, false, elements.Cast<UIElement>().ToArray());

            moveElement = null;
            placeElement = null;
        }

        protected override Freezable CreateInstanceCore() {
            return new MoveItemMouseHandler();
        }
    }
}
