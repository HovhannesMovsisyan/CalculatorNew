using System.Globalization;

namespace Calculator
{
    public partial class MainPage : ContentPage
    {
        const int MaxDigits = 16;

        string entry = "0";
        decimal? storedValue;
        string? pendingOperator;
        bool startNewEntry = true;

        public MainPage()
        {
            InitializeComponent();
        }

        private void OnDigitClicked(object? sender, EventArgs e)
        {
            if (sender is not Button button)
                return;

            if (startNewEntry || entry == "0")
                entry = button.Text;
            else if (entry.Count(char.IsDigit) < MaxDigits)
                entry += button.Text;

            startNewEntry = false;
            UpdateDisplay();
        }

        private void OnDecimalClicked(object? sender, EventArgs e)
        {
            if (startNewEntry)
                entry = "0.";
            else if (!entry.Contains('.'))
                entry += ".";

            startNewEntry = false;
            UpdateDisplay();
        }

        private void OnOperatorClicked(object? sender, EventArgs e)
        {
            if (sender is not Button button)
                return;

            // Chain calculations: 2 + 3 + evaluates the 2 + 3 before applying the next operator
            if (pendingOperator != null && storedValue != null && !startNewEntry)
            {
                if (!TryEvaluate(out var result))
                    return;
                entry = Format(result);
            }

            storedValue = Parse(entry);
            pendingOperator = button.Text;
            startNewEntry = true;
            UpdateDisplay();
        }

        private void OnEqualsClicked(object? sender, EventArgs e)
        {
            if (pendingOperator == null || storedValue == null)
                return;

            var expression = $"{Format(storedValue.Value)} {pendingOperator} {entry} =";
            if (!TryEvaluate(out var result))
                return;

            entry = Format(result);
            storedValue = null;
            pendingOperator = null;
            startNewEntry = true;
            UpdateDisplay(expression);
        }

        private void OnClearClicked(object? sender, EventArgs e)
        {
            Reset();
            UpdateDisplay();
        }

        private void OnNegateClicked(object? sender, EventArgs e)
        {
            if (entry == "0")
                return;

            entry = entry.StartsWith('-') ? entry[1..] : "-" + entry;
            UpdateDisplay();
        }

        private void OnPercentClicked(object? sender, EventArgs e)
        {
            entry = Format(Parse(entry) / 100);
            startNewEntry = true;
            UpdateDisplay();
        }

        private void OnBackspaceClicked(object? sender, EventArgs e)
        {
            if (startNewEntry)
                return;

            entry = entry.Length > 1 ? entry[..^1] : "0";
            if (entry == "-")
                entry = "0";
            UpdateDisplay();
        }

        bool TryEvaluate(out decimal result)
        {
            var left = storedValue!.Value;
            var right = Parse(entry);
            result = 0;

            try
            {
                result = pendingOperator switch
                {
                    "+" => left + right,
                    "−" => left - right,
                    "×" => left * right,
                    "÷" => right == 0 ? throw new DivideByZeroException() : left / right,
                    _ => right
                };
                return true;
            }
            catch (Exception ex) when (ex is DivideByZeroException or OverflowException)
            {
                Reset();
                DisplayLabel.Text = "Error";
                ExpressionLabel.Text = string.Empty;
                SemanticScreenReader.Announce("Error");
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
            DisplayLabel.Text = entry;
            ExpressionLabel.Text = expression
                ?? (pendingOperator != null && storedValue != null
                    ? $"{Format(storedValue.Value)} {pendingOperator}"
                    : string.Empty);
        }

        static decimal Parse(string text) =>
            decimal.Parse(text, NumberStyles.Number, CultureInfo.InvariantCulture);

        // Round off long fractions (e.g. 1 ÷ 3) and drop trailing zeros
        static string Format(decimal value) =>
            Math.Round(value, 10).ToString("0.##########", CultureInfo.InvariantCulture);
    }
}
