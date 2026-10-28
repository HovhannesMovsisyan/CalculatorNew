using System.Globalization;

namespace Calculator.Core
{
    /// <summary>
    /// Basic four-function calculator state machine, ported from web/calculator.js.
    /// The UI calls one method per button and then shows <see cref="Display"/> and <see cref="Expression"/>.
    /// </summary>
    public sealed class CalculatorEngine
    {
        public const int MaxDigits = 16;
        public const int MaxDecimals = 10;
        public const string ErrorText = "Error";

        string entry = "0";
        decimal? storedValue;
        Operator? pendingOperator;
        bool startNewEntry = true;

        /// <summary>The main number on screen, or <see cref="ErrorText"/>.</summary>
        public string Display { get; private set; } = "0";

        /// <summary>The smaller line above the result, e.g. "12 ×" or "12 × 3 =".</summary>
        public string Expression { get; private set; } = string.Empty;

        /// <summary>True after a divide by zero or overflow, until the next key.</summary>
        public bool HasError { get; private set; }

        public void InputDigit(int digit)
        {
            ArgumentOutOfRangeException.ThrowIfNegative(digit);
            ArgumentOutOfRangeException.ThrowIfGreaterThan(digit, 9);

            var text = digit.ToString(CultureInfo.InvariantCulture);

            if (startNewEntry || entry == "0")
                entry = text;
            else if (entry == "-0")
                entry = "-" + text;
            else if (entry.Count(char.IsDigit) < MaxDigits)
                entry += text;

            startNewEntry = false;
            UpdateDisplay();
        }

        public void InputDecimal()
        {
            if (startNewEntry)
                entry = "0.";
            else if (!entry.Contains('.'))
                entry += ".";

            startNewEntry = false;
            UpdateDisplay();
        }

        public void SetOperator(Operator op)
        {
            // Chain calculations: 2 + 3 + evaluates the 2 + 3 before applying the next operator
            if (pendingOperator != null && storedValue != null && !startNewEntry)
            {
                if (!TryEvaluate(out var result))
                    return;
                entry = Format(result);
            }

            storedValue = Parse(entry);
            pendingOperator = op;
            startNewEntry = true;
            UpdateDisplay();
        }

        /// <summary>The "=" key.</summary>
        public void Calculate()
        {
            if (pendingOperator == null || storedValue == null)
                return;

            var expression = $"{Format(storedValue.Value)} {pendingOperator.Value.Symbol()} {entry} =";
            if (!TryEvaluate(out var result))
                return;

            entry = Format(result);
            storedValue = null;
            pendingOperator = null;
            startNewEntry = true;
            UpdateDisplay(expression);
        }

        public void Clear()
        {
            Reset();
            UpdateDisplay();
        }

        public void Negate()
        {
            if (entry == "0")
                return;

            entry = entry.StartsWith('-') ? entry[1..] : "-" + entry;
            UpdateDisplay();
        }

        public void Percent()
        {
            entry = Format(Parse(entry) / 100);
            startNewEntry = true;
            UpdateDisplay();
        }

        public void Backspace()
        {
            if (startNewEntry)
                return;

            entry = entry.Length > 1 ? entry[..^1] : "0";
            if (entry == "-")
                entry = "0";
            UpdateDisplay();
        }

        // Shows "Error" and resets on divide by zero or overflow
        bool TryEvaluate(out decimal result)
        {
            var left = storedValue!.Value;
            var right = Parse(entry);
            result = 0;

            try
            {
                result = pendingOperator switch
                {
                    Operator.Add => left + right,
                    Operator.Subtract => left - right,
                    Operator.Multiply => left * right,
                    Operator.Divide => right == 0 ? throw new DivideByZeroException() : left / right,
                    _ => right
                };
                return true;
            }
            catch (Exception ex) when (ex is DivideByZeroException or OverflowException)
            {
                Reset();
                Display = ErrorText;
                Expression = string.Empty;
                HasError = true;
                return false;
            }
        }

        void Reset()
        {
            entry = "0";
            storedValue = null;
            pendingOperator = null;
            startNewEntry = true;
        }

        void UpdateDisplay(string? expression = null)
        {
            HasError = false;
            Display = entry;
            Expression = expression
                ?? (pendingOperator != null && storedValue != null
                    ? $"{Format(storedValue.Value)} {pendingOperator.Value.Symbol()}"
                    : string.Empty);
        }

        static decimal Parse(string text) =>
            decimal.Parse(text, NumberStyles.Number, CultureInfo.InvariantCulture);

        // Round off long fractions (e.g. 1 ÷ 3), drop trailing zeros, and never show "-0"
        static string Format(decimal value)
        {
            var rounded = Math.Round(value, MaxDecimals);
            if (rounded == 0)
                rounded = 0m;
            return rounded.ToString("0.##########", CultureInfo.InvariantCulture);
        }
    }
}
