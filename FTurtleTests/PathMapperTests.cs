using System;
using System.Collections.Generic;
using System.Linq;
using FTurtle;
using FTurtle.Application;
using FTurtle.Domain;
using FTurtleTests.Tools;
using Xunit;
using Xunit.Abstractions;
using Xunit.Sdk;

namespace FTurtleTests
{
    public class PathMapperTests : IClassFixture<PathTokenizerDefault>
    {
        private readonly ITestOutputHelper _output;
        private readonly IPathMapper _sot;
        private readonly RelativeMapper _sot2;
        private readonly IPathTokenizer _tokenizer;

        public PathMapperTests(ITestOutputHelper output, PathTokenizerDefault tokenizer)
        {
            _output = output;
            _tokenizer = tokenizer;
            _sot = new PathMapper();
            _sot2 = new RelativeMapper();
        }

        [Fact]
        public void TestPathMapperRotationsFiltering()
        {
            var path = PathGenerator.Create(20);
            var movements = _sot2.Map(_tokenizer.Parse(path), Heading.North);
            Assert.Equal(path.Count(c => c == (char)Command.Move), movements.Count()); // MapRelative removes all 'R' and 'L' tokens
        }

        [Theory]
        [MemberData(nameof(TestDataSimple))]
        public void TestPathMapperSimple((int, int) expected, string path)
        {
            var movements = _sot2.Map(_tokenizer.Parse(path), Heading.North).ToArray();
            var final = movements[^1];
            Assert.Equal(expected, final.Head);
        }

        [Fact]
        public void TestPathMapperCanAcceptEmptyData()
        {
            var path = "";
            var movements = _sot2.Map(_tokenizer.Parse(path), Heading.North);
            Assert.Empty(movements);

            // Default tokenizer trims tail rotations
            path = "LLLRRRLRLR";
            movements = _sot2.Map(_tokenizer.Parse(path), Heading.North);
            Assert.Empty(movements);
        }

        [Fact]
        public void TestPathMapperAbsolute()
        {
            var path = "MMLMMLMRMLLM";
            var start = new Position { X = 0, Y = 0, Heading = Heading.South };

            var trace = _sot.Map(_tokenizer.Parse(path), start).ToArray();
            Assert.Equal(path.Count(c => c == (char)Command.Move) + 1, trace.Length); // +1 for initialPosition

            var expected = new (int, int)[]
            {
                // Initial position
                (start.X, start.Y),
                // two steps to South (initial Heading is South)
                (1, 0), 
                (2, 0), 
                // two steps to East
                (2, 1), 
                (2, 2), 
                // one step to North
                (1, 2), 
                // one step to East
                (1, 3),
                // one step to West
                (1, 2)
            };

            for (var i = 0; i < expected.Length; ++i)
            {
                Assert.Equal(expected[i], (trace[i].X, trace[i].Y));
            }
        }

        public static IEnumerable<object[]> TestDataSimple()
        {
            yield return new object[] { Heading.North, "MMMMMM" };
            yield return new object[] { Heading.East, "MRMMLRLR" };
            yield return new object[] { Heading.South, "LMMLM" };
            yield return new object[] { Heading.West, "MRMRMMRMM" };
        }

    }
}
