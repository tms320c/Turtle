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
            Assert.Equal(path.Count(c => c == 'M'), movements.Count); // MapRelative removes all 'R' and 'L' tokens
        }

        [Theory]
        [MemberData(nameof(TestDataSimple))]
        public void TestPathMapperSimple((int, int) expected, string path)
        {
            var movements = _sot.MapRelative(path, _tokenizer);
            var final = movements[^1];
            Assert.Equal(expected, final.Head);
        }

        [Fact]
        public void TestPathMapperCanAcceptEmptyData()
        {
            var path = "";
            var movements = _sot.MapRelative(path, _tokenizer);
            Assert.Equal(0, movements.Count);

            // Default tokenizer trims tail rotations
            path = "LLLRRRLRLR";
            movements = _sot.MapRelative(path, _tokenizer);
            Assert.Equal(0, movements.Count);
        }

        public static IEnumerable<object[]> TestDataSimple()
        {
            yield return new object[] { Arrow.North, "MMMMMM" };
            yield return new object[] { Arrow.East, "MRMMLRLR" };
            yield return new object[] { Arrow.South, "LMMLM" };
            yield return new object[] { Arrow.West, "MRMRMMRMM" };
        }
    }
}
