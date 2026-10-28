using System.Globalization;

namespace Calculator.Converters
{
    /// <summary>
    /// Shrinks the result font so long numbers fit the display, like fitResult() in web/calculator.js.
    /// Values: [text, available width]. Parameter: the maximum font size.
    /// </summary>
    public sealed class FitFontSizeConverter : IMultiValueConverter
    {
        // Rough width of one OpenSans Semibold digit, relative to the font size
        const double CharWidthRatio = 0.62;

        public double MinFontSize { get; set; } = 20;

        public object Convert(object[] values, Type targetType, object? parameter, CultureInfo culture)
        {
            var maxFontSize = parameter is string text
                ? double.Parse(text, CultureInfo.InvariantCulture)
                : 56;

            if (values is not [string display, double width] || display.Length == 0 || width <= 0)
                return maxFontSize;

            var fitted = Math.Floor(width / (display.Length * CharWidthRatio));
            return Math.Clamp(fitted, MinFontSize, maxFontSize);
        }

        public object[] ConvertBack(object value, Type[] targetTypes, object? parameter, CultureInfo culture) =>
            throw new NotSupportedException();
    }
}
