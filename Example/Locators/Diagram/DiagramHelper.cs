using NTW.Panels;
using System.Windows;

namespace Example.Locators {
    public class DiagramHelper {

        public static string GetLegend(DependencyObject obj) {
            return (string)obj.GetValue(LegendProperty);
        }

        public static void SetLegend(DependencyObject obj, string value) {
            obj.SetValue(LegendProperty, value);
        }

        public static readonly DependencyProperty LegendProperty =
            DependencyProperty.RegisterAttached("Legend", typeof(string), typeof(DiagramHelper), new PropertyMetadata(null));


        public static double GetAngle(DependencyObject obj) {
            return (double)obj.GetValue(AngleProperty);
        }

        public static readonly DependencyProperty AngleProperty =
            DependencyProperty.RegisterAttached("Angle", typeof(double), typeof(DiagramHelper), new PropertyMetadata(0.0, AngleChanged));

        private static void AngleChanged(DependencyObject sender, DependencyPropertyChangedEventArgs e) {
            double angle = (double)e.NewValue;

            sender.SetValue(ReverseAngleProperty, -angle);

            sender.SetValue(LessThan90Property, false);
            sender.SetValue(LessThan180Property, false);
            sender.SetValue(LessThan270Property, false);
            sender.SetValue(LessThan360Property, false);

            if (angle < 90)
                sender.SetValue(LessThan90Property, true);
            else if (angle < 180)
                sender.SetValue(LessThan180Property, true);
            else if (angle < 270)
                sender.SetValue(LessThan270Property, true);
            else if (angle < 360)
                sender.SetValue(LessThan360Property, true);
        }


        public static double GetReverseAngle(DependencyObject obj) {
            return (double)obj.GetValue(ReverseAngleProperty);
        }

        public static readonly DependencyProperty ReverseAngleProperty =
            DependencyProperty.RegisterAttached("ReverseAngle", typeof(double), typeof(DiagramHelper), new PropertyMetadata(0.0));


        public static bool GetLessThan90(DependencyObject obj) {
            return (bool)obj.GetValue(LessThan90Property);
        }

        private static void SetLessThan90(DependencyObject obj, bool value) {
            obj.SetValue(LessThan90Property, value);
        }

        public static readonly DependencyProperty LessThan90Property =
            DependencyProperty.RegisterAttached("LessThan90", typeof(bool), typeof(DiagramHelper), new PropertyMetadata(false));


        public static bool GetLessThan180(DependencyObject obj) {
            return (bool)obj.GetValue(LessThan180Property);
        }

        private static void SetLessThan180(DependencyObject obj, bool value) {
            obj.SetValue(LessThan180Property, value);
        }

        public static readonly DependencyProperty LessThan180Property =
            DependencyProperty.RegisterAttached("LessThan180", typeof(bool), typeof(DiagramHelper), new PropertyMetadata(false));


        public static bool GetLessThan270(DependencyObject obj) {
            return (bool)obj.GetValue(LessThan270Property);
        }

        private static void SetLessThan270(DependencyObject obj, bool value) {
            obj.SetValue(LessThan270Property, value);
        }

        public static readonly DependencyProperty LessThan270Property =
            DependencyProperty.RegisterAttached("LessThan270", typeof(bool), typeof(DiagramHelper), new PropertyMetadata(false));


        public static bool GetLessThan360(DependencyObject obj) {
            return (bool)obj.GetValue(LessThan360Property);
        }

        private static void SetLessThan360(DependencyObject obj, bool value) {
            obj.SetValue(LessThan360Property, value);
        }

        public static readonly DependencyProperty LessThan360Property =
            DependencyProperty.RegisterAttached("LessThan360", typeof(bool), typeof(DiagramHelper), new PropertyMetadata(false));


        public static double GetValue(DependencyObject obj) {
            return (double)obj.GetValue(ValueProperty);
        }

        public static void SetValue(DependencyObject obj, double value) {
            obj.SetValue(ValueProperty, value);
        }

        public static readonly DependencyProperty ValueProperty =
            DependencyProperty.RegisterAttached("Value", typeof(double), typeof(DiagramHelper), new PropertyMetadata(0.0, UpdateGeneralProperties));


        public static double GetMinimum(DependencyObject obj) {
            return (double)obj.GetValue(MinimumProperty);
        }

        public static void SetMinimum(DependencyObject obj, double value) {
            obj.SetValue(MinimumProperty, value);
        }

        public static readonly DependencyProperty MinimumProperty =
            DependencyProperty.RegisterAttached("Minimum", typeof(double), typeof(DiagramHelper), new PropertyMetadata(0.0, UpdateGeneralProperties));


        public static double GetMaximum(DependencyObject obj) {
            return (double)obj.GetValue(MaximumProperty);
        }

        public static void SetMaximum(DependencyObject obj, double value) {
            obj.SetValue(MaximumProperty, value);
        }

        public static readonly DependencyProperty MaximumProperty =
            DependencyProperty.RegisterAttached("Maximum", typeof(double), typeof(DiagramHelper), new PropertyMetadata(0.0, UpdateGeneralProperties));

        private static void UpdateGeneralProperties(DependencyObject sender, DependencyPropertyChangedEventArgs e) {
            if (sender is FrameworkElement fe && fe.Parent is CustomPanel panel && panel.ItemsLocator is IDiagramLocator di)
                di.RebuildDiagram();
        }
    }
}
