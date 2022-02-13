using NTW.Panels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows;
using System.Windows.Media;

namespace Example.Designers {
    public class CanvasOffsetDesigner : CustomDesigner, ITranslateTransformDesigner {

        private TranslateTransform translateTransform;

        public CanvasOffsetDesigner() {
            this.translateTransform = new TranslateTransform();
        }

        #region Properties
        public Rect Bounds {
            get { return (Rect)GetValue(BoundsProperty); }
            set { SetValue(BoundsProperty, value); }
        }

        public static readonly DependencyProperty BoundsProperty =
            DependencyProperty.Register("Bounds", typeof(Rect), typeof(CanvasOffsetDesigner), new PropertyMetadata(new Rect(-100, -100, 100, 100)));
        #endregion

        #region ITranslateTransformDesigner
        public double TranslateX {
            get { return (double)GetValue(TranslateXProperty); }
            set { SetValue(TranslateXProperty, value); }
        }

        public static readonly DependencyProperty TranslateXProperty =
            DependencyProperty.Register("TranslateX", typeof(double), typeof(CanvasOffsetDesigner), new FrameworkPropertyMetadata(0.0, FrameworkPropertyMetadataOptions.AffectsParentArrange, TranslateXChanged));

        private static void TranslateXChanged(DependencyObject sender, DependencyPropertyChangedEventArgs e) {
            if (sender is CanvasOffsetDesigner designer && e.NewValue is double value)
                designer.translateTransform.X = value;
        }


        public double TranslateY {
            get { return (double)GetValue(TranslateYProperty); }
            set { SetValue(TranslateYProperty, value); }
        }

        public static readonly DependencyProperty TranslateYProperty =
            DependencyProperty.Register("TranslateY", typeof(double), typeof(CanvasOffsetDesigner), new FrameworkPropertyMetadata(0.0, FrameworkPropertyMetadataOptions.AffectsParentArrange, TranslateYChanged));

        private static void TranslateYChanged(DependencyObject sender, DependencyPropertyChangedEventArgs e) {
            if (sender is CanvasOffsetDesigner designer && e.NewValue is double value)
                designer.translateTransform.Y = value;
        }


        public Transform GetTransform() => translateTransform ?? Transform.Identity;

        public void SetTranslation(Vector deferent) {
            // Translate X
            if (TranslateX - deferent.X < Bounds.Left)
                TranslateX = Bounds.Left;
            else if (TranslateX - deferent.X > Bounds.Width)
                TranslateX = Bounds.Width;
            else
                TranslateX -= deferent.X;

            // Translate Y
            if (TranslateY - deferent.Y < Bounds.Top)
                TranslateY = Bounds.Top;
            else if (TranslateY - deferent.Y > Bounds.Height)
                TranslateY = Bounds.Height;
            else
                TranslateY -= deferent.Y;
        }
        #endregion

        protected override Freezable CreateInstanceCore() {
            return new CanvasOffsetDesigner();
        }
    }
}
