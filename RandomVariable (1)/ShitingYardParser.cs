using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using RandomVariable;

namespace ShutingYardParser
{
    public enum TokenType
    {
        Number,
        Parenthesis,
        Operator,
        WhiteSpace,
        RandomVariable,
        Dot
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
        private readonly MathAction _mathAction = new MathAction();
        
        private IDictionary<string, Operator> operators = new Dictionary<string, Operator>
        {
            ["+"] = new Operator {Name = "+", Precedence = 1},
            ["-"] = new Operator {Name = "-", Precedence = 1},
            ["*"] = new Operator {Name = "*", Precedence = 2},
            ["/"] = new Operator {Name = "/", Precedence = 2},
        };

        private bool CompareOperators(Operator op1, Operator op2)
        => op1.Precedence <= op2.Precedence;

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
            if (ch == '.')
                return TokenType.Dot;

            throw new Exception("Wrong character: " + ch);
        }

        public IEnumerable<Token> Tokenize(TextReader reader)
        {
            var token = new StringBuilder();

            int curr;
            var isRandomVariable = false;
            var prevNum = false;
            while ((curr = reader.Read()) != -1)
            {
                var ch = (char) curr;
                var currType = DetermineType(ch);
                if (currType == TokenType.WhiteSpace)
                    continue;
                
                token.Append(ch);

                if (!prevNum && curr == '-')
                    continue;
                if (curr == 'd')
                    isRandomVariable = true;
                
                
                var next = reader.Peek();
                prevNum = currType == TokenType.Number;
                var nextType = next != -1 ? DetermineType((char) next) : TokenType.WhiteSpace;
                
                if (currType != nextType && IsElementNotPartNum(currType, nextType))
                {
                    if (isRandomVariable)
                    {
                        yield return new Token(TokenType.RandomVariable, token.ToString());
                        isRandomVariable = false;
                    }
                    else
                    {
                        yield return new Token(currType, token.ToString());
                    }

                    token.Clear();
                }
            }
        }

        private bool IsElementNotPartNum(TokenType currType, TokenType nextType)
            => nextType != TokenType.Dot && currType != TokenType.Dot && currType != TokenType.RandomVariable &&
               TokenType.RandomVariable != nextType;

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
                            if (stackOperations.Count != 0)
                                MergeNum(stackOperations, stackNums);
                        }

                        break;
                    default:
                        throw new Exception("Wrong token");
                }
            }
            while (stackOperations.Count != 0)
                MergeNum(stackOperations, stackNums);

            return stackNums.Pop();
        }

        public double GetDiscreteRandomVariable(string line)
        {
            var nums = line
                .Split("d")
                .Select(double.Parse)
                .ToArray();
            var result = 0d;
            for (var j = 1; j <= nums[1]; j++)
                result += j;

            return result / nums[1] * nums[0];
        }

        private void MergeNum(Stack<Token> stackOperations, Stack<double> stackNums)
        {
            var operation = stackOperations.Pop().Value;
            var lastNums = (stackNums.Pop(),stackNums.Pop());
            stackNums.Push(_mathAction[operation, lastNums]);
        }
    }
}