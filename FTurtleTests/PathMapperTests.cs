using System;
using System.Collections.Generic;
using System.Linq;
using FTurtle;
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
        private readonly IPathTokenizer _tokenizer;

        public PathMapperTests(ITestOutputHelper output, PathTokenizerDefault tokenizer)
        {
            _output = output;
            _tokenizer = tokenizer;
            _sot = new PathMapper();
        }

        [Fact]
        public void TestPathMapperRotationsFiltering()
        {
            var path = PathGenerator.Create(20);
            var movements = _sot.MapRelative(path, _tokenizer);
            Assert.Equal(path.Count(c => c == 'M'), movements.Count()); // MapRelative removes all 'R' and 'L' tokens
        }

        [Theory]
        [MemberData(nameof(TestDataSimple))]
        public void TestPathMapperSimple((int, int) expected, string path)
        {
            var movements = _sot.MapRelative(path, _tokenizer).ToArray();
            var final = movements[^1];
            Assert.Equal(expected, final.Head);
        }

        [Fact]
        public void TestPathMapperCanAcceptEmptyData()
        {
            var path = "";
            var movements = _sot.MapRelative(path, _tokenizer);
            Assert.Empty(movements);

            // Default tokenizer trims tail rotations
            path = "LLLRRRLRLR";
            movements = _sot.MapRelative(path, _tokenizer);
            Assert.Empty(movements);
        }

        [Fact]
        public void TestPathMapperAbsolute()
        {
            var path = "MMLMMLMRMLLM";
            var start = new Position { X = 0, Y = 0, Heading = Arrow.Create(Heading.South) };

            var movements = _sot.MapRelative(path, _tokenizer);
            var trace = _sot.MapAbsolute(movements, start).ToArray();
            Assert.Equal(path.Count(c => c == 'M') + 1, trace.Length); // +1 for initialPosition

            // Initial position
            Assert.Equal(0, trace[0].X);
            Assert.Equal(0, trace[0].Y);

            // two steps to South
            Assert.Equal(1, trace[1].X);
            Assert.Equal(0, trace[1].Y);
            Assert.Equal(2, trace[2].X);
            Assert.Equal(0, trace[2].Y);

            // two steps to East
            Assert.Equal(2, trace[3].X);
            Assert.Equal(1, trace[3].Y);
            Assert.Equal(2, trace[4].X);
            Assert.Equal(2, trace[4].Y);

            // one step to North
            Assert.Equal(1, trace[5].X);
            Assert.Equal(2, trace[5].Y);

            // one step to East
            Assert.Equal(1, trace[6].X);
            Assert.Equal(3, trace[6].Y);

            // one step to West
            Assert.Equal(1, trace[7].X);
            Assert.Equal(2, trace[7].Y);
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
