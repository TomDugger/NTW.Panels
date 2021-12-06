using System.Windows;

namespace NTW.Panels {
    /// <summary>
    /// Standart size helper
    /// </summary>
    public static class SizeHelper {

        /// <summary>
        /// Allow to get size without infinity values
        /// </summary>
        /// <param name="original">Original size</param>
        /// <param name="correct">currect size</param>
        /// <returns>Result size</returns>
        public static Size CheckInfinity(Size original, Size correct) {
            double width = original.Width;
            double height = original.Height;

            if (double.IsInfinity(original.Width))
                width = correct.Width;

            if (double.IsInfinity(original.Height))
                height = correct.Height;

            return new Size(width, height);
        }

        /// <summary>
        /// Allow to get size without infinity values
        /// </summary>
        /// <param name="original">Original size</param>
        /// <param name="width">Correct width</param>
        /// <param name="height">Correct height</param>
        /// <returns>Result size</returns>
        public static Size CheckInfinity(Size original, double width, double height) {
            return CheckInfinity(original, new Size(width, height));
        }

        /// <summary>
        /// Allow to check first value and second value for infinity 
        /// </summary>
        /// <param name="firstValue">First value</param>
        /// <param name="secondValue">Second value</param>
        /// <returns>Return value</returns>
        public static double NoInfinity(double firstValue, double secondValue)
            => double.IsInfinity(firstValue) ? (double.IsInfinity(secondValue) ? 0 : secondValue) : firstValue;

        /// <summary>
        /// Allow to get rect without infinity values
        /// </summary>
        /// <param name="rect">Rectangle</param>
        /// <param name="size">Correct size</param>
        /// <returns>Return rect</returns>
        public static Rect CheckInfinity(Rect rect, Size size) {

            double width = rect.Width;
            double height = rect.Height;

            if (double.IsInfinity(width))
                width = size.Width;

            if (double.IsInfinity(height))
                height = size.Height;

            return new Rect(rect.Location, new Size(width, height));
        }
    }
}
