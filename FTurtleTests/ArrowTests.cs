using System;
using System.Collections.Generic;
using FTurtle;
using FTurtle.Application;
using FTurtle.Domain;
using TurtleTests.Tools;
using TurtleWorld.Core;
using Xunit;
using Xunit.Abstractions;
using Xunit.Sdk;

namespace TurtleTests
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
            Assert.Equal((-1, 0), Heading.North);
            Assert.Equal((0, 1), Heading.East);
            Assert.Equal((1, 0), Heading.South);
            Assert.Equal((0, -1), Heading.West);
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
            yield return new object[] { Heading.North, Heading.North };
            yield return new object[] { Heading.East, Heading.East };
            yield return new object[] { Heading.South, Heading.South };
            yield return new object[] { Heading.West, Heading.West };
        }

        public static IEnumerable<object[]> TestData2()
        {
            yield return new object[] { Heading.North, Heading.North.Item1, Heading.North.Item2 };
            yield return new object[] { Heading.East, Heading.East.Item1, Heading.East.Item2 };
            yield return new object[] { Heading.South, Heading.South.Item1, Heading.South.Item2 };
            yield return new object[] { Heading.West, Heading.West.Item1, Heading.West.Item2 };
        }

        public static IEnumerable<object[]> TestDataRotateRight()
        {
            yield return new object[] { Heading.East, Heading.North };
            yield return new object[] { Heading.South, Heading.East };
            yield return new object[] { Heading.West, Heading.South };
            yield return new object[] { Heading.North, Heading.West };
        }

        public static IEnumerable<object[]> TestDataRotateLeft()
        {
            yield return new object[] { Heading.West, Heading.North };
            yield return new object[] { Heading.North, Heading.East };
            yield return new object[] { Heading.East, Heading.South };
            yield return new object[] { Heading.South, Heading.West };
        }

    }
}
