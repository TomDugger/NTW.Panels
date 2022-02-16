using NTW.Panels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;

namespace Examples.Handlers {
    public class SelectRangeMouseHandler : MouseHandler, IDrawingPresenter {

        private List<object> links = new List<object>();

        private Rect SelectingRect = Rect.Empty;

        #region Properties
        public bool Contains {
            get { return (bool)GetValue(ContainsProperty); }
            set { SetValue(ContainsProperty, value); }
        }

        public static readonly DependencyProperty ContainsProperty =
            DependencyProperty.Register("Contains", typeof(bool), typeof(SelectRangeMouseHandler), new PropertyMetadata(false));


        public Action<IEnumerable<object>> Selecting {
            get { return (Action<IEnumerable<object>>)GetValue(SelectingProperty); }
            set { SetValue(SelectingProperty, value); }
        }

        public static readonly DependencyProperty SelectingProperty =
            DependencyProperty.Register("Selecting", typeof(Action<IEnumerable<object>>), typeof(SelectRangeMouseHandler), new PropertyMetadata(null));


        public Action<IEnumerable<object>> Selected {
            get { return (Action<IEnumerable<object>>)GetValue(SelectedProperty); }
            set { SetValue(SelectedProperty, value); }
        }

        public static readonly DependencyProperty SelectedProperty =
            DependencyProperty.Register("Selected", typeof(Action<IEnumerable<object>>), typeof(SelectRangeMouseHandler), new PropertyMetadata(null));
        #endregion

        #region MouseHandler
        public override bool CanDownExecute(MouseEventArgs args, UIElementCollection elements) {
            return SelectingRect == Rect.Empty && (Keyboard.IsKeyDown(Key.LeftShift) || Keyboard.IsKeyDown(Key.RightShift));
        }

        public override bool CanMoveExecute(MouseEventArgs args, UIElementCollection elements) {
            return SelectingRect != Rect.Empty && (Keyboard.IsKeyDown(Key.LeftShift) || Keyboard.IsKeyDown(Key.RightShift));
        }

        public override bool CanUpExecution(MouseEventArgs args, UIElementCollection elements) {
            return SelectingRect != Rect.Empty && (Keyboard.IsKeyDown(Key.LeftShift) || Keyboard.IsKeyDown(Key.RightShift));
        }


        public override void DownExecute(UIElementCollection elements, Point mousePosition, IItemsLocator locator, Size place, Vector offset, out bool stopExecution) {
            stopExecution = true;

            SelectingRect = new Rect(mousePosition, new Size(1, 1));
            DrawRectangle(SelectingRect);

            links.Clear();
            // find all element witch bounds intersect with Selecting
            foreach (FrameworkElement element in elements)
                if (locator.GetOriginalBounds(element).IntersectsWith(SelectingRect))
                    links.Add(element.DataContext);

            this.Selecting?.Invoke(links.ToArray());
        }

        public override void MoveExecution(UIElementCollection elements, Point mousePosition, IItemsLocator locator, Size place, Vector offset, out bool stopExecution) {
            stopExecution = true;

            SelectingRect = GetRect(mousePosition, SelectingRect);
            DrawRectangle(SelectingRect);

            links.Clear();
            // find all element witch bounds intersect with Selecting
            foreach (FrameworkElement element in elements) {
                Rect bounds = locator.GetOriginalBounds(element);

                if (Contains) {
                    if (SelectingRect.Contains(bounds))
                        links.Add(element.DataContext);
                } else if (bounds.IntersectsWith(SelectingRect))
                    links.Add(element.DataContext);
            }

            this.Selecting?.Invoke(links.ToArray());
        }

        public override void UpExecution(UIElementCollection elements, Point mousePosition, IItemsLocator locator, Size place, Vector offset, out bool stopExecution) {
            stopExecution = true;

            this.Selected?.Invoke(links.ToArray());

            links.Clear();

            SelectingRect = Rect.Empty;
            DrawRectangle(SelectingRect);
        }
        #endregion

        #region IDrawingPresenter
        private DrawingGroup backDrawing = new DrawingGroup();
        public Drawing BackDrawing => backDrawing;

        private DrawingGroup frontDrawing = new DrawingGroup();
        public Drawing FrontDrawing => frontDrawing;
        #endregion

        #region Helps
        private Rect GetRect(Point p, Rect rect) {

            if (p.X > rect.X && p.Y > rect.Y)
                return new Rect(rect.TopLeft, p);

            else if (p.X > rect.X && p.Y < rect.BottomLeft.Y)
                return new Rect(rect.BottomLeft, p);

            else if (p.X < rect.TopRight.X && p.Y > rect.Y)
                return new Rect(rect.TopRight, p);

            else if (p.X < rect.BottomRight.X && p.Y < rect.BottomRight.Y)
                return new Rect(p, rect.BottomRight);

            else
                return rect;
        }

        private void DrawRectangle(Rect rect) {
            backDrawing.Children.Clear();
            backDrawing.Children.Add(new GeometryDrawing(new SolidColorBrush(Colors.DodgerBlue) { Opacity = 0.3 }, null, new RectangleGeometry(rect)));


            frontDrawing.Children.Clear();
            frontDrawing.Children.Add(new GeometryDrawing(null, new Pen(Brushes.Green, 2), new RectangleGeometry(rect)));
        }
        #endregion

        protected override Freezable CreateInstanceCore() {
            return new SelectRangeMouseHandler();
        }
    }
}
