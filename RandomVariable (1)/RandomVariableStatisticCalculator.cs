using System.IO;
using ShutingYardParser;

namespace RandomVariable
{
    public class RandomVariableStatisticCalculator : IRandomVariableStatisticCalculator
    {
        private readonly Parser _parser;

        public RandomVariableStatisticCalculator()
        {
            _parser = new Parser();
        }

        public RandomVariableStatistic CalculateStatistic(string expression,
            params StatisticKind[] statisticForCalculate)
        {
            var randomVarStatistic = new RandomVariableStatistic();
            foreach (var statisticKind in statisticForCalculate)
            {
                switch (statisticKind)
                {
                    case StatisticKind.ExpectedValue:
                        randomVarStatistic.ExpectedValue = ShutingYardAlgoritm(expression);
                        break;
                }
            }

            return randomVarStatistic;
        }

        private double ShutingYardAlgoritm(string str)
        {
            using var reader = new StringReader(str);
            var tokens = _parser.Tokenize(reader);

            return _parser.ShuntingYard(tokens);
        }
    }
}