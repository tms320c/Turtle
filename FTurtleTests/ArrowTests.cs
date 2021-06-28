using System;
using System.Collections.Generic;
using FTurtle;
using FTurtle.Domain;
using FTurtleTests.Tools;
using Xunit;
using Xunit.Abstractions;
using Xunit.Sdk;

namespace FTurtleTests
{
    public class ArrowTests
    {
        private readonly ITestOutputHelper _output;

        public ArrowTests(ITestOutputHelper output)
        {
            _output = output;
        }

        [Fact]
        public void ArrowHeadingsAreProper()
        {
            Assert.Equal((-1, 0), Arrow.North);
            Assert.Equal((0, 1), Arrow.East);
            Assert.Equal((1, 0), Arrow.South);
            Assert.Equal((0, -1), Arrow.West);
        }

        [Theory]
        [MemberData(nameof(TestData))]
        public void ArrowCanBeCreatedHeadingToProperDirection((int, int) expected, (int, int) pointTo)
        {
            var sot = Arrow.Create(pointTo);
            Assert.Equal(expected, sot.Head);
        }

        [Theory]
        [MemberData(nameof(TestData2))]
        public void ArrowCanBeCreatedHeadingToProperDirection2((int, int) expected, int x, int y)
        {
            var sot = Arrow.Create(x, y);
            Assert.Equal(expected, sot.Head);
        }

        [Theory]
        [MemberData(nameof(TestDataRotateRight))]
        public void ArrowCanBeRotatedRight((int, int) expected, (int, int) pointTo)
        {
            var sot = Arrow.Create(pointTo);
            var rotated = sot.RotateRight();
            Assert.Equal(expected, rotated.Head);
        }

        [Theory]
        [MemberData(nameof(TestDataRotateLeft))]
        public void ArrowCanBeRotatedLeft((int, int) expected, (int, int) pointTo)
        {
            var sot = Arrow.Create(pointTo);
            var rotated = sot.RotateLeft();
            Assert.Equal(expected, rotated.Head);
        }

        public static IEnumerable<object[]> TestData()
        {
            yield return new object[] { Arrow.North, Arrow.North };
            yield return new object[] { Arrow.East, Arrow.East };
            yield return new object[] { Arrow.South, Arrow.South };
            yield return new object[] { Arrow.West, Arrow.West };
        }

        public static IEnumerable<object[]> TestData2()
        {
            yield return new object[] { Arrow.North, Arrow.North.Item1, Arrow.North.Item2 };
            yield return new object[] { Arrow.East, Arrow.East.Item1, Arrow.East.Item2 };
            yield return new object[] { Arrow.South, Arrow.South.Item1, Arrow.South.Item2 };
            yield return new object[] { Arrow.West, Arrow.West.Item1, Arrow.West.Item2 };
        }

        public static IEnumerable<object[]> TestDataRotateRight()
        {
            yield return new object[] { Arrow.East, Arrow.North };
            yield return new object[] { Arrow.South, Arrow.East };
            yield return new object[] { Arrow.West, Arrow.South };
            yield return new object[] { Arrow.North, Arrow.West };
        }

        public static IEnumerable<object[]> TestDataRotateLeft()
        {
            yield return new object[] { Arrow.West, Arrow.North };
            yield return new object[] { Arrow.North, Arrow.East };
            yield return new object[] { Arrow.East, Arrow.South };
            yield return new object[] { Arrow.South, Arrow.West };
        }

    }
}
