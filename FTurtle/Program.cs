using System;
using FTurtle.Domain;

namespace FTurtle
{
    class Program
    {
        static void Main(string[] args)
        {
            Console.WriteLine("Hello World!");
            var p = new Position {X = 0, Y = 0, Heading = Arrow.Create(Heading.North)};
        }
    }
}
