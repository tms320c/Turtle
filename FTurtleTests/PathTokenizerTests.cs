using System;
using System.Linq;
using FTurtle;
using FTurtle.Domain;
using FTurtleTests.Tools;
using Xunit;
using Xunit.Abstractions;
using Xunit.Sdk;

namespace FTurtleTests
{
    public class PathTokenizerTests
    {
        private readonly ITestOutputHelper _output;

        public PathTokenizerTests(ITestOutputHelper output)
        {
            _output = output;
        }

        [Fact]
        public void TestPathTokenizerDefault()
        {
            var path = PathGenerator.Create(20);
            _output.WriteLine("Path: " + path);

            var tokenizer = new PathTokenizerDefault();

            var tokens = tokenizer.Parse(path);
            Assert.True(path.Length >= tokens.Count()); // may cut L and R at the tail

            var enumerable = tokens as char[] ?? tokens.ToArray();

            for (var i = 0; i < enumerable.Length; ++i)
            {
                Assert.Equal(path[i], enumerable[i]);
            }
        }

        [Fact]
        public void TestPathTokenizerDefaultTrimRotationsAtTail()
        {
            var pathHead = "RLMMMRLM";
            var pathTail = "LRRLLR";
            _output.WriteLine("Path: " + pathHead + pathTail);

            var tokenizer = new PathTokenizerDefault();

            var tokens = tokenizer.Parse(pathHead + pathTail);
            Assert.Equal(pathHead.Length, tokens.Count());
            var enumerable = tokens as char[] ?? tokens.ToArray();
            Assert.True(enumerable[^1] == 'M');
        }
    }
}
