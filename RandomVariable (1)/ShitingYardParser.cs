using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;

namespace ShuntingYardParser
{
    public enum TokenType
    {
        Number,
        Function,
        Parenthesis,
        Operator,
        WhiteSpace,
        RandomVariable,
    };

    public struct Token
    {
        public TokenType Type { get; }
        public string Value { get; }

        public override string ToString() => $"{Type}: {Value}";

        public Token(TokenType type, string value)
        {
            Type = type;
            Value = value;
        }
    }

    class Operator
    {
        public string Name { get; set; }
        public int Precedence { get; set; }
    }

    public class Parser
    {
        private IDictionary<string, Operator> operators = new Dictionary<string, Operator>
        {
            ["+"] = new Operator {Name = "+", Precedence = 1},
            ["-"] = new Operator {Name = "-", Precedence = 1},
            ["*"] = new Operator {Name = "*", Precedence = 2},
            ["/"] = new Operator {Name = "/", Precedence = 2},
        };

        private bool CompareOperators(Operator op1, Operator op2)
        {
            return op1.Precedence <= op2.Precedence;
        }

        private bool CompareOperators(string op1, string op2) => CompareOperators(operators[op1], operators[op2]);

        private TokenType DetermineType(char ch)
        {
            if (char.IsDigit(ch))
                return TokenType.Number;
            if (char.IsWhiteSpace(ch))
                return TokenType.WhiteSpace;
            if (ch == '(' || ch == ')')
                return TokenType.Parenthesis;
            if (operators.ContainsKey(Convert.ToString(ch)))
                return TokenType.Operator;
            if (ch == 'd')
                return TokenType.RandomVariable;

            throw new Exception("Wrong character");
        }

        public IEnumerable<Token> Tokenize(TextReader reader)
        {
            var token = new StringBuilder(4);

            int curr;
            var isRandomVariable = false;
            while ((curr = reader.Read()) != -1)
            {
                var ch = (char) curr;
                var currType = DetermineType(ch);
                if (currType == TokenType.WhiteSpace || currType == TokenType.RandomVariable)
                    continue;

                token.Append(ch);
                var next = reader.Peek();
                if (next == 'd')
                {
                    token.Append((char) next);
                    isRandomVariable = true;
                    continue;
                }

                var nextType = next != -1 ? DetermineType((char) next) : TokenType.WhiteSpace;
                if (currType != nextType)
                {
                    if (isRandomVariable)
                    {
                        yield return new Token(TokenType.RandomVariable, token.ToString());
                        isRandomVariable = false;
                    }
                    else
                        yield return new Token(currType, token.ToString());
                    token.Clear();
                }
            }
        }

        public double ShuntingYard(IEnumerable<Token> tokens)
        {
            var stackOperations = new Stack<Token>();
            var stackNums = new Stack<double>();
            foreach (var tok in tokens)
            {
                switch (tok.Type)
                {
                    case TokenType.Number:
                        stackNums.Push(double.Parse(tok.Value));
                        break;
                    case TokenType.Function:
                        stackOperations.Push(tok);
                        break;
                    case TokenType.Operator:
                        while (stackOperations.Any() && stackOperations.Peek().Type == TokenType.Operator &&
                               CompareOperators(tok.Value, stackOperations.Peek().Value))
                            MergeNum(stackOperations, stackNums);
                        stackOperations.Push(tok);
                        break;
                    case TokenType.RandomVariable:
                        stackNums.Push(GetDiscreteRandomVariable(tok.Value));
                        break;
                    case TokenType.Parenthesis:
                        if (tok.Value == "(")
                            stackOperations.Push(tok);
                        else
                        {
                            while (stackOperations.Peek().Value != "(")
                                MergeNum(stackOperations, stackNums);
                            stackOperations.Pop();
                            if (stackOperations.Count != 0 && stackOperations.Peek().Type == TokenType.Function)
                                MergeNum(stackOperations, stackNums);
                        }

                        break;
                    default:
                        throw new Exception("Wrong token");
                }
            }

            var answer = 0.0d;
            while (stackOperations.Count != 0)
                MergeNum(stackOperations, stackNums);

            return stackNums.Pop();
            /*while (stackOperations.Any())
            {
                var tok = stackOperations.Pop();
                if (tok.Type == TokenType.Parenthesis)
                    throw new Exception("Mismatched parentheses");
                yield return tok;
            }*/
            throw new ArgumentException();
        }

        public double GetDiscreteRandomVariable(string line)
        {
            var nums = line
                .Split("d")
                .Select(double.Parse)
                .ToArray();
            var result = 0;
            for (var j = 1; j <= nums[1]; j++)
                result += j;

            return result / nums[1] * nums[0];
        }

        private static void MergeNum(Stack<Token> stackOperations, Stack<double> stackNums)
        {
            var operation = stackOperations.Pop().Value;
            var mergeNum = 0d;
            if (operation == "+")
                mergeNum = stackNums.Pop() + stackNums.Pop();
            else if (operation == "-")
            {
                var firstNum = stackNums.Pop();
                mergeNum = stackNums.Pop() - firstNum;
            }
            else if (operation == "*")
                mergeNum = stackNums.Pop() * stackNums.Pop();
            else if (operation == "/")
            {
                var firstNum = stackNums.Pop();
                mergeNum = stackNums.Pop() / firstNum;
            }

            stackNums.Push(mergeNum);
        }
    }

    public class Program
    {
        public static void Main(string[] args)
        {
            var text = Console.ReadLine();
            using (var reader = new StringReader(text))
            {
                var parser = new Parser();
                var tokens = parser.Tokenize(reader).ToList();
                //Console.WriteLine(string.Join("\n", tokens));

                var rpn = parser.ShuntingYard(tokens);
                Console.WriteLine(rpn);
            }
        }
    }
}