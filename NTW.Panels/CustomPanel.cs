using System;
using System.ComponentModel;
using System.Linq;
using System.Windows;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Interop;
using System.Windows.Media;

namespace NTW.Panels {

    /// <summary>
    /// Custom panel with locator and handlers implementation
    /// </summary>
    public class CustomPanel: ScrollPanel, IInputElement {

        private static IItemsLocator defaultLocator = new StackLocator();
        private AdornerDrawingPresenter upLayer;

        static CustomPanel() {
            BackgroundProperty.OverrideMetadata(typeof(CustomPanel), new FrameworkPropertyMetadata(Brushes.Transparent, FrameworkPropertyMetadataOptions.AffectsRender));
        }

        public CustomPanel() : base() {

            this.ClipToBounds = true;

            this.Focusable = true;

            FocusManager.SetIsFocusScope(this, true);

            this.Handlers = new HandlersCollection();

            this.Loaded += PanelLoaded;
        }

        private void PanelUnloaded(object sender, RoutedEventArgs e) {

            if (upLayer != null) {
                var adornerLayer = AdornerLayer.GetAdornerLayer(this);

                if (adornerLayer != null)
                    adornerLayer.Remove(upLayer);

                upLayer = null;

                this.Loaded += PanelLoaded;
                this.Unloaded -= PanelUnloaded;
            }
        }

        private void PanelLoaded(object sender, RoutedEventArgs e) {

            if (upLayer == null) {

                var adornerLayer = AdornerLayer.GetAdornerLayer(this);

                if (adornerLayer != null) {
                    upLayer = new AdornerDrawingPresenter(this);
                    adornerLayer.Add(upLayer);
                } else
                    this.IsVisibleChanged += PanelIsVisibleChanged;

                this.Loaded -= PanelLoaded;
                this.Unloaded += PanelUnloaded;
            }
        }

        private void PanelIsVisibleChanged(object sender, DependencyPropertyChangedEventArgs e) {
            if (upLayer == null && IsVisible) {

                var adornerLayer = AdornerLayer.GetAdornerLayer(this);

                upLayer = new AdornerDrawingPresenter(this);

                if (adornerLayer != null) {
                    adornerLayer.Add(upLayer);
                }

                this.Loaded -= PanelLoaded;
                this.IsVisibleChanged -= PanelIsVisibleChanged;
            }
        }

        #region Properties
        /// <summary>
        /// Panel items locator
        /// </summary>
        public CustomLocator ItemsLocator {
            get { return (CustomLocator)GetValue(ItemsLocatorProperty); }
            set { SetValue(ItemsLocatorProperty, value); }
        }

        public static readonly DependencyProperty ItemsLocatorProperty =
            DependencyProperty.Register("ItemsLocator", typeof(CustomLocator), typeof(CustomPanel), new FrameworkPropertyMetadata(null,
                FrameworkPropertyMetadataOptions.Inherits,
                ItemsLocatorPropertyChanged));

        private static void ItemsLocatorPropertyChanged(DependencyObject d, DependencyPropertyChangedEventArgs e) {

            if (d is CustomPanel sender && e.OldValue != e.NewValue) {

                if (e.OldValue is ICallingModifer callOld)
                    callOld.PanelCalling -= sender.PanelCalling;

                if (e.OldValue is INotifyOption optionOld)
                    optionOld.OptionCalling -= sender.UpdateOptionCalling;


                if (e.NewValue is ICallingModifer callNew)
                    callNew.PanelCalling += sender.PanelCalling;

                if (e.NewValue is INotifyOption optionNew)
                    optionNew.OptionCalling += sender.UpdateOptionCalling;
            }
        }

        internal void UpdateOptionCalling(CustomObject sender, UpdateOptions option) {
            switch (option) {
                case UpdateOptions.Arrange:
                case UpdateOptions.ParentArrange:
                    this.InvalidateArrange();
                    break;
                case UpdateOptions.Measure:
                case UpdateOptions.ParentMeasure:
                    this.InvalidateMeasure();
                    break;
            }
        }

        internal void PanelCalling(Action<CustomPanel> action) {
            action?.Invoke(this);
        }

        /// <summary>
        /// Handlers collection
        /// </summary>
        public HandlersCollection Handlers {
            get { return (HandlersCollection)GetValue(HandlersProperty); }
            set { SetValue(HandlersProperty, value); }
        }

        public static readonly DependencyProperty HandlersProperty =
            DependencyProperty.Register("Handlers", typeof(HandlersCollection), typeof(CustomPanel), new FrameworkPropertyMetadata(null, HandlersPropertyChanged));

        private static void HandlersPropertyChanged(DependencyObject sender, DependencyPropertyChangedEventArgs e) {

            if (sender is CustomPanel panel) {
                if (e.OldValue is HandlersCollection o) {
                    o.ClearOwner();
                    o.OptionCalling -= panel.UpdateOptionCalling;
                }

                if (e.NewValue is HandlersCollection n) {
                    n.OptionCalling += panel.UpdateOptionCalling;
                    n.SetOwner(panel);
                }
            }
        }
        #endregion


        protected override Size MeasureOverride(Size availableSize) {

            if (!IsResizingFinished) return new Size();

            if (DesignerProperties.GetIsInDesignMode(this)) return new Size();

            Size result = (ItemsLocator ?? defaultLocator).Measure(availableSize, this.InternalChildren.Cast<UIElement>().ToArray());

            return this.InternalChildren.Count == 0 ? new Size() : result;
        }

        protected override Size ArrangeOverride(Size finalSize) {

            if (!IsResizingFinished) return new Size();

            if (DesignerProperties.GetIsInDesignMode(this) || !this.IsVisible) return new Size();

            var offset = new Vector(double.IsNaN(this.offset.X) ? 0 : this.offset.X, double.IsNaN(this.offset.Y) ? 0 : this.offset.Y);

            Size result = (ItemsLocator ?? defaultLocator).Arrange(finalSize, offset, default(Vector), out Size verifySize, true, this.InternalChildren.Cast<UIElement>().ToArray());

            VerifyScrollData(finalSize, verifySize);

            return this.InternalChildren.Count == 0 ? new Size() : result;
        }


        protected override void OnRender(DrawingContext dc) {

            if (!IsResizingFinished) return;

            base.OnRender(dc);

            if (this.ItemsLocator is IDrawingPresenter dp && dp.BackDrawing != null)
                dc.DrawDrawing(dp.BackDrawing);

            foreach (IDrawingPresenter drawingPresenter in this.Handlers.Where(x => x.IsActive && x is IDrawingPresenter idp && idp.BackDrawing != null))
                dc.DrawDrawing(drawingPresenter.BackDrawing);
        }


        protected override void OnPreviewMouseDown(MouseButtonEventArgs e) {

            foreach (IMouseDownHandler mouseHandler in Handlers.Where(x => x.IsActive && x is IMouseDownHandler)) {
                if (mouseHandler.CanDownExecute(e, this.InternalChildren)) {
                    mouseHandler.DownExecute(this.InternalChildren, e.GetPosition(this), ItemsLocator ?? defaultLocator, this.RenderSize, this.offset, out bool stop);

                    if (stop)
                        break;
                }
            }

            base.OnPreviewMouseDown(e);
        }

        protected override void OnPreviewMouseMove(MouseEventArgs e) {
            foreach (IMouseMoveHandler mouseHandler in Handlers.Where(x => x.IsActive && x is IMouseMoveHandler)) {
                if (mouseHandler.CanMoveExecute(e, this.InternalChildren)) {
                    mouseHandler.MoveExecution(this.InternalChildren, e.GetPosition(this), ItemsLocator ?? defaultLocator, this.RenderSize, this.offset, out bool stop);

                    if (!this.IsFocused)
                        this.Focus();

                    if (stop)
                        break;
                }
            }

            base.OnPreviewMouseMove(e);
        }

        protected override void OnPreviewMouseUp(MouseButtonEventArgs e) {
            foreach (IMouseUpHandler mouseHandler in Handlers.Where(x => x.IsActive && x is IMouseUpHandler)) {
                if (mouseHandler.CanUpExecution(e, this.InternalChildren)) {
                    mouseHandler.UpExecution(this.InternalChildren, e.GetPosition(this), ItemsLocator ?? defaultLocator, this.RenderSize, this.offset, out bool stop);

                    if (stop)
                        break;
                }
            }

            base.OnPreviewMouseUp(e);
        }

        protected override void OnPreviewMouseWheel(MouseWheelEventArgs e) {

            foreach (IMouseWheelHandler mouseHandler in Handlers.Where(x => x.IsActive && x is IMouseWheelHandler)) {
                if (mouseHandler.CanWheelExecution(e, this.InternalChildren)) {
                    mouseHandler.WheelExecution(this.InternalChildren, e.GetPosition(this), e.Delta, ItemsLocator ?? defaultLocator, this.RenderSize, this.offset, out bool stop);

                    if (stop)
                        break;
                }
            }

            base.OnPreviewMouseWheel(e);
        }


        protected override void OnMouseEnter(MouseEventArgs e) {

            foreach (IMouseEnterHandler mouseHandler in Handlers.Where(x => x.IsActive && x is IMouseEnterHandler)) {
                if (mouseHandler.CanEnterExecute(e, this.InternalChildren)) {
                    mouseHandler.EnterExecute(this.InternalChildren, e.GetPosition(this), ItemsLocator ?? defaultLocator, this.RenderSize, this.offset, out bool stop);

                    if (stop)
                        break;
                }
            }

            base.OnMouseEnter(e);
        }

        protected override void OnMouseLeave(MouseEventArgs e) {

            foreach (IMouseLeaveHandler mouseHandler in Handlers.Where(x => x.IsActive && x is IMouseLeaveHandler)) {
                if (mouseHandler.CanLeaveExecute(e, this.InternalChildren)) {
                    mouseHandler.LeaveExecute(this.InternalChildren, e.GetPosition(this), ItemsLocator ?? defaultLocator, this.RenderSize, this.offset, out bool stop);

                    if (stop)
                        break;
                }
            }

            base.OnMouseLeave(e);
        }


        protected override void OnPreviewKeyDown(KeyEventArgs e) {

            foreach (IKeyHandler keyHandler in Handlers.Where(x => x.IsActive && x is IKeyHandler)) {
                if (keyHandler.CanKeyDown(e, this.InternalChildren)) {
                    keyHandler.KeyDownExecution(this.InternalChildren, e.Key, ItemsLocator ?? defaultLocator, this.RenderSize, this.offset, out bool stop);

                    if (stop)
                        break;
                }
            }

            base.OnPreviewKeyDown(e);
        }

        protected override void OnPreviewKeyUp(KeyEventArgs e) {

            foreach (IKeyHandler keyHandler in Handlers.Where(x => x.IsActive && x is IKeyHandler)) {
                if (keyHandler.CanKeyUp(e, this.InternalChildren)) {
                    keyHandler.KeyUpExecution(this.InternalChildren, e.Key, ItemsLocator ?? defaultLocator, this.RenderSize, this.offset, out bool stop);

                    if (stop)
                        break;
                }
            }

            base.OnPreviewKeyUp(e);
        }


        protected override void OnPreviewDragEnter(DragEventArgs e) {

            foreach (IDragEnterHandler dragHandler in Handlers.Where(x => x.IsActive && x is IDragEnterHandler)) {
                if (dragHandler.CanDragEnterExecute(e, this.InternalChildren)) {
                    dragHandler.DragEnterExecute(this.InternalChildren, new DragDropData(e, e.GetPosition(this)), ItemsLocator ?? defaultLocator, this.RenderSize, this.offset, out bool stop);

                    if (stop)
                        break;
                }
            }

            base.OnPreviewDragEnter(e);
        }

        protected override void OnPreviewDragLeave(DragEventArgs e) {

            foreach (IDragLeaveHandler dragHandler in Handlers.Where(x => x.IsActive && x is IDragLeaveHandler)) {
                if (dragHandler.CanDragLeaveExecute(e, this.InternalChildren)) {
                    dragHandler.DragLeaveExecute(this.InternalChildren, new DragDropData(e, e.GetPosition(this)), ItemsLocator ?? defaultLocator, this.RenderSize, this.offset, out bool stop);

                    if (stop)
                        break;
                }
            }

            base.OnPreviewDragLeave(e);
        }

        protected override void OnPreviewDragOver(DragEventArgs e) {

            foreach (IDragOverHandler dragHandler in Handlers.Where(x => x.IsActive && x is IDragOverHandler)) {
                if (dragHandler.CanDragOverExecute(e, this.InternalChildren)) {
                    dragHandler.DragOverExecute(this.InternalChildren, new DragDropData(e, e.GetPosition(this)), ItemsLocator ?? defaultLocator, this.RenderSize, this.offset, out bool stop);

                    if (stop)
                        break;
                }
            }

            base.OnPreviewDragOver(e);
        }

        protected override void OnPreviewDrop(DragEventArgs e) {

            foreach(IDropHandler dropHandler in Handlers.Where(x => x.IsActive && x is IDropHandler)) {
                if (dropHandler.CanDropExecute(e, this.InternalChildren)) {
                    dropHandler.DropExecute(this.InternalChildren, new DragDropData(e, e.GetPosition(this)), ItemsLocator ?? defaultLocator, this.RenderSize, this.offset, out bool stop);

                    if (stop)
                        break;
                }
            }

            base.OnPreviewDrop(e);
        }

        protected override void ResizingStart() {
            upLayer?.Clear();

            this.InvalidateMeasure();
        }

        protected override void ResizingFinished() {

            upLayer?.Refresh();

            this.InvalidateMeasure();
        }
    }
}
