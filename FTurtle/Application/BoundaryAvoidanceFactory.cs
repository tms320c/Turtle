using System;
using System.Collections.Concurrent;
using FTurtle.Infrastructure;
using TurtleWorld.Core;
using TurtleWorld.Structure;

namespace FTurtle.Application
{
    /// <summary>
    /// Builds the configuration. Keeps the builders in "secret" and provides clients with
    /// either building delegate, or ready configuration.
    /// </summary>
    public static class BoundaryAvoidanceFactory
    {
        // ConcurrentDictionary as a first step to multithreading; and I love GetOrAdd method.
        private static readonly ConcurrentDictionary<StrategyKind, IBoundaryAvoidanceStrategy> Builders = new ConcurrentDictionary<StrategyKind, IBoundaryAvoidanceStrategy>();

        /// <summary>
        /// Creates or reuses boundary avoidance strategy.
        /// It is responsibility of a strategy to be thread safe and re entrant.
        /// </summary>
        /// <param name="kind">What to build or get</param>
        /// <param name="creator">Delegate that creates an instance</param>
        /// <returns>Strategy implementation delegate</returns>
        public static Func<Position, IBoard, Position> Create(StrategyKind kind, Func<IBoundaryAvoidanceStrategy> creator = null)
        {
            return Builders.GetOrAdd(kind,
                creator?.Invoke() ?? kind switch
                {
                    StrategyKind.Clip => new ClipStrategy(),
                    StrategyKind.Bounce => new BounceStrategy(),
                    StrategyKind.TurnLeft => new TurnLeftStrategy(),
                    _ => ThrowIt(kind)
                }).Fix;
        }

        private static IBoundaryAvoidanceStrategy ThrowIt(StrategyKind kind)
        {
            throw new NotImplementedException("Strategy is not supported yet");
        }
    }
}
