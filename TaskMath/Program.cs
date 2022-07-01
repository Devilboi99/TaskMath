using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using RandomVariable;
using ShuntingYardParser;

public class Program
{
    public static void Main()
    {
        var parse = new Parser();
        Console.WriteLine(parse.GetDiscreteRandomVariable("2d6"));
    }
}

