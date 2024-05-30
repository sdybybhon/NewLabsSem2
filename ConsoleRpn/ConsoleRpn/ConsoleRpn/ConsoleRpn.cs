using System;
using System.Collections.Generic;
using RpnLogic;

namespace RpnConsoleApp
{
    class RpnConsole
    {
        static void Main(string[] args)
        {
            Console.Write("Ваше выражение: ");
            string expression = Console.ReadLine().Replace(" ", string.Empty);

            Console.Write("Значение x: ");
            string xValueStr = Console.ReadLine();
            double xValue;
            if (!double.TryParse(xValueStr, out xValue))
            {
                Console.WriteLine("Неверное значение для x");
                return;
            }

            RpnCalculator calculator = new RpnCalculator();
            double result = calculator.CalculateWithVariable(expression, xValue);

            Console.WriteLine($"Результат: {result}");
        }
    }
}