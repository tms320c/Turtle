using System;
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
    public class PathTokenizerTests : IClassFixture<PathTokenizerDefault>
    {
        private readonly ITestOutputHelper _output;
        private readonly IPathTokenizer _defaultTokenizer;

        public PathTokenizerTests(ITestOutputHelper output, PathTokenizerDefault defaultTokenizer)
        {
            _output = output;
            _defaultTokenizer = defaultTokenizer;
        }

        [Fact]
        public void ShouldParseRandomStringsWithProperSymbols()
        {
            var path = PathGenerator.Create(20);
            _output.WriteLine("Path: " + path);

            var tokens = _defaultTokenizer.Parse(path, null);
            Assert.True(path.Length >= tokens.Count()); // may cut L and R at the tail

            var enumerable = tokens as Command[] ?? tokens.ToArray();

            for (var i = 0; i < enumerable.Length; ++i)
            {
                Assert.Equal(path[i], (char)enumerable[i]);
            }
        }

        [Fact]
        public void ShouldTrimRotationCommandsFromTheStringTail()
        {
            var pathHead = "RLMMMRLM";
            var pathTail = "LRRLLR";
            _output.WriteLine("Path: " + pathHead + pathTail);

            var tokens = _defaultTokenizer.Parse(pathHead + pathTail);
            Assert.Equal(pathHead.Length, tokens.Count());
            var enumerable = tokens as Command[] ?? tokens.ToArray();
            Assert.True(enumerable[^1] == Command.Move);
        }

        [Fact]
        public void ShouldProcessInvalidTokens()
        {
            var path = "XLMMMRLM";
            _output.WriteLine("Path: " + path);

            var tokens = _defaultTokenizer.Parse(path);
            Assert.Equal(path.Length - 1, tokens.Count());
            var enumerable = tokens as Command[] ?? tokens.ToArray();
            Assert.True(enumerable[^1] == Command.Move);
        }

        [Fact]
        public void CanAcceptAndInvokeCustomConverter()
        {
            var path = "XLMMMRLM";
            _output.WriteLine("Path: " + path);
            _ = Assert.Throws<Exception>(() => _defaultTokenizer.Parse(path, (c) => throw new Exception()).ToArray());
        }
    }
}
