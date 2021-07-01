using System;
using System.Collections.Generic;
using System.Linq;
using FTurtle;
using FTurtle.Application;
using FTurtle.Domain;
using TurtleTests.Tools;
using TurtleWorld.Core;
using TurtleWorld.Entities;
using TurtleWorld.Structure.Collision;
using Xunit;
using Xunit.Abstractions;
using Xunit.Sdk;

namespace TurtleTests
{
    public class AvoidanceStrategyTests
    {
        private readonly ITestOutputHelper _output;
        private readonly IBoard _board;

        public static readonly int Width = 10;
        public static readonly int Height = 10;

        public AvoidanceStrategyTests(ITestOutputHelper output)
        {
            _output = output;
            _board = new MockBoard(Width, Height);
        }

        [Theory]
        [MemberData(nameof(TestDataForClip))]
        public void ClipStrategyShouldClip(Position expected, Position toFix)
        {
            var sot = BoundaryAvoidanceFactory.Create(StrategyKind.Clip);
            var position = sot(toFix, _board);
            Assert.Equal(expected, position);
            Assert.Equal(expected.Heading, position.Heading); // Position.Heading does not contribute to equality
        }

        [Theory]
        [MemberData(nameof(TestDataForBounce))]
        public void BounceStrategyShouldBounce(Position expected, Position toFix)
        {
            var sot = BoundaryAvoidanceFactory.Create(StrategyKind.Bounce);
            var position = sot(toFix, _board);
            Assert.Equal(expected, position);
            Assert.Equal(expected.Heading, position.Heading); // Position.Heading does not contribute to equality
        }

        [Theory]
        [MemberData(nameof(TestDataForTurnLeft))]
        public void TurnLeftStrategyShouldClipAndTurn(Position expected, Position toFix)
        {
            var sot = BoundaryAvoidanceFactory.Create(StrategyKind.TurnLeft);
            var position = sot(toFix, _board);
            Assert.Equal(expected, position);
            Assert.Equal(expected.Heading, position.Heading); // Position.Heading does not contribute to equality
        }

        public static IEnumerable<object[]> TestDataForClip()
        {
            yield return new object[] // inside. Should not be changed
            {
                new Position
                {
                    X = 0, Y = 0
                },
                new Position
                {
                    X = 0, Y = 0
                },
            };
            yield return new object[]
            {
                new Position
                {
                    X = 0, Y = 0
                },
                new Position
                {
                    X = -17, Y = 0
                },
            };
            yield return new object[]
            {
                new Position
                {
                    X = 0, Y = 0
                },
                new Position
                {
                    X = 0, Y = -42
                },
            };
            yield return new object[]
            {
                new Position
                {
                    X = Height - 1, Y = 0
                },
                new Position
                {
                    X = Height, Y = 0
                },
            };
            yield return new object[]
            {
                new Position
                {
                    X = 0, Y = Width - 1
                },
                new Position
                {
                    X = 0, Y = Width
                },
            };
        }

        public static IEnumerable<object[]> TestDataForBounce()
        {
            yield return new object[] // inside. Should not be changed
            {
                new Position
                {
                    X = 0, Y = 0
                },
                new Position
                {
                    X = 0, Y = 0
                },
            };
            yield return new object[]
            {
                new Position
                {
                    X = 1, Y = 0
                },
                new Position
                {
                    X = -17, Y = 0
                },
            };
            yield return new object[]
            {
                new Position
                {
                    X = 0, Y = 1
                },
                new Position
                {
                    X = 0, Y = -42
                },
            };
            yield return new object[]
            {
                new Position
                {
                    X = Height - 2, Y = 0
                },
                new Position
                {
                    X = Height, Y = 0
                },
            };
            yield return new object[]
            {
                new Position
                {
                    X = 0, Y = Width - 2
                },
                new Position
                {
                    X = 0, Y = Width
                },
            };
        }
        public static IEnumerable<object[]> TestDataForTurnLeft()
        {
            yield return new object[] // inside. Should not be changed
            {
                new Position
                {
                    X = 0, Y = 0, Heading = Heading.Void // means "do not change heading"
                },
                new Position
                {
                    X = 0, Y = 0, Heading = Heading.South
                },
            };
            // all the rest data are outside.
            // Expected heading is always West because this is not calculated direction, but just a signal to tracer.
            // West means "turn left", but actual direction depends on the current turtle coordinate frame.
            yield return new object[]
            {
                new Position
                {
                    X = 0, Y = 0, Heading = Heading.West
                },
                new Position
                {
                    X = -17, Y = 0, Heading = Heading.North
                },
            };
            yield return new object[]
            {
                new Position
                {
                    X = 0, Y = 0, Heading = Heading.West
                },
                new Position
                {
                    X = 0, Y = -42, Heading = Heading.West
                },
            };
            yield return new object[]
            {
                new Position
                {
                    X = Height - 1, Y = 0, Heading = Heading.West
                },
                new Position
                {
                    X = Height, Y = 0, Heading = Heading.South
                },
            };
            yield return new object[]
            {
                new Position
                {
                    X = 0, Y = Width - 1, Heading = Heading.West
                },
                new Position
                {
                    X = 0, Y = Width, Heading = Heading.East
                },
            };
        }

    }

    internal class MockBoard : IBoard
    {
        public MockBoard(int width, int height)
        {
            Width = width;
            Height = height;
        }

        public int Width { get; }
        public int Height { get; }
        public Position Target { get; }
        public bool IsInside(int x, int y)
        {
            return x >= 0 && x < Height && y >= 0 && y < Width;
        }

        public bool IsInside(Position position)
        {
            return IsInside(position.X, position.Y);
        }

        public bool HasMine(Position position)
        {
            return false;
        }

        public bool HasMine(int x, int y)
        {
            return false;
        }

        public void AddMine(Position position)
        {
            return;
        }

        public void AddMine(int x, int y)
        {
            return;
        }
    }

}
