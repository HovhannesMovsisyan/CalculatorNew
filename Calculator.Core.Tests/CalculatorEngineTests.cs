using Calculator.Core;

namespace Calculator.Core.Tests
{
    public class CalculatorEngineTests
    {
        // Presses space-separated keys, e.g. "1 2 × 3 ="
        static CalculatorEngine Press(string keys)
        {
            var engine = new CalculatorEngine();
            foreach (var key in keys.Split(' ', StringSplitOptions.RemoveEmptyEntries))
            {
                switch (key)
                {
                    case "+": engine.SetOperator(Operator.Add); break;
                    case "−": engine.SetOperator(Operator.Subtract); break;
                    case "×": engine.SetOperator(Operator.Multiply); break;
                    case "÷": engine.SetOperator(Operator.Divide); break;
                    case "=": engine.Calculate(); break;
                    case ".": engine.InputDecimal(); break;
                    case "C": engine.Clear(); break;
                    case "±": engine.Negate(); break;
                    case "%": engine.Percent(); break;
                    case "⌫": engine.Backspace(); break;
                    default:
                        foreach (var c in key)
                            engine.InputDigit(c - '0');
                        break;
                }
            }
            return engine;
        }

        [Fact]
        public void StartsAtZero()
        {
            var engine = new CalculatorEngine();

            Assert.Equal("0", engine.Display);
            Assert.Equal("", engine.Expression);
            Assert.False(engine.HasError);
        }

        // Same cases as the web version's browser tests
        [Theory]
        [InlineData("1 2 × 3 =", "36", "12 × 3 =")]
        [InlineData(". 1 + . 2 =", "0.3", "0.1 + 0.2 =")]
        [InlineData("2 + 3 × 4 =", "20", "5 × 4 =")]
        [InlineData("1 ÷ 3 =", "0.3333333333", "1 ÷ 3 =")]
        [InlineData("5 ±", "-5", "")]
        [InlineData("5 ± ⌫", "0", "")]
        [InlineData("5 0 %", "0.5", "")]
        [InlineData("1 2 3 ⌫", "12", "")]
        [InlineData("9 + × 2 =", "18", "9 × 2 =")]
        [InlineData("0 0 7", "7", "")]
        [InlineData("1 . . 5", "1.5", "")]
        [InlineData("2 + 3 = + 1 =", "6", "5 + 1 =")]
        [InlineData("8 − 1 0 =", "-2", "8 − 10 =")]
        [InlineData("0 . ± ⌫ 5", "-5", "")]
        public void Calculates(string keys, string display, string expression)
        {
            var engine = Press(keys);

            Assert.Equal(display, engine.Display);
            Assert.Equal(expression, engine.Expression);
        }

        [Theory]
        [InlineData("7", "7", "")]
        [InlineData("7 +", "7", "7 +")]
        [InlineData("7 + 2", "2", "7 +")]
        [InlineData("7 + 2 ×", "9", "9 ×")]
        public void ShowsPendingOperation(string keys, string display, string expression)
        {
            var engine = Press(keys);

            Assert.Equal(display, engine.Display);
            Assert.Equal(expression, engine.Expression);
        }

        [Fact]
        public void DivideByZeroShowsErrorAndResets()
        {
            var engine = Press("7 ÷ 0 =");

            Assert.Equal(CalculatorEngine.ErrorText, engine.Display);
            Assert.Equal("", engine.Expression);
            Assert.True(engine.HasError);

            engine.InputDigit(4);
            Assert.Equal("4", engine.Display);
            Assert.False(engine.HasError);
        }

        [Fact]
        public void DivideByZeroInChainShowsError()
        {
            var engine = Press("8 ÷ 0 +");

            Assert.Equal(CalculatorEngine.ErrorText, engine.Display);
            Assert.True(engine.HasError);
        }

        [Fact]
        public void OverflowShowsError()
        {
            // 16 nines squared has 32 digits, more than decimal.MaxValue (~7.9e28)
            var engine = Press("9999999999999999 × 9999999999999999 =");

            Assert.Equal(CalculatorEngine.ErrorText, engine.Display);
            Assert.True(engine.HasError);
        }

        [Fact]
        public void EntryIsLimitedToMaxDigits()
        {
            var engine = Press("12345678901234567890");

            Assert.Equal("1234567890123456", engine.Display);
            Assert.Equal(CalculatorEngine.MaxDigits, engine.Display.Length);
        }

        [Fact]
        public void DigitLimitIgnoresSignAndDecimalPoint()
        {
            var engine = Press("1 . 234567890123456789 ±");

            Assert.Equal("-1.234567890123456", engine.Display);
        }

        [Fact]
        public void LongResultIsShownInFull()
        {
            // Results may be longer than the 16-digit entry limit; the UI shrinks the font to fit
            var engine = Press("9999999999999999 × 9999999 =");

            Assert.Equal("99999989999999990000001", engine.Display);
            Assert.False(engine.HasError);
        }

        [Fact]
        public void TinyResultRoundsToZeroWithoutMinusSign()
        {
            var engine = Press("1 ÷ 1000000000000 ± =");

            Assert.Equal("0", engine.Display);
        }

        [Fact]
        public void EqualsWithoutOperatorDoesNothing()
        {
            var engine = Press("4 2 =");

            Assert.Equal("42", engine.Display);
            Assert.Equal("", engine.Expression);
        }

        [Fact]
        public void RepeatedEqualsKeepsResult()
        {
            var engine = Press("2 + 3 = =");

            Assert.Equal("5", engine.Display);
        }

        [Fact]
        public void DigitAfterResultStartsNewNumber()
        {
            var engine = Press("2 + 3 = 7");

            Assert.Equal("7", engine.Display);
            Assert.Equal("", engine.Expression);
        }

        [Fact]
        public void BackspaceDoesNotEditResult()
        {
            var engine = Press("2 + 3 = ⌫");

            Assert.Equal("5", engine.Display);
        }

        [Fact]
        public void DecimalAfterResultStartsWithZero()
        {
            var engine = Press("2 + 3 = . 5");

            Assert.Equal("0.5", engine.Display);
        }

        [Fact]
        public void NegateZeroDoesNothing()
        {
            var engine = Press("±");

            Assert.Equal("0", engine.Display);
        }

        [Fact]
        public void ClearResetsEverything()
        {
            var engine = Press("1 2 + 3 C");

            Assert.Equal("0", engine.Display);
            Assert.Equal("", engine.Expression);

            engine.InputDigit(5);
            engine.Calculate();
            Assert.Equal("5", engine.Display);
        }

        [Theory]
        [InlineData(-1)]
        [InlineData(10)]
        public void InvalidDigitThrows(int digit)
        {
            Assert.Throws<ArgumentOutOfRangeException>(() => new CalculatorEngine().InputDigit(digit));
        }
    }
}
