using NTW.Panels;
using System.Windows;
using System.Windows.Media;

namespace Examples.Designers {
    public class ZoomDesigner : CustomDesigner, IScaleTransformDesigner {

        private ScaleTransform scaleTransform;

        public ZoomDesigner() {
            this.scaleTransform = new ScaleTransform();
        }

        #region Properties
        public Size MinimumLimit {
            get { return (Size)GetValue(MinimumLimitProperty); }
            set { SetValue(MinimumLimitProperty, value); }
        }

        public static readonly DependencyProperty MinimumLimitProperty =
            DependencyProperty.Register("MinimumLimit", typeof(Size), typeof(ZoomDesigner), new PropertyMetadata(new Size(0.5, 0.5)));


        public Size MaximumLimit {
            get { return (Size)GetValue(MaximumLimitProperty); }
            set { SetValue(MaximumLimitProperty, value); }
        }

        public static readonly DependencyProperty MaximumLimitProperty =
            DependencyProperty.Register("MaximumLimit", typeof(Size), typeof(ZoomDesigner), new PropertyMetadata(new Size(3, 3)));
        #endregion

        #region IScaleTransformDesigner
        public double ScaleX {
            get { return (double)GetValue(ScaleXProperty); }
            set { SetValue(ScaleXProperty, value); }
        }

        public static readonly DependencyProperty ScaleXProperty =
            DependencyProperty.Register("ScaleX", typeof(double), typeof(ZoomDesigner), new PropertyMetadata(1.0,  ScaleXChanged));

        private static void ScaleXChanged(DependencyObject sender, DependencyPropertyChangedEventArgs e) {
            if (sender is ZoomDesigner designer && e.NewValue is double value)
                designer.scaleTransform.ScaleX = value;
        }


        public double ScaleY {
            get { return (double)GetValue(ScaleYProperty); }
            set { SetValue(ScaleYProperty, value); }
        }

        public static readonly DependencyProperty ScaleYProperty =
            DependencyProperty.Register("ScaleY", typeof(double), typeof(ZoomDesigner), new PropertyMetadata(1.0, ScaleYChanged));

        private static void ScaleYChanged(DependencyObject sender, DependencyPropertyChangedEventArgs e) {
            if (sender is ZoomDesigner designer && e.NewValue is double value)
                designer.scaleTransform.ScaleY = value;
        }


        public Transform GetTransform() => scaleTransform ?? Transform.Identity;

        public void SetScale(double x, double y, bool set = false) {
            // Scale X
            if (ScaleX + x < MinimumLimit.Width)
                ScaleX = MinimumLimit.Width;
            else if (ScaleX + x > MaximumLimit.Width)
                ScaleX = MaximumLimit.Width;
            else
                ScaleX = set ? x : ScaleX + x;

            // Scale Y
            if (ScaleY + y < MinimumLimit.Height)
                ScaleY = MinimumLimit.Height;
            else if (ScaleY + y > MaximumLimit.Height)
                ScaleY = MaximumLimit.Height;
            else
                ScaleY = set ? y : ScaleY + y;

            this.SetUpdateOption(this, UpdateOptions.Measure);
        }

        public void SetScaleCenter(double x, double y) {
            scaleTransform.CenterX = x;
            scaleTransform.CenterY = y;
        }
        #endregion

        protected override Freezable CreateInstanceCore() {
            return new ZoomDesigner();
        }
    }
}
