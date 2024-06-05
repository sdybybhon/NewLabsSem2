using System.Globalization;
using System.Text;

namespace RpnLogic
{
    internal class Tokenizer
    {
        public static List<Token> Tokenize(string expression)
        {
            List<Token> tokens = new List<Token>();
            StringBuilder currentNumber = new StringBuilder();
            bool decimalSeparatorFound = false;

            for (int i = 0; i < expression.Length; i++)
            {
                char c = expression[i];

                if (char.IsDigit(c))
                {
                    currentNumber.Append(c);
                }
                else if (c == '.')
                {
                    if (!decimalSeparatorFound)
                    {
                        currentNumber.Append('.');
                        decimalSeparatorFound = true;
                    }
                    else
                    {
                        throw new FormatException("Неправильный формат числа");
                    }
                }
                else
                {
                    if (currentNumber.Length > 0)
                    {
                        double number = double.Parse(currentNumber.ToString(), CultureInfo.InvariantCulture);
                        tokens.Add(new Number(number));
                        currentNumber.Clear();
                        decimalSeparatorFound = false;
                    }

                    if (c == '(' || c == ')')
                    {
                        tokens.Add(new Parenthesis(c.ToString()));
                    }
                    else if (c == '+')
                    {
                        tokens.Add(new Addition());
                    }
                    else if (c == '-')
                    {
                        tokens.Add(new Subtraction());
                    }
                    else if (c == '*')
                    {
                        tokens.Add(new Multiplication());
                    }
                    else if (c == '/')
                    {
                        tokens.Add(new Division());
                    }
                    else if (c == 'l' && expression.Substring(i, 3).ToLower() == "log")
                    {
                        tokens.Add(new Logarithm());
                        i += 2;
                    }
                    else if (c == '^')
                    {
                        tokens.Add(new Power());
                    }
                    else if (c == 's' && expression.Substring(i, 3).ToLower() == "sin")
                    {
                        tokens.Add(new Sine());
                        i += 2;
                    }
                    else if (c == 'c' && expression.Substring(i, 3).ToLower() == "cos")
                    {
                        tokens.Add(new Cosine());
                        i += 2;
                    }
                    else if (c == 't' && expression.Substring(i, 2).ToLower() == "tg")
                    {
                        tokens.Add(new Tangent());
                        i += 1;
                    }
                    else if (c == 'c' && expression.Substring(i, 3).ToLower() == "ctg")
                    {
                        tokens.Add(new Cotangent());
                        i += 2;
                    }
                    else if (c == 's' && expression.Substring(i, 4).ToLower() == "sqrt")
                    {
                        tokens.Add(new SquareRoot());
                        i += 3;
                    }
                    else if (c == 'r' && expression.Substring(i, 2).ToLower() == "rt")
                    {
                        tokens.Add(new Root());
                        i += 1;
                    }
                    else if (char.IsLetter(c) && c.ToString().ToLower() == "x")
                    {
                        tokens.Add(new Variable(c.ToString()));
                    }
                }
            }

            if (currentNumber.Length > 0)
            {
                double number = double.Parse(currentNumber.ToString(), CultureInfo.InvariantCulture);
                tokens.Add(new Number(number));
            }

            return tokens;
        }

        private static Token GetMathOperationToken(string value)
        {
            switch (value)
            {
                case "+":
                    return new Addition();
                case "-":
                    return new Subtraction();
                case "*":
                    return new Multiplication();
                case "/":
                    return new Division();
                case "log":
                    return new Logarithm();
                case "^":
                    return new Power();
                case "sqrt":
                    return new SquareRoot();
                case "rt":
                    return new Root();
                case "sin":
                    return new Sine();
                case "cos":
                    return new Cosine();
                case "tg":
                    return new Tangent();
                case "ctg":
                    return new Cotangent();
                default:
                    throw new InvalidOperationException("Недопустимая операция");
            }
        }
    }
}
