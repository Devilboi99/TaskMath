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

    private Tuple<double, string>[] _expressionsWithoutBrackets = new Tuple<double, string>[]
    {
        Tuple.Create(8d , "5+3"),
        Tuple.Create(4d, "8-4"),
        Tuple.Create(12d,"6*2"),
        Tuple.Create(2d ,"6/3"),
        Tuple.Create(610d, "10+20*30"),
        Tuple.Create(41d, "20* 10/5+1"),
        Tuple.Create(23.5, "22+1*3/2"),
    };

    private Tuple<double, string>[] _expressionWithBrackets = new Tuple<double, string>[]
    {
        Tuple.Create(8d, "(5+3)"),
        Tuple.Create(4d, "(8-4)"),
        Tuple.Create(12d, "(6*2)"),
        Tuple.Create(2d, "(6/3)"),
        Tuple.Create(900d, "(10+20)*30"),
        Tuple.Create(33.33d, "20*10/(5+1)"),
        Tuple.Create(34.5, "(22+1)*(3/2)"),
        Tuple.Create(24d,"(3+2+(4+3)*1)*2")
    };

    private Tuple<double, string>[] _expressionWithRandomVar = new Tuple<double, string>[]
    {
        Tuple.Create(10.5, "1d20"),
        Tuple.Create( 9d,"2d6+2" ),
        Tuple.Create(5.7,"2d6+(-1d12/5)"),
        Tuple.Create(6.75, "(1d12+2d6)/2")
    };

    

    [SetUp]
    public void Setup()
    {
    }

    [Test]
    public void BasicMathTestShuntingYardWithoutBracket()
    {
        foreach (var expression in _expressionsWithoutBrackets)
            Assert.AreEqual(expression.Item1, ShutingYardAlgoritm(expression.Item2));
    }

    [Test]
    public void BasicMathTestShuntingYardWithBracket()
    {
        foreach (var expression in _expressionWithBrackets)
                Assert.AreEqual(expression.Item1, ShutingYardAlgoritm(expression.Item2), 5e-1);
    }

    [Test]

    public void ExpressionWithRandomVariable()
    {
        foreach (var expression in _expressionWithRandomVar)
            Assert.AreEqual(expression.Item1, ShutingYardAlgoritm(expression.Item2), 5e-1); 
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
    public void TokenRandomVariable()
    {
        var str = "1d20"; 
        using (var reader = new StringReader(str))
        {
            var parser = new Parser();
            var tokens = parser.Tokenize(reader).ToList();
            Assert.AreEqual(new Token(TokenType.RandomVariable, "1d20"),tokens.First());
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