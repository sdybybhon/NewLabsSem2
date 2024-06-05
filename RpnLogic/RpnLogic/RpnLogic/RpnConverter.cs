using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RpnLogic
{
    internal class RpnConverter
    {
        public static Queue<Token> ConvertToRPN(List<Token> tokens)
        {
            Queue<Token> rpn = new Queue<Token>();
            Stack<Token> stack = new Stack<Token>();

            foreach (Token token in tokens)
            {
                if (token is Number)
                {
                    rpn.Enqueue(token);
                }
                else if (token is MathOperation)
                {
                    while (stack.Count > 0 && stack.Peek() is MathOperation && ((MathOperation)stack.Peek()).Priority > ((MathOperation)token).Priority)
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
        {
            Stack<double> stack = new Stack<double>();

            while (rpn.Count > 0)
            {
                Token token = rpn.Dequeue();

                if (token is Number)
                {
                    stack.Push(((Number)token).Value);
                }
                else if (token is MathOperation)
                {
                    int requiredOperands = ((MathOperation)token).RequiredOperands;
                    if (stack.Count < requiredOperands)
                    {
                        throw new InvalidOperationException("Неправильное количество операндов");
                    }

                    double[] operands = new double[requiredOperands];
                    for (int i = 0; i < requiredOperands; i++)
                    {
                        operands[i] = stack.Pop();
                    }

                    double result = ((MathOperation)token).Perform(operands);
                    stack.Push(result);
                }
            }

            return stack.Pop();
        }
    }
}
