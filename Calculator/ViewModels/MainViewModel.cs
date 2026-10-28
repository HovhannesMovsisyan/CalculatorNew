using System.Globalization;
using Calculator.Core;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;

namespace Calculator.ViewModels
{
    /// <summary>
    /// Binds the calculator page to <see cref="CalculatorEngine"/>. Each command forwards one button press
    /// to the engine and then copies its display state back into bindable properties.
    /// </summary>
    public sealed partial class MainViewModel : ObservableObject
    {
        readonly CalculatorEngine engine = new();

        public MainViewModel()
        {
            Refresh();
        }

        /// <summary>The main number on screen, or "Error".</summary>
        [ObservableProperty]
        public partial string Display { get; private set; }

        /// <summary>The smaller line above the result, e.g. "12 ×".</summary>
        [ObservableProperty]
        public partial string Expression { get; private set; }

        // Digits come in as XAML CommandParameter strings ("0" to "9")
        [RelayCommand]
        void Digit(string digit)
        {
            engine.InputDigit(int.Parse(digit, CultureInfo.InvariantCulture));
            Refresh();
        }

        [RelayCommand]
        void Decimal()
        {
            engine.InputDecimal();
            Refresh();
        }

        [RelayCommand]
        void SetOperator(Operator op)
        {
            engine.SetOperator(op);
            Refresh();
        }

        [RelayCommand]
        void Calculate()
        {
            engine.Calculate();
            Refresh();
        }

        [RelayCommand]
        void Clear()
        {
            engine.Clear();
            Refresh();
        }

        [RelayCommand]
        void Negate()
        {
            engine.Negate();
            Refresh();
        }

        [RelayCommand]
        void Percent()
        {
            engine.Percent();
            Refresh();
        }

        [RelayCommand]
        void Backspace()
        {
            engine.Backspace();
            Refresh();
        }

        void Refresh()
        {
            Display = engine.Display;
            Expression = engine.Expression;
        }
    }
}
