using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using RandomVariable;
using ShuntingYardParser;

public class Tests
{
    /*private RandomVariableStatisticCalculator RandomMath = new RandomVariableStatisticCalculator();*/
    private Parser _parser = new Parser();
    private Tuple<string,double>[] _expressionsWithoutBrackets = new Tuple<string, double>[]
    {
        Tuple.Create("5 + 3", 8d),
        Tuple.Create("8 - 4", 4d),
        Tuple.Create("6 * 2", 12d),
        Tuple.Create("6 / 3", 2d),
        Tuple.Create("10 + 20 * 30", 610d),
        Tuple.Create("20 * 10 / 5 + 1", 41d),
        Tuple.Create("22 + 1 * 3 / 2", 23.5),
    };
    
    private Tuple<string,double>[] _expressionWithBrackets = new Tuple<string, double>[]
    {
        Tuple.Create("(5 + 3)", 8d),
        Tuple.Create("(8 - 4)", 4d),
        Tuple.Create("(6 * 2)", 12d),
        Tuple.Create("(6 / 3)", 2d),
        Tuple.Create("(10 + 20) * 30", 900d),
        Tuple.Create("20 * 10 / (5 + 1)", 33.33d), 
        Tuple.Create("(22 + 1) * (3 / 2)", 34.5d),
        Tuple.Create("( 3 + 2 + (4 + 3 ) * 1 ) * 2",24d)
    };
    
    
    [SetUp]
    public void Setup()
    {
    }

    [Test]
    public void basicMathTestShuntingYardWithoutBracket()
    {
        foreach (var expression in _expressionsWithoutBrackets)
            Assert.AreEqual(expression.Item2, ShutingYardAlgoritm(expression.Item1));
    }
    
    [Test]
    public void basicMathTestShuntingYardWithBracket()
    {
        var j = 0;
        foreach (var expression in _expressionWithBrackets)
        {
            Assert.AreEqual(expression.Item2, ShutingYardAlgoritm(expression.Item1), 5e-1);
            j++;
        }
            
    }

    private static double ShutingYardAlgoritm(string str)
    {
        using (var reader = new StringReader(str))
        {
            var parser = new Parser();
            var tokens = parser.Tokenize(reader).ToList();
            //Console.WriteLine(string.Join("\n", tokens));

            return parser.ShuntingYard(tokens);
        }
    }

    [Test]
    public void Test1()
    {
        /*
                Assert.Equals(new RandomVariableStatistic() {ExpectedValue = 6}, RandomMath.CalculateStatistic("2+2*2", StatisticKind.Variance));
                Assert.Equals(10.5, RandomMath.CalculateStatistic("1d20", StatisticKind.Variance));
                Assert.Equals(5.7, RandomMath.CalculateStatistic("2d6+(-1d12/5)", StatisticKind.Variance));
                Assert.Equals(6, RandomMath.CalculateStatistic("2+2*2", StatisticKind.Variance));
                
                Assert.Pass();*/
    }
}