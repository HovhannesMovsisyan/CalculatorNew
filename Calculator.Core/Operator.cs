namespace Calculator.Core
{
    public enum Operator
    {
        Add,
        Subtract,
        Multiply,
        Divide
    }

    public static class OperatorExtensions
    {
        // Symbols shown in the expression line, same as the web and MAUI buttons
        public static string Symbol(this Operator op) => op switch
        {
            Operator.Add => "+",
            Operator.Subtract => "−",
            Operator.Multiply => "×",
            Operator.Divide => "÷",
            _ => throw new ArgumentOutOfRangeException(nameof(op), op, null)
        };
    }
}
