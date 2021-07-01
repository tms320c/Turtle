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
    public class MineFieldTests
    {
        private readonly ITestOutputHelper _output;
        private readonly IMineField _sot;

        public MineFieldTests(ITestOutputHelper output)
        {
            _output = output;
            _sot = new MineField();
        }

        [Fact]
        public void ShouldAcceptTheMinesAndFindThem()
        {
            var p1 = new Position
            {
                X = 0, Y = 0
            };
            var p2 = new Position
            {
                X = 1, Y = 1
            };
            Assert.False(_sot.HasMine(p1));
            Assert.False(_sot.HasMine(p2));

            _sot.SetMine(p1);
            Assert.True(_sot.HasMine(p1));
            Assert.False(_sot.HasMine(p2));

            _sot.SetMine(p2);
            Assert.True(_sot.HasMine(p1));
            Assert.True(_sot.HasMine(p2));
        }
    }
}
