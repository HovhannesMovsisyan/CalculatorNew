using System.Globalization;
using System.Text;

namespace Calculator.Core
{
    /// <summary>
    /// Formats <see cref="CalculatorEngine"/> text for people to read. Display only: the engine keeps
    /// working with plain numbers, and text that is not a number (such as "Error") passes through unchanged.
    /// </summary>
    public static class DisplayFormatter
    {
        public const char GroupSeparator = ',';

        /// <summary>
        /// Adds thousands separators to the whole-number part, e.g. "7000000" → "7,000,000" and "-1234.50" → "-1,234.50".
        /// Entries still being typed, like "12." or "0.50", keep their decimal point and trailing zeros.
        /// </summary>
        public static string AddGroupSeparators(string text)
        {
            if (!IsNumber(text))
                return text;

            var sign = text.StartsWith('-') ? "-" : string.Empty;
            var unsigned = text[sign.Length..];
            var point = unsigned.IndexOf('.');
            var whole = point < 0 ? unsigned : unsigned[..point];
            var fraction = point < 0 ? string.Empty : unsigned[point..];

            var grouped = new StringBuilder(whole.Length + whole.Length / 3);
            for (var i = 0; i < whole.Length; i++)
            {
                if (i > 0 && (whole.Length - i) % 3 == 0)
                    grouped.Append(GroupSeparator);
                grouped.Append(whole[i]);
            }

            return sign + grouped + fraction;
        }

        /// <summary>Adds thousands separators to each number in an expression line, e.g. "70000 × 100 =" → "70,000 × 100 =".</summary>
        public static string AddGroupSeparatorsToExpression(string expression) =>
            string.Join(' ', expression.Split(' ').Select(AddGroupSeparators));

        /// <summary>
        /// Scientific notation with at most <paramref name="significantDigits"/> digits and no trailing zeros,
        /// e.g. "1234567890123456" with 9 digits → "1.23456789E+15". Returns null for text that is not a number.
        /// </summary>
        public static string? ToScientific(string text, int significantDigits)
        {
            ArgumentOutOfRangeException.ThrowIfLessThan(significantDigits, 1);

            if (!IsNumber(text))
                return null;

            var value = decimal.Parse(text, NumberStyles.Number, CultureInfo.InvariantCulture);
            var format = significantDigits == 1
                ? "0E+0"
                : "0." + new string('#', significantDigits - 1) + "E+0";
            return value.ToString(format, CultureInfo.InvariantCulture);
        }

        // An optional minus, then digits with at most one decimal point
        static bool IsNumber(string text)
        {
            var body = text.StartsWith('-') ? text.AsSpan(1) : text.AsSpan();
            var digits = 0;
            var points = 0;

            foreach (var c in body)
            {
                if (char.IsAsciiDigit(c))
                    digits++;
                else if (c == '.')
                    points++;
                else
                    return false;
            }

            return digits > 0 && points <= 1;
        }
    }
}
