using System;
using TurtleTests.Tools;
using Xunit;
using Xunit.Abstractions;
using Xunit.Sdk;

namespace TurtleTests
{
    public class TestToolsTests
    {
        private ITestOutputHelper _output;

        public TestToolsTests(ITestOutputHelper output)
        {
            _output = output;
        }

        [Fact]
        public void TestPathGeneratorNormal()
        {
            var path = PathGenerator.Create(10);
            Assert.True(path.GetTypeCode() == TypeCode.String);
            Assert.Equal(10, path.Length);
            _output.WriteLine("Path is " + path);
        }

        [Fact]
        public void TestPathGeneratorLimits()
        {
            var path1 = PathGenerator.Create(0);
            Assert.True(path1.GetTypeCode() == TypeCode.String);
            Assert.Equal(1, path1.Length);

            var path2 = PathGenerator.Create(100000000);
            Assert.True(path2.GetTypeCode() == TypeCode.String);
            Assert.Equal(32768, path2.Length);
        }
    }
}
