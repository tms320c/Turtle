using System;
using System.Linq;
using FTurtle;
using FTurtle.Application;
using FTurtle.Domain;
using FTurtleTests.Tools;
using TurtleWorld.Core;
using TurtleWorld.Entities;
using Xunit;
using Xunit.Abstractions;
using Xunit.Sdk;

namespace FTurtleTests
{
    public class BoardTests
    {
        private readonly ITestOutputHelper _output;
        private readonly IMineField _mines;
        private readonly Position _exit;

        public BoardTests(ITestOutputHelper output)
        {
            _output = output;
            _mines = new MineField();
            _exit = new Position
            {
                X = 10,
                Y = 10
            };
        }

        [Fact]
        public void ConstructorShouldThrowExceptions()
        {

            var exception = Assert.Throws<ArgumentOutOfRangeException>(() => new Board(-1, 0, _exit, null));
            Assert.Equal("width", exception.ParamName);

            exception = Assert.Throws<ArgumentOutOfRangeException>(() => new Board(1, 0, _exit, null));
            Assert.Equal("height", exception.ParamName);

            var ex = Assert.Throws<ArgumentNullException>(() => new Board(1, 1, _exit, null));
            Assert.Equal("mines", ex.ParamName);

            var ex1 = Assert.Throws<ArgumentException>(() => new Board(10, 10, _exit, _mines));
            Assert.Equal("target", ex1.ParamName);
        }

        [Fact]
        public void ShouldReportCorrectDimensions()
        {
            var width = 17;
            var height = 42;
            var board = new Board(width, height, _exit, _mines);
            Assert.Equal(width, board.Width);
            Assert.Equal(height, board.Height);
        }

        [Fact]
        public void ShouldReportIfHasMineAtPosition()
        {
            var x = 17;
            var y = 42;
            _mines.SetMine(new Position {X = x, Y = y});

            var board = new Board(100, 100, _exit, _mines);
            Assert.True(board.HasMine(x, y));
            Assert.False(board.HasMine(0, 0));
        }

        [Fact]
        public void ShouldAddMineAtPosition()
        {
            var x = 17;
            var y = 42;
            var width = 50;
            var height = 20;

            var board = new Board(width, height, _exit, _mines);

            board.AddMine(new Position { X = x, Y = y });

            for (var i = 0; i < height; ++i)
            {
                for (var j = 0; j < width; ++j)
                {
                    if (i == x && j == y)
                    {
                        Assert.True(board.HasMine(i, j));
                    }
                    else
                    {
                        Assert.False(board.HasMine(i, j));
                    }
                }
            }

            board.AddMine(0, 0);
            Assert.True(board.HasMine(0, 0));
        }

        [Fact]
        public void ShouldThrowExceptionIfPositionIsOutside()
        {
            var x = 17;
            var y = 42;

            var width = 11;
            var height = 11;

            var board = new Board(width, height, _exit, _mines);

            var ex = Assert.Throws<ArgumentException>(() => board.HasMine(x, y));
            ex = Assert.Throws<ArgumentException>(() => board.AddMine(x, y));
            _output.WriteLine(ex.Message);

            ex = Assert.Throws<ArgumentException>(() => board.HasMine(-1, 0));
            ex = Assert.Throws<ArgumentException>(() => board.AddMine(-1, 0));
            _output.WriteLine(ex.Message);

            ex = Assert.Throws<ArgumentException>(() => board.HasMine(0, -1));
            ex = Assert.Throws<ArgumentException>(() => board.AddMine(0, -1));
            _output.WriteLine(ex.Message);

            ex = Assert.Throws<ArgumentException>(() => board.HasMine(9, height));
            ex = Assert.Throws<ArgumentException>(() => board.AddMine(9, height));
            _output.WriteLine(ex.Message);
            
            ex = Assert.Throws<ArgumentException>(() => board.HasMine(width, 0));
            ex = Assert.Throws<ArgumentException>(() => board.AddMine(width, 0));
            _output.WriteLine(ex.Message);
        }
    }
}
