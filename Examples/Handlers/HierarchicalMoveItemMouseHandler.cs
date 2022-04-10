using NTW.Panels;
using System;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;

namespace Examples.Handlers {
    public class HierarchicalMoveItemMouseHandler : MouseHandler, IDrawingPresenter {

        private static UIElement hierarchicalMoveElement;
        private static Vector moveOffset = default(Vector);
        private static DrawingGroup lastSelectingDrawing = null;
        private static DrawingGroup lastCurrentDrawing = null;

        private static UIElement placeElement;

        private static DateTime clickTime = default(DateTime);

        private static bool memberExpandedState;


        private DrawingGroup currentDrawing;
        private DrawingGroup selectingDrawing;

        public HierarchicalMoveItemMouseHandler(): base() {
            selectingDrawing = new DrawingGroup();
            currentDrawing = new DrawingGroup();

            frontDrawing.Children.Add(currentDrawing);
            frontDrawing.Children.Add(selectingDrawing);
        }

        #region Properties
        public Action<object, object, object, object> HierarchicalMoveUp {
            get { return (Action<object, object, object, object>)GetValue(HierarchicalMoveUpProperty); }
            set { SetValue(HierarchicalMoveUpProperty, value); }
        }

        public static readonly DependencyProperty HierarchicalMoveUpProperty =
            DependencyProperty.Register("HierarchicalMoveUp", typeof(Action<object, object, object, object>), typeof(HierarchicalMoveItemMouseHandler), new PropertyMetadata(null));

        #region Visual
        public Brush SelectedStroke {
            get { return (Brush)GetValue(SelectedStrokeProperty); }
            set { SetValue(SelectedStrokeProperty, value); }
        }

        public static readonly DependencyProperty SelectedStrokeProperty =
            DependencyProperty.Register("SelectedStroke", typeof(Brush), typeof(HierarchicalMoveItemMouseHandler), new PropertyMetadata(Brushes.Green));


        public double SelectedThickness {
            get { return (double)GetValue(SelectedThicknessProperty); }
            set { SetValue(SelectedThicknessProperty, value); }
        }

        public static readonly DependencyProperty SelectedThicknessProperty =
            DependencyProperty.Register("SelectedThickness", typeof(double), typeof(HierarchicalMoveItemMouseHandler), new PropertyMetadata(1.0));


        public Brush MouseOverStroke {
            get { return (Brush)GetValue(MouseOverStrokeProperty); }
            set { SetValue(MouseOverStrokeProperty, value); }
        }

        public static readonly DependencyProperty MouseOverStrokeProperty =
            DependencyProperty.Register("MouseOverStroke", typeof(Brush), typeof(HierarchicalMoveItemMouseHandler), new PropertyMetadata(Brushes.DodgerBlue));


        public double MouseOverThickness {
            get { return (double)GetValue(MouseOverThicknessProperty); }
            set { SetValue(MouseOverThicknessProperty, value); }
        }

        public static readonly DependencyProperty MouseOverThicknessProperty =
            DependencyProperty.Register("MouseOverThickness", typeof(double), typeof(HierarchicalMoveItemMouseHandler), new PropertyMetadata(1.0));
        #endregion
        #endregion

        #region IDrawingPresenter
        public Drawing BackDrawing { get; }

        private DrawingGroup frontDrawing = new DrawingGroup();
        public Drawing FrontDrawing => frontDrawing;
        #endregion

        #region MouseHandler
        public override bool CanDownExecute(MouseEventArgs args, UIElementCollection elements) {
            clickTime = DateTime.Now;
            return args.LeftButton == MouseButtonState.Pressed;
        }

        public override bool CanMoveExecute(MouseEventArgs args, UIElementCollection elements) {
            return args.LeftButton == MouseButtonState.Pressed 
                && clickTime != default(DateTime)
                && clickTime.AddMilliseconds(100) < DateTime.Now 
                && hierarchicalMoveElement != null;
        }

        public override bool CanUpExecution(MouseEventArgs args, UIElementCollection elements) {
            return true;
        }


        public override void DownExecute(UIElementCollection elements, Point mousePosition, IItemsLocator locator, Size place, Vector offset, out bool stopExecution) {
            stopExecution = true;

            hierarchicalMoveElement = null;

            if (Mouse.RightButton == MouseButtonState.Pressed) {
                StopExecution(elements, locator, place, offset);
                return;
            }

            hierarchicalMoveElement = elements.Cast<UIElement>().FirstOrDefault(x => x.IsMouseOver);
        }

        public override void MoveExecution(UIElementCollection elements, Point mousePosition, IItemsLocator locator, Size place, Vector offset, out bool stopExecution) {
            stopExecution = false;

            currentDrawing.Children.Clear();
            lastSelectingDrawing?.Children.Clear();

            if (elements.Contains(hierarchicalMoveElement)) {
                if (hierarchicalMoveElement is TreeViewItem tvi) {
                    memberExpandedState = tvi.IsExpanded;
                    if (tvi.IsExpanded)
                        tvi.IsExpanded = false;
                }

                lastCurrentDrawing?.Children.Clear();
                lastCurrentDrawing = DrawUnderLine(currentDrawing, locator.GetOriginalBounds(hierarchicalMoveElement), SelectedStroke, SelectedThickness);

                // set application cursor
                Mouse.OverrideCursor = Cursors.Hand;
            }

            #region highlight place element
            UIElement mouseOver = elements.Cast<UIElement>().FirstOrDefault(x => {
                return x != hierarchicalMoveElement && x.Visibility == Visibility.Visible && x.IsMouseOver;
            });

            if (mouseOver != null)
                placeElement = mouseOver;

            if (placeElement == null)
                return;

            Vector perOffset = default(Vector);
            if (elements.Contains(hierarchicalMoveElement)
                && elements.IndexOf(hierarchicalMoveElement) < elements.IndexOf(placeElement))
                perOffset = -moveOffset;

            var placeBounds = locator.GetOriginalBounds(placeElement, perOffset);
            if (placeBounds != default(Rect))
                lastSelectingDrawing = DrawUnderLine(selectingDrawing, placeBounds, MouseOverStroke, MouseOverThickness);
            #endregion
        }

        public override void UpExecution(UIElementCollection elements, Point mousePosition, IItemsLocator locator, Size place, Vector offset, out bool stopExecution) {

            stopExecution = false;

            currentDrawing.Children.Clear();
            selectingDrawing.Children.Clear();

            if (elements.Contains(placeElement)) {

                object[] result = null;

                if (hierarchicalMoveElement is FrameworkElement me && placeElement is FrameworkElement pe) {
                    var pme = (FrameworkElement)VisualTreeHelper.GetParent(me);
                    var ppe = (FrameworkElement)VisualTreeHelper.GetParent(pe);

                    result = new object[] { pme?.DataContext, me.DataContext, ppe?.DataContext, pe.DataContext };
                }

                Panel.SetZIndex(hierarchicalMoveElement, 0);
                if (hierarchicalMoveElement is TreeViewItem tvi)
                    tvi.IsExpanded = memberExpandedState;

                lastCurrentDrawing?.Children.Clear();
                lastCurrentDrawing = null;

                hierarchicalMoveElement = null;
                placeElement = null;

                // set application cursor
                Mouse.OverrideCursor = null;

                if (result != null)
                    HierarchicalMoveUp?.Invoke(result[0], result[1], result[2], result[3]);
            }

            StopExecution(elements, locator, place, offset);
        }
        #endregion

        #region Helps
        private void StopExecution(UIElementCollection elements, IItemsLocator locator, Size place, Vector offset) {

            locator.Arrange(place, offset, default(Vector), out Size fVerifySize, false, elements.Cast<UIElement>().ToArray());

            moveOffset = default(Vector);
            lastSelectingDrawing = null;
            clickTime = default(DateTime);
        }

        private DrawingGroup DrawUnderLine(DrawingGroup drawing, Rect rect, Brush brush, double thickness) {
            drawing.Children.Clear();

            drawing.Children.Add(new GeometryDrawing(null, new Pen(brush, thickness), new RectangleGeometry(rect)));

            return drawing;
        } 
        #endregion

        protected override Freezable CreateInstanceCore() {
            return new HierarchicalMoveItemMouseHandler();
        }
    }
}
