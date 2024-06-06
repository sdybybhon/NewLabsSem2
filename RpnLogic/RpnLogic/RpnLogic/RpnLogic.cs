using System;
using System.Collections;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;

namespace RpnLogic
{
    public class RpnCalculator
    {
        public double CalculateWithVariable(string expression, double xValue)
        {
            List<Token> tokens = Tokenizer.Tokenize(expression);
            ReplaceVariableTokens(tokens, xValue);
            Queue<Token> rpn = RpnConverter.ConvertToRPN(tokens);
            double result = RpnConverter.EvaluateRPN(rpn);
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

    internal abstract class Token
    {
        public virtual string Value { get; protected set; }
        public virtual int Priority { get; protected set; }
    }

    internal class Number : Token
    {
        public double Value { get; private set; }

        public Number(double value)
        {
            Value = value;
        }
    }

    internal abstract class MathOperation : Token
    {
        public abstract int RequiredOperands { get; }
        public abstract double Perform(params double[] operands);
    }

    internal class Addition : MathOperation
    {
        public override string Value => "+";
        public override int Priority => 1;
        public override int RequiredOperands => 2;

        public override double Perform(params double[] operands)
        {
            return operands[0] + operands[1];
        }
    }

    internal class Subtraction : MathOperation
    {
        public override string Value => "-";
        public override int Priority => 1;
        public override int RequiredOperands => 2;

        public override double Perform(params double[] operands)
        {
            return operands[1] - operands[0];
        }
    }

    internal class Multiplication : MathOperation
    {
        public override string Value => "*";
        public override int Priority => 2;
        public override int RequiredOperands => 2;

        public override double Perform(params double[] operands)
        {
            return operands[0] * operands[1];
        }
    }

    internal class Division : MathOperation
    {
        public override string Value => "/";
        public override int Priority => 2;
        public override int RequiredOperands => 2;

        public override double Perform(params double[] operands)
        {
            return operands[1] / operands[0];
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

    internal class Logarithm : MathOperation
    {
        public override string Value => "log";
        public override int Priority => 3;
        public override int RequiredOperands => 2;

        public override double Perform(params double[] operands)
        {
            return Math.Log(operands[0], operands[1]);
        }
    }

    internal class Power : MathOperation
    {
        public override string Value => "^";
        public override int Priority => 3;
        public override int RequiredOperands => 2;

        public override double Perform(params double[] operands)
        {
            return Math.Pow(operands[1], operands[0]);
        }
    }


    internal class SquareRoot : MathOperation
    {
        public override string Value => "sqrt";
        public override int Priority => 3;
        public override int RequiredOperands => 1;

        public override double Perform(params double[] operands)
        {
            return Math.Sqrt(operands[0]);
        }
    }

    internal class Root : MathOperation
    {
        public override string Value => "rt";
        public override int Priority => 3;
        public override int RequiredOperands => 2;

        public override double Perform(params double[] operands)
        {
            return Math.Pow(operands[0], 1 / operands[1]);
        }
    }

    internal class Sine : MathOperation
    {
        public override string Value => "sin";
        public override int Priority => 3;
        public override int RequiredOperands => 1;

        public override double Perform(params double[] operands)
        {
            return Math.Sin(operands[0]);
        }
    }

    internal class Cosine : MathOperation
    {
        public override string Value => "cos";
        public override int Priority => 3;
        public override int RequiredOperands => 1;

        public override double Perform(params double[] operands)
        {
            return Math.Cos(operands[0]);
        }
    }

    internal class Tangent : MathOperation
    {
        public override string Value => "tg";
        public override int Priority => 3;
        public override int RequiredOperands => 1;

        public override double Perform(params double[] operands)
        {
            return Math.Tan(operands[0]);
        }
    }

    internal class Cotangent : MathOperation
    {
        public override string Value => "ctg";
        public override int Priority => 3;
        public override int RequiredOperands => 1;

        public override double Perform(params double[] operands)
        {
            return 1.0 / Math.Tan(operands[0]);
        }
    }
}
