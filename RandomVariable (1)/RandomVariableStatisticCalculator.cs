using System;
using System.IO;
using System.Linq;
using System.Linq.Expressions;
using ShuntingYardParser;

namespace RandomVariable
{
    public class RandomVariableStatisticCalculator : IRandomVariableStatisticCalculator
    {
        private Parser _parser;

        public RandomVariableStatisticCalculator()
        {
            _parser = new Parser();
        }

        public RandomVariableStatistic CalculateStatistic(string expression,
            params StatisticKind[] statisticForCalculate)
        {
            var RandomVarStatistic = new RandomVariableStatistic();
            foreach (var statisticKind in statisticForCalculate)
            {
                switch (statisticKind)
                {
                    case StatisticKind.ExpectedValue:
                        RandomVarStatistic.ExpectedValue = ShutingYardAlgoritm(expression);
                        break;
                }
            }

            return RandomVarStatistic;
        }

        private static double ShutingYardAlgoritm(string str)
        {
            using (var reader = new StringReader(str))
            {
                var parser = new Parser();
                var tokens = parser.Tokenize(reader).ToList();

                return parser.ShuntingYard(tokens);
            }
        }
    }
}