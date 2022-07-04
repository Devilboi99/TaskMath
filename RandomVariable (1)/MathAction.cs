using System;
using System.Collections.Generic;

namespace RandomVariable
{
    public class MathAction
    {
        public Dictionary<string, Func<(double, double), double>> _mathAction { get; private set; }
        public MathAction()
        {
            _mathAction = new Dictionary<string, Func<(double, double), double>>
            {
                {"+", d => d.Item2 + d.Item1},
                {"-", d => d.Item2 - d.Item1},
                {"*", d => d.Item2 * d.Item1},
                {"/", d => d.Item2 / d.Item1}
            };
        }

        public double this[string operation, (double,double) lastNums] => 
            _mathAction[operation](lastNums);
    }
}