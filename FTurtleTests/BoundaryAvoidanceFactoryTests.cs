using System;
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
    public class BoundaryAvoidanceFactoryTests
    {
        private readonly ITestOutputHelper _output;

        public BoundaryAvoidanceFactoryTests(ITestOutputHelper output)
        {
            _output = output;
        }

        [Fact]
        public void ShouldCreateInstancesOfProperTypeOrThrow()
        {
            foreach (var kind in Enum.GetValues(typeof(StrategyKind)).Cast<StrategyKind>() )
            {
                if (kind == StrategyKind.Custom)
                {
                    _ = Assert.Throws<NotImplementedException>(() => BoundaryAvoidanceFactory.Create(kind));
                    Assert.IsType<Func<Position, IBoard, Position>>(BoundaryAvoidanceFactory.Create(kind, () => new MockStrategy()));
                    continue;
                }
                Assert.IsType<Func<Position, IBoard, Position>>(BoundaryAvoidanceFactory.Create(kind));
            }
        }

        internal class MockStrategy : IBoundaryAvoidanceStrategy
        {
            public Position Fix(Position position, IBoard board)
            {
                return position;
            }
        }
    }
}
