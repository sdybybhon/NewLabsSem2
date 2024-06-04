using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using System.Globalization;
using System.Linq;
using System.Linq.Expressions;
using System.Security.Authentication.ExtendedProtection;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Schema;


namespace RpnLogic
{
    public class RpnCalculator
    {
        public double CalculateWithVariable(string expression, double xValue)
        {
            List<Token> tokens = RpnLogic.Tokenize(expression);
            ReplaceVariableTokens(tokens, xValue);
            Queue<Token> rpn = RpnLogic.ConvertToRPN(tokens);
            double result = RpnLogic.EvaluateRPN(rpn);
            return result;
        }

        private static void ReplaceVariableTokens(List<Token> tokens, double xValue)
        {
            for (int i = 0; i < tokens.Count; i++)
            {
                if (tokens[i] is Variable)
                {
                    tokens[i] = new Number(xValue);
                }
            }
        }
    }
    internal class RpnLogic
    {
        public static List<Token> Tokenize(string expression)
        //Этим методом разбиваю выражение на токены и возвращаю список токенов.
        //Прохожу по каждому символу в выражении и определяет, является ли символ числом, операцией или скобкой.
        //Если число, то он добавляет его в список токенов типа Number. 
        //Если это операция или скобка, то он добавляет их в список токенов типа Operation или Parenthesis.
        {
            List<Token> tokens = new List<Token>();
            StringBuilder currentNumber = new StringBuilder();
            bool decimalSeparatorFound = false;

            foreach (char c in expression)
            {
                if (char.IsDigit(c))
                {
                    currentNumber.Append(c);
                }
                else if (c == ',' || c == '.')
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
                    else if (c == '+' || c == '-' || c == '*' || c == '/')
                    {
                        tokens.Add(new Operation(c.ToString(), GetPriority(c.ToString())));
                    }
                    else if (char.IsLetter(c) && c.ToString().ToLower() == "x")
                    {
                        tokens.Add(new Variable(c.ToString()));
                    }
                }
            }

            if (currentNumber.Length > 0)
            {
                double number = double.Parse(currentNumber.ToString().Replace(",", "."));
                tokens.Add(new Number(number));
            }

            return tokens;
        }


        public static Queue<Token> ConvertToRPN(List<Token> tokens)
        //Здесь просто преобразую список токенов в ОПЗ и возвращает очередь токенов.
        //Если токен является числом, то он просто добавляется в очередь. 
        //Если токен является операцией, то метод проверяет приоритет операции и сравнивает его с операциями, уже находящимися в стеке.
        //Если токен является скобкой, то метод проверяет, является ли она открывающей или закрывающей. 
        //И т.п, по итогу получим очередь ОПЗ
        {
            Queue<Token> rpn = new Queue<Token>(); //Создаю пустую очередь токенов для хранения ОПЗ.
            Stack<Token> stack = new Stack<Token>(); //Создаю пустой стек токенов для выполнения преобразований.

            foreach (Token token in tokens)
            {
                if (token is Number)
                {
                    rpn.Enqueue(token);
                }
                else if (token is Operation)
                {
                    while (stack.Count > 0 && stack.Peek() is Operation && ((Operation)stack.Peek()).Priority >= ((Operation)token).Priority)
                    {
                        rpn.Enqueue(stack.Pop());
                    }

                    stack.Push(token);
                }
                else if (token is Parenthesis)
                {
                    if (token.Value == "(")
                    {
                        stack.Push(token);
                    }
                    else if (token.Value == ")")
                    {
                        while (stack.Count > 0 && !(stack.Peek() is Parenthesis) && ((stack.Peek() as Parenthesis)?.Value != "("))
                        {
                            rpn.Enqueue(stack.Pop());
                        }

                        if (stack.Count == 0 || !(stack.Peek() is Parenthesis) || ((stack.Peek() as Parenthesis)?.Value != "("))
                        {
                            throw new InvalidOperationException("Несогласованные скобки");
                        }

                        stack.Pop();
                    }
                }
            }

            while (stack.Count > 0)
            {
                if (stack.Peek() is Parenthesis)
                {
                    throw new InvalidOperationException("Несогласованные скобки");
                }

                rpn.Enqueue(stack.Pop());
            }

            return rpn;
        }

        public static double EvaluateRPN(Queue<Token> rpn)
        //Здесь вычисляем то, что получили в ОПЗ. 
        {
            Stack<double> stack = new Stack<double>();

            while (rpn.Count > 0)
            {
                Token token = rpn.Dequeue();

                if (token is Number)
                {
                    stack.Push(((Number)token).Value);
                }
                else if (token is Operation)
                {
                    if (stack.Count < 2)
                    {
                        throw new InvalidOperationException("Неправильное количество операндов");
                    }

                    double operand2 = stack.Pop();
                    double operand1 = stack.Pop();
                    double result = PerformOperation(operand1, operand2, ((Operation)token).Value);
                    stack.Push(result);
                }
            }

            return stack.Pop();
        }

        static double PerformOperation(double operand1, double operand2, string operation)
        // Метод для выполнения маттмаческих операции
        {
            switch (operation)
            {
                case "+":
                    return operand1 + operand2;
                case "-":
                    return operand1 - operand2;
                case "*":
                    return operand1 * operand2;
                case "/":
                    return operand1 / operand2;
                default:
                    throw new InvalidOperationException("Недопустимая операция");
            }
        }

        static int GetPriority(string operation)
        // Метод для определения приоритета операции
        {
            switch (operation)
            {
                case "+":
                case "-":
                    return 1;
                case "*":
                case "/":
                    return 2;
                default:
                    return 0;
            }
        }
    }

    internal abstract class Token
    {
        public string Value { get; protected set; }
    }

    internal class Number : Token
    {
        public double Value { get; private set; }

        public Number(double value)
        {
            Value = value;
        }
    }

    internal class Operation : Token
    {
        public int Priority { get; private set; }

        public Operation(string value, int priority)
        {
            Value = value;
            Priority = priority;
        }
    }

    internal class Parenthesis : Token
    {
        public Parenthesis(string value)
        {
            Value = value;
        }
    }

    internal class Variable : Token
    {
        public Variable(string value)
        {
            Value = value;
        }
    }
}