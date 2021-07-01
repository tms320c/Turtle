using System;
using System.Collections.Generic;
using System.Linq;
using FTurtle;
using FTurtle.Application;
using FTurtle.Domain;
using FTurtle.Infrastructure;
using TurtleTests.Tools;
using TurtleWorld.Core;
using TurtleWorld.Structure;
using TurtleWorld.Structure.Exceptions;
using Xunit;
using Xunit.Abstractions;
using Xunit.Sdk;

namespace TurtleTests
{
    public class ConfigBuilderTests
    {
        private readonly ITestOutputHelper _output;
        private readonly StandardConfigBuilder _sot;

        public ConfigBuilderTests(ITestOutputHelper output)
        {
            _output = output;
            _sot = new StandardConfigBuilder();
        }

        [Fact]
        public void ShouldThrowExceptionIfConfigurationIsNotReady()
        {
            var ex = Assert.Throws<ConfigurationNotReadyException>(() => _sot.Get());
            _output.WriteLine(ex.Message);
        }

        [Theory]
        [MemberData(nameof(TestConfigDataAcceptable))]
        public void ShouldCreateConfigurationFromReasonableText(string[] configData)
        {
            foreach (var line in configData)
            {
                _sot.Build(line);
            }

            var config = _sot.Get();
            
            Assert.Equal(4, config.Target.Y);
            Assert.Equal(2, config.Target.X);

            Assert.Equal(1, config.Start.X);
            Assert.Equal(0, config.Start.Y);
            Assert.Equal(Heading.North, config.Start.Heading);

            var board = config.Board;
            Assert.Equal(5, board.Width);
            Assert.Equal(4, board.Height);

            Assert.True(board.HasMine(new Position {X = 1, Y = 1}));
            Assert.True(board.HasMine(new Position {X = 3, Y = 1}));
            Assert.True(board.HasMine(new Position {X = 3, Y = 3}));

            var moves = config.Moves.ToList();
            Assert.Equal(2, moves.Count);
            Assert.Equal("RMLMM", moves[0]);
            Assert.Equal("RMMM", moves[1]);
        }

        [Theory]
        [MemberData(nameof(TestConfigDataNegativeNumbersNotAcceptable))]
        public void ShouldThrowIfAnyNumberIsNegative(string[] configData)
        {
            foreach (var line in configData)
            {
                _sot.Build(line);
            }
            // negative positions (but mines) are totally unacceptable. So, Not Ready exception
            var ex = Assert.Throws<ConfigurationNotReadyException>(() => _sot.Get());
            _output.WriteLine(ex.Message);
        }

        /// //////////////////////////////////////////////////////////////////////////////
        /// 
        public static IEnumerable<object[]> TestConfigDataAcceptable()
        {
            yield return new object[]
            {
                new string[]
                {
                    "5 4", // width height
                    "1,1 1,3 3,3", // mines (Y,X)
                    "4 2", // exit Y X
                    "0 1 N", // start Y X heading
                    "R M L M M", // path 1
                    "R M M M" // path 2
                }
            };

            yield return new object[]
            {
                new string[]
                {
                    "  5   4,", // width height
                    "   ",
                    "      ",
                    "   qqqq   ",
                    ",1, 1  1 , 3  3,,3", // mines (Y,X)
                    "4 2", // exit Y X
                    "0   1 n", // start Y X heading
                    " r m   l M M", // path 1
                    "R M m   M " // path 2
                }
            };

        }

        public static IEnumerable<object[]> TestConfigDataNegativeNumbersNotAcceptable()
        {

            yield return new object[]
            {
                new string[]
                {
                    "5 4", // width height
                    "1,1 1,3 3,3", // mines (Y,X)
                    "4 -2", // exit Y X
                    "0 1 N", // start Y X heading
                    "R M L M M", // path 1
                    "R M M M" // path 2
                }
            };

            yield return new object[]
            {
                new string[]
                {
                    "5 4", // width height
                    "1,1 1,3 3,3", // mines (Y,X)
                    "4 2", // exit Y X
                    "0 -1 N", // start Y X heading
                    "R M L M M", // path 1
                    "R M M M" // path 2
                }
            };

        }

    }
}
