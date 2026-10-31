using Calculator.Core;

namespace Calculator.Core.Tests
{
    public class DisplayFormatterTests
    {
        [Theory]
        [InlineData("0", "0")]
        [InlineData("999", "999")]
        [InlineData("1000", "1,000")]
        [InlineData("7000000", "7,000,000")]
        [InlineData("1234567890123456", "1,234,567,890,123,456")]
        [InlineData("-1234.5", "-1,234.5")]
        [InlineData("-123", "-123")]
        [InlineData("12345.6789012345", "12,345.6789012345")]
        [InlineData("1234.", "1,234.")]
        [InlineData("0.50", "0.50")]
        [InlineData("-0", "-0")]
        [InlineData("Error", "Error")]
        [InlineData("", "")]
        public void AddsGroupSeparators(string text, string expected)
        {
            Assert.Equal(expected, DisplayFormatter.AddGroupSeparators(text));
        }

        [Theory]
        [InlineData("70000 × 100 =", "70,000 × 100 =")]
        [InlineData("1234 −", "1,234 −")]
        [InlineData("-5000 ÷ 2.5 =", "-5,000 ÷ 2.5 =")]
        [InlineData("", "")]
        public void AddsGroupSeparatorsToExpression(string expression, string expected)
        {
            Assert.Equal(expected, DisplayFormatter.AddGroupSeparatorsToExpression(expression));
        }

        [Theory]
        [InlineData("1234567890123456", 9, "1.23456789E+15")]
        [InlineData("7000000", 15, "7E+6")]
        [InlineData("-7000000", 15, "-7E+6")]
        [InlineData("99999989999999990000001", 15, "9.999999E+22")]
        [InlineData("99999", 2, "1E+5")]
        [InlineData("123456", 1, "1E+5")]
        [InlineData("0.0000000012", 3, "1.2E-9")]
        [InlineData("1234.", 3, "1.23E+3")]
        public void FormatsScientific(string text, int significantDigits, string expected)
        {
            Assert.Equal(expected, DisplayFormatter.ToScientific(text, significantDigits));
        }

        [Theory]
        [InlineData("Error")]
        [InlineData("")]
        [InlineData("-")]
        public void ScientificIgnoresNonNumbers(string text)
        {
            Assert.Null(DisplayFormatter.ToScientific(text, 10));
        }

        [Fact]
        public void ScientificNeedsAtLeastOneDigit()
        {
            Assert.Throws<ArgumentOutOfRangeException>(() => DisplayFormatter.ToScientific("5", 0));
        }

        // The real bug report: 70000 × 100 = must show every digit, grouped
        [Fact]
        public void EngineResultIsGroupedForDisplay()
        {
            var engine = new CalculatorEngine();
            foreach (var digit in new[] { 7, 0, 0, 0, 0 })
                engine.InputDigit(digit);
            engine.SetOperator(Operator.Multiply);
            foreach (var digit in new[] { 1, 0, 0 })
                engine.InputDigit(digit);
            engine.Calculate();

            Assert.Equal("7000000", engine.Display);
            Assert.Equal("7,000,000", DisplayFormatter.AddGroupSeparators(engine.Display));
            Assert.Equal("70,000 × 100 =", DisplayFormatter.AddGroupSeparatorsToExpression(engine.Expression));
        }
    }
}
