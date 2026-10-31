using System.ComponentModel;
using Calculator.Core;
using Calculator.ViewModels;

namespace Calculator
{
    public partial class MainPage : ContentPage
    {
        // Result font range, in device-independent units
        const double MaxResultFontSize = 56;
        const double MinResultFontSize = 20;

        // Longest mantissa tried when the grouped number doesn't fit, e.g. 1.23456789012345E+20
        const int MaxScientificDigits = 15;

        readonly MainViewModel viewModel;
        double fittedWidth;

        public MainPage(MainViewModel viewModel)
        {
            InitializeComponent();
            BindingContext = this.viewModel = viewModel;

            viewModel.PropertyChanged += OnViewModelPropertyChanged;
            DisplayContent.SizeChanged += OnDisplaySizeChanged;
            FitResult();
        }

        void OnViewModelPropertyChanged(object? sender, PropertyChangedEventArgs e)
        {
            if (e.PropertyName == nameof(MainViewModel.Display))
                FitResult();
        }

        // Height also changes when the font does, so refit only when the width changes (rotation, window resize)
        void OnDisplaySizeChanged(object? sender, EventArgs e)
        {
            if (DisplayContent.Width != fittedWidth)
                FitResult();
        }

        // Shows the result as large as the display's real width allows: the grouped number first,
        // then scientific notation with fewer and fewer digits, so digits are never cut off
        void FitResult()
        {
            var available = DisplayContent.Width;
            fittedWidth = available;

            var candidates = ResultTexts(viewModel.Display);
            if (Result.Handler == null || available <= 0)
            {
                // Not laid out yet; SizeChanged fits it once the width is known
                Result.Text = candidates[0];
                return;
            }

            foreach (var text in candidates)
            {
                if (FitFontSize(text, available) is { } size)
                {
                    Result.Text = text;
                    Result.FontSize = size;
                    return;
                }
            }

            Result.Text = candidates[^1];
            Result.FontSize = MinResultFontSize;
        }

        static List<string> ResultTexts(string display)
        {
            var texts = new List<string> { DisplayFormatter.AddGroupSeparators(display) };
            for (var digits = MaxScientificDigits; digits >= 1; digits--)
            {
                if (DisplayFormatter.ToScientific(display, digits) is { } scientific && !texts.Contains(scientific))
                    texts.Add(scientific);
            }
            return texts;
        }

        // Largest whole font size at which text fits in width, or null if it doesn't fit even at the minimum
        double? FitFontSize(string text, double width)
        {
            Result.Text = text;
            Result.FontSize = MaxResultFontSize;
            var naturalWidth = MeasureResult();
            if (naturalWidth <= width)
                return MaxResultFontSize;

            // Text width grows in proportion to the font size: start from that estimate,
            // then step down in case rounding or kerning left it a little too wide
            for (var size = Math.Floor(MaxResultFontSize * width / naturalWidth); size >= MinResultFontSize; size--)
            {
                Result.FontSize = size;
                if (MeasureResult() <= width)
                    return size;
            }
            return null;
        }

        // Asks the platform for the label's natural one-line width with its current text and font
        double MeasureResult() =>
            Result.Measure(double.PositiveInfinity, double.PositiveInfinity).Width;
    }
}
