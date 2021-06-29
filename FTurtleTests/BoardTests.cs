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
    public class BoardTests
    {
        private readonly ITestOutputHelper _output;
        private readonly IMineField _mines;

        public BoardTests(ITestOutputHelper output)
        {
            _output = output;
            _mines = new MineField();
        }

        [Fact]
        public void ConstructorShouldThrowExceptions()
        {
            var exception = Assert.Throws<ArgumentOutOfRangeException>(() => new Board(-1, 0, null));
            Assert.Equal("width", exception.ParamName);

            exception = Assert.Throws<ArgumentOutOfRangeException>(() => new Board(1, 0, null));
            Assert.Equal("height", exception.ParamName);

            var ex = Assert.Throws<ArgumentNullException>(() => new Board(1, 1, null));
            Assert.Equal("mines", ex.ParamName);

        }

        [Fact]
        public void ShouldReportCorrectDimensions()
        {
            var width = 17;
            var height = 42;
            var board = new Board(width, height, _mines);
            Assert.Equal(width, board.Width);
            Assert.Equal(height, board.Height);
        }

        [Fact]
        public void ShouldReportIfHasMineAtPosition()
        {
            var x = 17;
            var y = 42;
            _mines.SetMine(new Position {X = x, Y = y});

            var board = new Board(100, 100, _mines);
            Assert.True(board.HasMine(x, y));
            Assert.False(board.HasMine(0, 0));
        }

        [Fact]
        public void ShouldThrowExceptionIfPositionIsOutside()
        {
            var x = 17;
            var y = 42;

            var width = 10;
            var height = 10;

            var board = new Board(width, height, _mines);

            var ex = Assert.Throws<ArgumentException>(() => board.HasMine(x, y));
            _output.WriteLine(ex.Message);

            ex = Assert.Throws<ArgumentException>(() => board.HasMine(-1, 0));
            _output.WriteLine(ex.Message);

            ex = Assert.Throws<ArgumentException>(() => board.HasMine(0, -1));
            _output.WriteLine(ex.Message);

            ex = Assert.Throws<ArgumentException>(() => board.HasMine(9, height));
            _output.WriteLine(ex.Message);
            
            ex = Assert.Throws<ArgumentException>(() => board.HasMine(width, 0));
            _output.WriteLine(ex.Message);
        }
    }
}
