using System;
using System.Collections.Generic;
using System.Linq;
using FTurtle;
using FTurtle.Application;
using FTurtle.Domain;
using NSubstitute;
using TurtleTests.Tools;
using TurtleWorld.Core;
using TurtleWorld.Structure;
using Xunit;
using Xunit.Abstractions;
using Xunit.Sdk;

namespace TurtleTests
{
    public class PathMapperTests : IClassFixture<PathTokenizer>
    {
        private readonly ITestOutputHelper _output;
        private readonly RelativeMapper _sot2;
        private readonly IPathTokenizer _tokenizer;
        private readonly IBoard _boardMock;

        public PathMapperTests(ITestOutputHelper output, PathTokenizer tokenizer)
        {
            _output = output;
            _tokenizer = tokenizer;
            // internal to FTurtle assembly. Has been made visible to the tests in Directory.build.props
            // the reason is its algorithm complexity.
            _sot2 = new RelativeMapper();

            // Use mock instead of local class which implements IBoard because I want to assert that some methods are invoked.
            _boardMock = Substitute.For<IBoard>();
        }

        [Fact]
        public void InternalPathMapperShouldFilterOutAllRotations()
        {
            var path = PathGenerator.Create(20);
            var movements = _sot2.Map(_tokenizer.Parse(path), Heading.North);
            Assert.Equal(path.Count(c => c == (char)Command.Move), movements.Count()); // MapRelative removes all 'R' and 'L' tokens
        }

        [Theory]
        [MemberData(nameof(TestDataSimple))]
        public void InternalPathMapperCanHandleNonCornerCases((int, int) expected, string path)
        {
            var movements = _sot2.Map(_tokenizer.Parse(path), Heading.North).ToArray();
            var final = movements[^1];
            Assert.Equal(expected, final.Head);
        }

        [Fact]
        public void ConstructorShouldThrowExceptions()
        {
            var ex = Assert.Throws<ArgumentNullException>(() => new PathMapper(null));
            Assert.Equal("board", ex.ParamName);
        }

        [Fact]
        public void InternalPathMapperCanAcceptEmptyDataAndMapItToEmptyCollection()
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
        public void ShouldGenerateCorrectCoordinates()
        {
            var sot = new PathMapper(_boardMock);

            var path = "MMLMMLMRMLLM";
            var start = new Position { X = 0, Y = 0, Heading = Heading.South };

            // Do not test collision detection here.
            var trace = sot.Map(_tokenizer.Parse(path), start, null).ToArray();
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

        [Fact]
        public void ShouldInvokeConstraintHandlerThatEnforcesCoordinatesClipping()
        {
            var boardWidth = 10;
            var boardHeight = 10;
            _boardMock.Height.Returns(boardHeight);
            _boardMock.Width.Returns(boardWidth);

            var sot = new PathMapper(_boardMock);

            var path = "MMLMMLMRMLLM";

            // close to the bottom of board. Will collide with the wall on the 2nd step.
            var start = new Position { X = boardHeight - 2, Y = 0, Heading = Heading.South };

            var trace = sot.Map(_tokenizer.Parse(path), start, (p, b) => new Position
            {
                // Simple clipping strategy.
                // The turtle stays at the boundary until rotation command (or end of the path)
                X = p.X >= b.Height 
                    ? b.Height - 1 
                    : (p.X < 0 
                        ? 0 
                        : p.X),
                Y = p.Y >= b.Width 
                    ? b.Width - 1 
                    : (p.Y < 0 
                        ? 0 
                        : p.Y),
                // Heading does not matter
            }).ToArray();

            Assert.Equal(path.Count(c => c == (char)Command.Move) + 1, trace.Length); // +1 for initialPosition
            _ = _boardMock.Received().Height;
            _ = _boardMock.Received().Width;

            var expected = new (int, int)[]
            {
                // Initial position
                (start.X, start.Y),
                // two steps to South (initial Heading is South)
                (9, 0),
                (9, 0), // supposed to be 10 but should be clipped by the constraintHandler
                // two steps to East
                (9, 1),
                (9, 2), 
                // one step to North
                (8, 2), 
                // one step to East
                (8, 3),
                // one step to West
                (8, 2)
            };

            for (var i = 0; i < expected.Length; ++i)
            {
                Assert.Equal(expected[i], (trace[i].X, trace[i].Y));
            }
        }

        [Fact]
        public void ShouldInvokeConstraintHandlerThatEnforcesBounce()
        {
            var boardWidth = 10;
            var boardHeight = 10;
            _boardMock.Height.Returns(boardHeight);
            _boardMock.Width.Returns(boardWidth);

            var sot = new PathMapper(_boardMock);

            var path = "MMLMMLMRMLLM";

            // close to the bottom of board. Will collide with the wall on the 2nd step.
            var start = new Position { X = boardHeight - 2, Y = 0, Heading = Heading.South };

            var trace = sot.Map(_tokenizer.Parse(path), start, (p, b) =>
            {
                // this strategy makes a bounce: the turtle bounces from a boundary (moves one step back) but keeps the heading
                var h = b.Height;
                var w = b.Width;

                var x = p.X >= h
                    ? p.X - 2 // may go off board for 1 row boards (will be -1 then)
                    : (p.X < 0
                        ? 1 // may go off board for 1 row boards (will be 1 then)
                        : p.X);

                var y = p.Y >= w
                    ? p.Y - 2 // may go off board for 1 column boards (will be -1 then)
                    : (p.Y < 0
                        ? 1 // may go off board for 1 column boards (will be 1 then)
                        : p.Y);

                return new Position
                {
                    X = x < 0 || x >= h ? h - 1 : x, // second check, see the comments above
                    Y = y < 0 || y >= w ? w - 1 : y
                    // Heading does not matter
                };
            }).ToArray();

            Assert.Equal(path.Count(c => c == (char)Command.Move) + 1, trace.Length); // +1 for initialPosition
            _ = _boardMock.Received().Height;
            _ = _boardMock.Received().Width;

            var expected = new (int, int)[]
            {
                // Initial position
                (start.X, start.Y),
                // two steps to South (initial Heading is South)
                (9, 0),
                (8, 0), // supposed to be 10 but should bounce by the constraintHandler
                // two steps to East
                (8, 1),
                (8, 2), 
                // one step to North
                (7, 2), 
                // one step to East
                (7, 3),
                // one step to West
                (7, 2)
            };

            for (var i = 0; i < expected.Length; ++i)
            {
                Assert.Equal(expected[i], (trace[i].X, trace[i].Y));
            }
        }

        [Fact]
        public void ShouldInvokeConstraintHandlerThatEnforcesTurn()
        {
            var boardWidth = 10;
            var boardHeight = 10;
            _boardMock.Height.Returns(boardHeight);
            _boardMock.Width.Returns(boardWidth);

            var sot = new PathMapper(_boardMock);

            var path = "MMLMMLMRMLLM";

            // close to the bottom of board. Will collide with the wall on the 2nd step.
            var start = new Position { X = boardHeight - 2, Y = 0, Heading = Heading.South };

            var trace = sot.Map(_tokenizer.Parse(path), start, (p, b) =>
            {
                // this strategy makes a turn: the turtle turns to move along the boundary maybe, because:
                // if the next command is L or R then the heading will be changed
                var h = b.Height;
                var w = b.Width;

                // Simple clipping at first.
                var collisionDetected = p.X >= h || p.X < 0 || p.Y >= w || p.Y < 0;

                var x = p.X >= h
                    ? h - 1
                    : (p.X < 0
                        ? 0
                        : p.X);
                var y = p.Y >= w
                    ? w - 1
                    : (p.Y < 0
                        ? 0
                        : p.Y);

                return new Position
                {
                    X = x,
                    Y = y,
                    Heading = collisionDetected ? Heading.West : Heading.Void // left turn is required if collision
                };
            }).ToArray();

            Assert.Equal(path.Count(c => c == (char)Command.Move) + 1, trace.Length); // +1 for initialPosition
            _ = _boardMock.Received().Height;
            _ = _boardMock.Received().Width;

            var expected = new (int, int)[]
            {
                // Initial position
                (start.X, start.Y),
                // two steps to South (initial Heading is South) MM
                (9, 0), // M
                (9, 0), // M . supposed to be 10 but should be clipped by the constraintHandler. Turtle also turned left by constraintHandler and the heading is East!
                // then it has got L command, so next two steps to North (left from East is North)
                (8, 0), // M
                (7, 0), // M and got L.  left from North is West
                // one step to West
                (7, 0), // M clipped because of the collision with left boundary and constraintHandler set the heading to South
                // got R command. R from South is West
                (7, 0), // M hits the boundary again and constraintHandler set the heading to South
                // 2 L commands from South is North
                (6, 0) // M one step to North
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
