using System;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Controls.Primitives;
using System.Windows.Media;

namespace NTW.Panels {
    /// <summary>
    /// Base panel with scrollInfo Implementation 
    /// </summary>
    public abstract class ScrollPanel : Panel, IScrollInfo {

        protected const double lineSize = 16;
        protected const double wheelSize = 3 * lineSize;

        protected bool canHorizontallyScroll;
        protected bool canVerticallyScroll;
        protected ScrollViewer scrollOwner;
        protected Vector offset;
        protected Size extent;
        protected Size viewport;

        public bool CanVerticallyScroll { get => canVerticallyScroll; set => canVerticallyScroll = value; }
        public bool CanHorizontallyScroll { get => canHorizontallyScroll; set => canHorizontallyScroll = value; }


        public double ExtentWidth => extent.Width;

        public double ExtentHeight => extent.Height;


        public double ViewportWidth => viewport.Width;

        public double ViewportHeight => viewport.Height;


        public double HorizontalOffset => offset.X;

        public double VerticalOffset => offset.Y;


        public ScrollViewer ScrollOwner { get => scrollOwner; set => scrollOwner = value; }


        public void LineDown() {
            SetVerticalOffset(VerticalOffset + lineSize);
        }

        public void LineLeft() {
            SetHorizontalOffset(HorizontalOffset - lineSize);
        }

        public void LineRight() {
            SetHorizontalOffset(HorizontalOffset + lineSize);
        }

        public void LineUp() {
            SetVerticalOffset(VerticalOffset - lineSize);
        }


        public virtual Rect MakeVisible(Visual visual, Rect rectangle) {
            return rectangle;
        }


        public void MouseWheelDown() {
            SetVerticalOffset(VerticalOffset + wheelSize);
        }

        public void MouseWheelLeft() {
            SetHorizontalOffset(HorizontalOffset - wheelSize);
        }

        public void MouseWheelRight() {
            SetHorizontalOffset(HorizontalOffset + wheelSize);
        }

        public void MouseWheelUp() {
            SetVerticalOffset(VerticalOffset - wheelSize);
        }


        public void PageDown() {
            SetVerticalOffset(VerticalOffset + ViewportHeight);
        }

        public void PageLeft() {
            SetHorizontalOffset(HorizontalOffset - ViewportWidth);
        }

        public void PageRight() {
            SetHorizontalOffset(HorizontalOffset + ViewportWidth);
        }

        public void PageUp() {
            SetVerticalOffset(VerticalOffset - ViewportHeight);
        }


        public void SetHorizontalOffset(double offset) {
            this.offset.X = Math.Max(0, Math.Min(offset, ExtentWidth - ViewportWidth));
            InvalidateArrange();
        }

        public void SetVerticalOffset(double offset) {

            this.offset.Y = Math.Max(0, Math.Min(offset, ExtentHeight - ViewportHeight));
            InvalidateArrange();
        }

        protected void VerifyScrollData(Size viewport, Size extent) {
            this.extent = extent;
            this.viewport = viewport;

            offset.X = Math.Max(0, Math.Min(offset.X, ExtentWidth - ViewportWidth));
            offset.Y = Math.Max(0, Math.Min(offset.Y, ExtentHeight - ViewportHeight));

            if (ScrollOwner != null) ScrollOwner.InvalidateScrollInfo();
        }
    }
}
