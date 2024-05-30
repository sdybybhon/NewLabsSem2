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
            string input = Console.ReadLine().Replace(" ", string.Empty);

            RpnCalculator calculator = new RpnCalculator();
            double result = calculator.CalculateExpression(input);

            Console.WriteLine($"Результат: {result}");
        }
    }
}